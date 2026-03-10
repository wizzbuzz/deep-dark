using UnityEngine;
using System;

public class EventManager
{
    public static Action generateWorld;
    public static Action worldGenerated;
    public static Action removeTorches;
    public static Action torchesRemoved;
    public static Action distributeItems;
    public static Action itemsDistributed;
    public static Action randomizeRoles;
    public static Action rolesRandomized;
    public static Action spawnPlayers;
    public static Action playersSpawned;
    public static Action hideLoadingScreen;
    public static Action loadingScreenHidden;
    public static Action showTutorial;
    public static Action tutorialEnded;
    public static Action startGame;
    public static Action showEndScreen;
    public static Action gameOver;
    public static Action scoreSubmitted;
    public static Action stopGame;

    public static Action<int> GivePoints;
    public static Action LevelConstructed;
    public static Action LevelLoaded;
    public static Action DataGathered;
    public static Action<string> NameChanged;
    public static Action playerEscapedChase;
    public static Action playerEnteredChase;
    public static Action playerReachedLadder;
}
