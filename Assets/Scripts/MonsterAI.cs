using UnityEngine;

enum MapElement
{
    GrowingTreeElement,
    distanceToGoal
}

public class MonsterAI : MonoBehaviour
{
    [SerializeField] GameObject[,] openList, closedList, staticList;

    [SerializeField] GameObject playerElement, selfElement;
    private void Start()
    {
        
    }
    private void GetPath(GrowingTreeElement goal) 
    { 
        // Add MapElement with monsterPositionInMap and add to openMap
        // While openMap is not empty
            // Take from openList with lowest distanceToGoal
            // If Node Current = node goal we finish
            // Add connected neighbours to openList
            // move node current to closed list
    }

    private void GetPositionInMap()
    {

    }

    private void FindPlayer()
    {
        Transform playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        Debug.Log(Mathf.FloorToInt(playerTransform.position.x / 2) + " " + Mathf.FloorToInt(playerTransform.position.z));
        playerElement = staticList[Mathf.FloorToInt(playerTransform.position.x / 2), Mathf.FloorToInt(playerTransform.position.z)];
    }

    private void FindSelf()
    {
        selfElement = staticList[Mathf.FloorToInt(transform.position.x / 2), Mathf.FloorToInt(transform.position.z)];
    }

    private void GetWorld()
    {
        staticList = FindFirstObjectByType<WorldGenerationGrowingTreeImproved>().protoWorld;
    }

    private void OnEnable()
    {
        EventManager.startGame += GetWorld;
        EventManager.startGame += GetPositionInMap;
        EventManager.startGame += FindPlayer;
        EventManager.startGame += FindSelf;
    }

    private void OnDisable()
    {
        EventManager.startGame -= GetWorld;
        EventManager.startGame -= GetPositionInMap;
        EventManager.startGame -= FindPlayer;
        EventManager.startGame -= FindSelf;
    }
}
