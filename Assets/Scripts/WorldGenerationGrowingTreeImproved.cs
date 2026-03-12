using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGenerationGrowingTreeImproved : MonoBehaviour
{
    [Header("Map Generation")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minimalMapSize = .8f; // Minimum fill ratio before accepting the map
    [SerializeField]
    [Range(0, 100)]
    private int connectionChance = 50; // Percent chance to carve extra passages to neighbors

    [HideInInspector]
    public GameObject[,] protoWorld; // 2D grid of tile GameObjects
    [SerializeField]
    private GameObject element; // Prefab used for each grid cell
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float torchRemovalChance;
    private List<GameObject> activeList = new List<GameObject>(); // Frontier cells still being processed


    // Generates the maze, retrying until enough tiles are carved
    void GenerateWorld() {
        int worldWidth = GameDetails.Instance.worldWidth;
        int worldHeight = GameDetails.Instance.worldHeight;

        // Retry until the map meets the minimum fill ratio
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

            // Seed the algorithm from the center of the grid
            activeList.Add(protoWorld[worldWidth / 2, worldHeight / 2]);

            // Growing-tree loop: process cells until the frontier is empty
            while (activeList.Count > 0)
            {
                // Pick a random cell from the active frontier
                GameObject c = activeList[Mathf.RoundToInt(Random.Range(0, activeList.Count))];
                int x = Mathf.RoundToInt(c.transform.position.x / 2);
                int y = Mathf.RoundToInt(c.transform.position.z / 2);

                // Gather valid cardinal neighbors within bounds
                List<GameObject> neighbours = new List<GameObject>();
                if (x - 1 >= 0) neighbours.Add(protoWorld[x - 1, y]);
                if (x + 1 < worldWidth) neighbours.Add(protoWorld[x + 1, y]);
                if (y - 1 >= 0) neighbours.Add(protoWorld[x, y - 1]);
                if (y + 1 < worldHeight) neighbours.Add(protoWorld[x, y + 1]);

                // Filter to only unvisited neighbors
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
                    // Guarantee one passage by connecting a random undecided neighbor
                    GameObject firstCell = undecidedNeighbours[Random.Range(0, undecidedNeighbours.Count)];
                    ConnectCells(c, firstCell, x, y);

                    if (!activeList.Contains(firstCell))
                    {
                        activeList.Add(firstCell);
                    }

                    // Optionally carve extra passages based on connectionChance
                    foreach (GameObject cell in undecidedNeighbours)
                    {
                        if (cell != firstCell && Random.Range(0, 100) < connectionChance)
                        {
                            ConnectCells(c, cell, x, y);

                            if (!activeList.Contains(cell))
                            {
                                activeList.Add(cell);
                            }
                        }
                    }
                }

                // Mark current cell as visited and remove from frontier
                c.transform.tag = "Decided";
                activeList.Remove(c);
            }

            RemoveUndecided();
            RemoveCorners();
        }

        EventManager.worldGenerated.Invoke();
    }

    // Removes walls between two adjacent cells based on their relative direction
    void ConnectCells(GameObject c, GameObject cell, int x, int y)
    {
        int nx = Mathf.RoundToInt(cell.transform.position.x / 2);
        int ny = Mathf.RoundToInt(cell.transform.position.z / 2);
        Vector2 orientation = new Vector2(nx, ny) - new Vector2(x, y); // Direction from c to cell
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

    // Randomly destroys torch objects to thin out lighting
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

    // Destroys all tiles that were never visited during generation
    private void RemoveUndecided()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Undecided"))
        {
            Destroy(g);
        }
    }

    // Destroys all existing tiles to prepare for a fresh generation pass
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

    // Tells each decided tile to clean up unnecessary corner geometry
    private static void RemoveCorners()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Decided"))
        {
            g.GetComponent<GrowingTreeElement>().RemoveCorners();
        }
    }

    // Subscribe to generation events
    void OnEnable()
    {
        EventManager.generateWorld += GenerateWorld;
        EventManager.removeTorches += RemoveTorches;
    }

    // Unsubscribe to prevent leaks
    void OnDisable()
    {
        EventManager.generateWorld -= GenerateWorld;
        EventManager.removeTorches -= RemoveTorches;
    }
}
