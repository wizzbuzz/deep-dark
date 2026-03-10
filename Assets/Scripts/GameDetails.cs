using System.Collections.Generic;
using UnityEngine;

public class GameDetails : MonoBehaviour
{
    public static GameDetails Instance { get; private set; }
    public int worldWidth = 20, worldHeight = 20, totalPoints = 200;
    public enum player { Monster, Human, Undecided}
    public player winner = player.Undecided;

    public void SetWinner(player p)
    {
        winner = p;
    }

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
