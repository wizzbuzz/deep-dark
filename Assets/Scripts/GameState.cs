using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // Tracks the high-level phase the game is currently in
    private enum GamePhase { Loading, Playing, GameOver }
    // The active phase, starting in Loading until setup completes
    private GamePhase currentGamePhase = GamePhase.Loading;

    // Global singleton access point for GameState
    public static GameState Instance { get; private set; }

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

    // Kick off world generation when the scene starts
    void Start()
    {
        EventManager.generateWorld?.Invoke();
    }

    // Advance to torch removal once the world has finished generating
    void OnWorldGenerated() { 
        Debug.Log("World Generated.");
        EventManager.removeTorches?.Invoke();
    }

    // Advance to item distribution once torches have been removed
    void OnTorchesRemoved()
    {
        Debug.Log("Torches Removed.");
        EventManager.distributeItems?.Invoke();
    }

    // Advance to role randomization once items are placed
    void OnItemsDistributed()
    {
        Debug.Log("Items Distributed.");
        EventManager.randomizeRoles?.Invoke();
    }

    // Advance to player spawning once roles have been assigned
    void OnRolesRandomized()
    {
        Debug.Log("Roles Randomized.");
        EventManager.spawnPlayers?.Invoke();
    }

    // Hide the loading screen once all players are spawned
    void OnPlayersSpawned()
    {
        Debug.Log("Players Spawned.");
        EventManager.hideLoadingScreen?.Invoke();
        EventManager.loadingScreenHidden?.Invoke();
    }

    // Show the tutorial once the loading screen is fully hidden
    void OnLoadingScreenHidden()
    {
        Debug.Log("Loading Screen Hidden.");
        EventManager.showTutorial?.Invoke();
    }

    // Start the game once the player has finished the tutorial
    void OnTutorialEnded()
    {
        Debug.Log("Tutorial Ended.");
        EventManager.startGame?.Invoke();
    }

    // Transition the game phase to Playing
    void OnStartGame()
    {
        Debug.Log("Game Started.");
        currentGamePhase = GamePhase.Playing;
    }

    // Display the end screen when the game is over
    void OnGameOver()
    {
        Debug.Log("Showing End Screen.");
        EventManager.showEndScreen?.Invoke();
    }

    // Stop the game after the player submits their score
    private void OnScoreSubmitted()
    {
        EventManager.stopGame?.Invoke();
    }

    // Subscribe to all game-lifecycle events when this object becomes active
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

    // Unsubscribe from all game-lifecycle events when this object is deactivated
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
