using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private enum GamePhase { Loading, Playing, GameOver }
    private GamePhase currentGamePhase = GamePhase.Loading;

    public static GameState Instance { get; private set; }

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

    void Start()
    {
        EventManager.generateWorld?.Invoke();
    }

    void OnWorldGenerated() { 
        Debug.Log("World Generated.");
        EventManager.removeTorches?.Invoke();
    }

    void OnTorchesRemoved()
    {
        Debug.Log("Torches Removed.");
        EventManager.distributeItems?.Invoke();
    }

    void OnItemsDistributed()
    {
        Debug.Log("Items Distributed.");
        EventManager.randomizeRoles?.Invoke();
    }

    void OnRolesRandomized()
    {
        Debug.Log("Roles Randomized.");
        EventManager.spawnPlayers?.Invoke();
    }

    void OnPlayersSpawned()
    {
        Debug.Log("Players Spawned.");
        EventManager.hideLoadingScreen?.Invoke();
        EventManager.loadingScreenHidden?.Invoke();
    }

    void OnLoadingScreenHidden()
    {
        Debug.Log("Loading Screen Hidden.");
        EventManager.showTutorial?.Invoke();
    }

    void OnTutorialEnded()
    {
        Debug.Log("Tutorial Ended.");
        EventManager.startGame?.Invoke();
    }

    void OnStartGame()
    {
        Debug.Log("Game Started.");
        currentGamePhase = GamePhase.Playing;
    }

    void OnGameOver()
    {
        Debug.Log("Showing End Screen.");
        EventManager.showEndScreen?.Invoke();
    }

    private void OnScoreSubmitted()
    {
        EventManager.stopGame?.Invoke();
    }

    void OnEnable()
    {
        EventManager.worldGenerated += OnWorldGenerated;
        EventManager.torchesRemoved += OnTorchesRemoved;
        EventManager.itemsDistributed += OnItemsDistributed;
        EventManager.rolesRandomized += OnRolesRandomized;
        EventManager.playersSpawned += OnPlayersSpawned;
        EventManager.loadingScreenHidden += OnLoadingScreenHidden;
        EventManager.tutorialEnded += OnTutorialEnded;
        EventManager.startGame += OnStartGame;
        EventManager.gameOver += OnGameOver;
        EventManager.scoreSubmitted += OnScoreSubmitted;
    }

    void OnDisable()
    {
        EventManager.worldGenerated -= OnWorldGenerated;
        EventManager.torchesRemoved -= OnTorchesRemoved;
        EventManager.itemsDistributed -= OnItemsDistributed;
        EventManager.rolesRandomized -= OnRolesRandomized;
        EventManager.playersSpawned -= OnPlayersSpawned;
        EventManager.loadingScreenHidden -= OnLoadingScreenHidden;
        EventManager.startGame -= OnStartGame;
        EventManager.tutorialEnded -= OnTutorialEnded;
        EventManager.gameOver -= OnGameOver;
        EventManager.scoreSubmitted -= OnScoreSubmitted;
    }
}
