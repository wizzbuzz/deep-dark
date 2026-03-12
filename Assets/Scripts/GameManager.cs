using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Global singleton access point for GameManager
    public static GameManager Instance {get; private set;}
    // Maps player index to their assigned role ("Player" or "Monster")
    public Dictionary<int, string> playerRoles = new Dictionary<int, string>();
    // Prefabs/objects for the player, monster, and exit ladder
    public GameObject playerObject, monsterObject, ladderObject;
    // Percentage chance that player 1 gets the Player role (vs Monster)
    public int rChance = 50;
    // Skips role randomization and monster setup when true
    public bool singlePlayer = false;
    
    // Enforce a single instance, destroying any duplicate
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Randomly assigns Player and Monster roles to the two controllers, or sets up solo play
    void RandomizeRoles()
    {
        if (!singlePlayer)
        {
            bool randomChance = Random.Range(0, 100) > rChance;

            if (randomChance)
            {
                playerObject.GetComponent<PlayerMovement>().playerNumber = 1;
                playerRoles[0] = "Player";
                monsterObject.GetComponent<PlayerMovement>().playerNumber = 2;
                playerRoles[1] = "Monster";
            } else
            {
                playerObject.GetComponent<PlayerMovement>().playerNumber = 2;
                playerRoles[1] = "Monster";
                monsterObject.GetComponent<PlayerMovement>().playerNumber = 1;
                playerRoles[0] = "Player";
            }

            EventManager.rolesRandomized.Invoke();
        } else
        {
            playerObject.GetComponent<PlayerMovement>().playerNumber = 1;
            playerRoles[0] = "Player";
            EventManager.rolesRandomized.Invoke();
        }
    }

    // Places the player at the lowest room, the ladder at the highest, and the monster at a random room
    void SpawnObjects()
    {
        float lowestPos = math.INFINITY;
        GameObject lowestPosObject = null;
        float highestPos = 0;
        GameObject highestPosObject = null;

        GameObject randomSpawn = null;

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Decided"))
        {
            float pos = g.transform.position.x + g.transform.position.y;
            if(Random.Range(0, 100) < 50)
            {
                randomSpawn = g;
            }

            if(pos < lowestPos)
            {
                lowestPos = pos;
                lowestPosObject = g;
            }

            if(pos > highestPos)
            {
                highestPos = pos;
                highestPosObject = g;
            }
        }

        playerObject.transform.position = lowestPosObject.transform.position + (Vector3.up * 2);
        ladderObject.transform.position = highestPosObject.transform.position;
        monsterObject.transform.position = randomSpawn.transform.position + (Vector3.up * 0);
        EventManager.playersSpawned.Invoke();
    }

    // Return to the main menu scene
    void QuitGame() { 
        SceneManager.LoadScene(0);
    }

    // Subscribe to role, spawn, and stop events when this object becomes active
    void OnEnable()
    {
        EventManager.randomizeRoles += RandomizeRoles;
        EventManager.spawnPlayers += SpawnObjects;
        EventManager.stopGame += QuitGame;
    }

    // Unsubscribe from role, spawn, and stop events when this object is deactivated
    void OnDisable()
    {
        EventManager.randomizeRoles -= RandomizeRoles;
        EventManager.spawnPlayers -= SpawnObjects;
        EventManager.stopGame -= QuitGame;
    }
}
