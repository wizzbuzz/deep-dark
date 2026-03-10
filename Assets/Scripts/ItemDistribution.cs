using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDistribution : MonoBehaviour
{
    [SerializeField]
    private float highDistribution = .2f;

    [SerializeField]
    private float midDistribution = .3f;

    [SerializeField]
    private GameObject lowPrefab, midPrefab, highPrefab;
    [SerializeField]
    private GameObject stalactitePrefab;
    [SerializeField] private int stalactiteAmount = 200;

    [SerializeField]
    private List<GameObject> placedObjects = new List<GameObject>();

    private float spawnRadius = .8f;
    private int spawnedPoints = 0;
    void DistributeCoins()
    {
        int totalPoints = GameDetails.Instance.totalPoints;

        // Convert to total coins
        highDistribution = totalPoints * highDistribution;
        midDistribution = totalPoints * midDistribution;

        for(int i = 0; i < stalactiteAmount; i++)
        {
            SpawnStalachtite();
        }


        // Spawn Bullions
        while (spawnedPoints < highDistribution)
        {
            SpawnItem(highPrefab, 10);
        }

        // Spawn Gems
        while (spawnedPoints < midDistribution)
        {
            SpawnItem(midPrefab, 5);
        }

        // Spawn Coins
        while (spawnedPoints < totalPoints)
        {
            SpawnItem(lowPrefab, 1);
        }

        EventManager.itemsDistributed.Invoke();
    }

    private void SpawnStalachtite()
    {
        GameObject tmp = Instantiate(stalactitePrefab);
        tmp.transform.position = GetRandomSpawn() - (Vector3.up * Random.Range(2.5f, 3f));
        tmp.transform.parent = gameObject.transform;
        placedObjects.Add(tmp);
    }

    private void SpawnItem(GameObject item, int value)
    {
        GameObject tmp = Instantiate(item);
        tmp.transform.position = GetRandomSpawn();
        tmp.transform.rotation = Quaternion.Euler(-90, Random.Range(0, 360), 0);
        tmp.transform.parent = gameObject.transform;
        placedObjects.Add(tmp);
        spawnedPoints += value;
    }

    private Vector3 GetRandomSpawn()
    {
        GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Floor");
        List<GameObject> validFloors = new List<GameObject>();

        foreach (GameObject floor in allFloors)
        {
            if (floor.transform.parent != null && floor.transform.parent.CompareTag("Decided"))
            {
                validFloors.Add(floor);
            }
        }

        Vector3 randomFloor = validFloors[Mathf.RoundToInt(Random.Range(0, validFloors.Count))].transform.position;
        Vector3 randomOffset = new Vector3(Random.Range(spawnRadius * -1, spawnRadius), 0, Random.Range(spawnRadius * -1, spawnRadius));

        return randomFloor + randomOffset;
    }

    void OnEnable()
    {
        EventManager.distributeItems += DistributeCoins;
    }

    void OnDisable()
    {
        EventManager.distributeItems -= DistributeCoins;
    }
}
