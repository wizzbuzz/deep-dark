using System.Collections.Generic;
using UnityEngine;

public class GameDetails : MonoBehaviour
{
    // Global singleton access point for GameDetails
    public static GameDetails Instance { get; private set; }
    // World dimensions and the total collectible point value to distribute
    public int worldWidth = 20, worldHeight = 20, totalPoints = 200;
    // Represents which type of player won, or if the result is not yet decided
    public enum player { Monster, Human, Undecided}
    // Stores the winning player type once the game ends
    public player winner = player.Undecided;

    // Records the winning player type
    public void SetWinner(player p)
    {
        winner = p;
    }

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
}
