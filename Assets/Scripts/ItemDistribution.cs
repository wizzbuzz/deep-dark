using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDistribution : MonoBehaviour
{
    // Fraction of total points allocated to high-value items
    [SerializeField]
    private float highDistribution = .2f;

    // Fraction of total points allocated to mid-value items
    [SerializeField]
    private float midDistribution = .3f;

    // Prefabs for each coin tier
    [SerializeField]
    private GameObject lowPrefab, midPrefab, highPrefab;
    // Prefab and count for decorative stalactites
    [SerializeField]
    private GameObject stalactitePrefab;
    [SerializeField] private int stalactiteAmount = 200;

    // Tracks all spawned objects in the scene
    [SerializeField]
    private List<GameObject> placedObjects = new List<GameObject>();

    // Max offset from a floor tile center when placing an item
    private float spawnRadius = .8f;
    // Running total of point value for items spawned so far
    private int spawnedPoints = 0;
    // Spawns stalactites and all coin tiers across the level based on total points
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

    // Instantiates a stalactite slightly below a random floor position
    private void SpawnStalachtite()
    {
        GameObject tmp = Instantiate(stalactitePrefab);
        tmp.transform.position = GetRandomSpawn() - (Vector3.up * Random.Range(2.5f, 3f));
        tmp.transform.parent = gameObject.transform;
        placedObjects.Add(tmp);
    }

    // Places a single item at a random floor position and adds its value to the running total
    private void SpawnItem(GameObject item, int value)
    {
        GameObject tmp = Instantiate(item);
        tmp.transform.position = GetRandomSpawn();
        tmp.transform.rotation = Quaternion.Euler(-90, Random.Range(0, 360), 0);
        tmp.transform.parent = gameObject.transform;
        placedObjects.Add(tmp);
        spawnedPoints += value;
    }

    // Returns a random position near a floor tile that belongs to a decided room
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

    // Subscribe to the distribute-items event when this object becomes active
    void OnEnable()
    {
        EventManager.distributeItems += DistributeCoins;
    }

    // Unsubscribe from the distribute-items event when this object is deactivated
    void OnDisable()
    {
        EventManager.distributeItems -= DistributeCoins;
    }
}
