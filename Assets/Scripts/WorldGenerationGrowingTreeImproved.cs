using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGenerationGrowingTreeImproved : MonoBehaviour
{
    [Header("Map Generation")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minimalMapSize = .8f;
    [SerializeField]
    [Range(0, 100)]
    private int connectionChance = 50;

    [HideInInspector]
    public GameObject[,] protoWorld;
    [SerializeField]
    private GameObject element;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float torchRemovalChance;
    private List<GameObject> activeList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void GenerateWorld() {
        int worldWidth = GameDetails.Instance.worldWidth;
        int worldHeight = GameDetails.Instance.worldHeight;

        while (GameObject.FindGameObjectsWithTag("Decided").Length < worldWidth * worldHeight * minimalMapSize)
        {
            ClearPreviousGeneration();
            activeList.Clear();
            protoWorld = new GameObject[worldWidth, worldHeight];

            // Fill grid with undecided tiles
            for (int i = 0; i < worldWidth; i++)
            {
                for (int j = 0; j < worldHeight; j++)
                {
                    protoWorld[i, j] = Instantiate(element);
                    protoWorld[i, j].transform.parent = gameObject.transform;
                    protoWorld[i, j].transform.position = new Vector3(i * 2, 0, j * 2);
                }
            }

            // Start at center
            activeList.Add(protoWorld[worldWidth / 2, worldHeight / 2]);

            while (activeList.Count > 0)
            {
                // Pick random neightbour c
                GameObject c = activeList[Mathf.RoundToInt(Random.Range(0, activeList.Count))];
                int x = Mathf.RoundToInt(c.transform.position.x / 2);
                int y = Mathf.RoundToInt(c.transform.position.z / 2);

                // Loop through neighbours
                List<GameObject> neighbours = new List<GameObject>();
                if (x - 1 >= 0) neighbours.Add(protoWorld[x - 1, y]);
                if (x + 1 < worldWidth) neighbours.Add(protoWorld[x + 1, y]);
                if (y - 1 >= 0) neighbours.Add(protoWorld[x, y - 1]);
                if (y + 1 < worldHeight) neighbours.Add(protoWorld[x, y + 1]);

                // Find undecided neighbors
                List<GameObject> undecidedNeighbours = new List<GameObject>();
                foreach (GameObject cell in neighbours)
                {
                    if (cell.transform.tag == "Undecided")
                    {
                        undecidedNeighbours.Add(cell);
                    }
                }

                // If there are undecided neighbors, connect to at least one
                if (undecidedNeighbours.Count > 0)
                {
                    // Always connect to at least one random undecided neighbor
                    GameObject firstCell = undecidedNeighbours[Random.Range(0, undecidedNeighbours.Count)];
                    ConnectCells(c, firstCell, x, y);

                    // Only add to active list if not already present
                    if (!activeList.Contains(firstCell))
                    {
                        activeList.Add(firstCell);
                    }

                    // For remaining undecided neighbors, use connectionChance
                    foreach (GameObject cell in undecidedNeighbours)
                    {
                        if (cell != firstCell && Random.Range(0, 100) < connectionChance)
                        {
                            ConnectCells(c, cell, x, y);

                            // Only add to active list if not already present
                            if (!activeList.Contains(cell))
                            {
                                activeList.Add(cell);
                            }
                        }
                    }
                }

                c.transform.tag = "Decided";
                activeList.Remove(c);
            }

            RemoveUndecided();
            RemoveCorners();
        }

        EventManager.worldGenerated.Invoke();
    }

    void ConnectCells(GameObject c, GameObject cell, int x, int y)
    {
        int nx = Mathf.RoundToInt(cell.transform.position.x / 2);
        int ny = Mathf.RoundToInt(cell.transform.position.z / 2);
        Vector2 orientation = new Vector2(nx, ny) - new Vector2(x, y);
        GrowingTreeElement cComp = c.GetComponent<GrowingTreeElement>();
        GrowingTreeElement cellComp = cell.GetComponent<GrowingTreeElement>();

        if (orientation == Vector2.left)
        {
            Destroy(cComp.negX);
            Destroy(cellComp.posX);
            cComp.negX = null;
            cellComp.posX = null;
        }
        else if (orientation == Vector2.right)
        {
            Destroy(cComp.posX);
            Destroy(cellComp.negX);
            cComp.posX = null;
            cellComp.negX = null;
        }
        else if (orientation == Vector2.up)
        {
            Destroy(cComp.posZ);
            Destroy(cellComp.negZ);
            cComp.posZ = null;
            cellComp.negZ = null;
        }
        else if (orientation == Vector2.down)
        {
            Destroy(cComp.negZ);
            Destroy(cellComp.posZ);
            cComp.negZ = null;
            cellComp.posZ = null;
        }
    }

    private void RemoveTorches()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Torch"))
        {
            if(Random.Range(0f, 1f) < torchRemovalChance)
            {
                Destroy(g);
            }
        }
        EventManager.torchesRemoved.Invoke();
    }

    private void RemoveUndecided()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Undecided"))
        {
            Destroy(g);
        }
    }

    private void ClearPreviousGeneration()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Decided"))
        {
            Destroy(g);
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Undecided"))
        {
            Destroy(g);
        }
    }

    private static void RemoveCorners()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Decided"))
        {
            g.GetComponent<GrowingTreeElement>().RemoveCorners();
        }
    }

    void OnEnable()
    {
        EventManager.generateWorld += GenerateWorld;
        EventManager.removeTorches += RemoveTorches;
    }

    void OnDisable()
    {
        EventManager.generateWorld -= GenerateWorld;
        EventManager.removeTorches -= RemoveTorches;
    }
}
