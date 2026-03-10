using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;




public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public Dictionary<int, string> playerRoles = new Dictionary<int, string>();
    public GameObject playerObject, monsterObject, ladderObject;
    public int rChance = 50;
    public bool singlePlayer = false;
    
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

    void QuitGame() { 
        SceneManager.LoadScene(0);
    }

    void OnEnable()
    {
        EventManager.randomizeRoles += RandomizeRoles;
        EventManager.spawnPlayers += SpawnObjects;
        EventManager.stopGame += QuitGame;
    }

    void OnDisable()
    {
        EventManager.randomizeRoles -= RandomizeRoles;
        EventManager.spawnPlayers -= SpawnObjects;
        EventManager.stopGame -= QuitGame;
    }
}
