using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MatchData : MonoBehaviour
{
    // Global singleton access point for MatchData
    public static MatchData Instance { get; private set; }

    [Header("Match Data")]
    // Total coins the human has collected during the match
    [SerializeField] private int coinsCollected = 0;
    // Elapsed match time in seconds
    [SerializeField] private int matchDuration = 0;
    // Total distance the human has moved
    [SerializeField] private float distanceTraveledHuman = 0;
    // Total distance the beast has moved
    [SerializeField] private float distanceTraveledBeast = 0;
    // Number of seconds the human spent within proximity of the beast
    [SerializeField] private int timeNearBeast = 0;
    // Number of times the human successfully escaped a chase
    [SerializeField] private int escapesFromChase = 0;
    // Whether the human has reached the exit ladder
    [SerializeField] private bool exitReached = false;
    // Cumulative time the beast spent actively chasing
    [SerializeField] private float timeSpentChasing = 0;
    // Running average duration of each chase in seconds
    [SerializeField] private float averageChaseDuration = 0;

    [Header("Comparisons")]
    // Minimum match duration in seconds required to pass the integrity check
    [SerializeField] private int minimumDuration = 180;
    // Minimum distance each player must travel to pass the integrity check
    [SerializeField] private int minimumDistance = 0;
    // Minimum number of escapes required to pass the integrity check
    [SerializeField] private int minimumChases = 0;
    // Distance threshold at which the human is considered near the beast
    [Range(0f, 10f)]
    [SerializeField] private float beastHumanDistanceTrigger = 3;

    [Header("Bonuses")]
    // Score multiplier applied per successful near-miss escape
    [SerializeField] private const int nearMissBonusMultiplier = 2;

    [Header("Prefabs")]
    // Reference to the human player object
    [SerializeField] private GameObject human; 
    // Reference to the beast player object
    [SerializeField] private GameObject beast;

    [Header("End")]
    // Final calculated score shown at the end of the match
    public int endScore;

    // Previous frame positions used to calculate each player's travel distance
    private Vector3 oldHumanPosition = Vector3.zero;
    private Vector3 oldBeastPosition = Vector3.zero;

    // Tracks whether the human is currently within the beast's danger radius
    private bool nearBeast = false;

    // Measures the duration of each individual chase
    private Stopwatch chaseStopwatch;

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

    // Visualises the beast proximity trigger radius in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        Gizmos.DrawSphere(beast.transform.position, beastHumanDistanceTrigger);
    }

    // Computes the final score using multipliers for danger, chases, and activity
    public void CalculateEndScore()
    {
        Debug.Log("Calculating End Score...");
        int baseScore = coinsCollected;

        // Match Integrity Check
        bool integrity = true;
        if(matchDuration < minimumDuration) 
        {
            integrity = false;
        }

        if(distanceTraveledHuman < minimumDistance || distanceTraveledBeast < minimumDistance) 
        {
            integrity = false;
        }

        if(escapesFromChase < minimumChases) 
        {
            integrity = false;
        }

        Debug.Log(integrity);

        if(integrity) 
        {
            endScore = baseScore;
            return;
        }

        // DANGER MULTIPLIER
        float dangerRatio = (float)timeNearBeast / matchDuration;
        float dangerMultiplier = (float)(1.0 + Mathf.Clamp(dangerRatio, 0f, 1f)); // Cap the multiplier to prevent excessive scoring

        // CHASE MULTIPLIER
        float chaseMultiplier = (float)(1.0 + Mathf.Clamp(averageChaseDuration / 20, 0, 1.5f));

        // ACTIVITY MULTIPLIER
        float movementScore = (distanceTraveledBeast + distanceTraveledHuman) / 200;
        float activityMultiplier = (float)(1.0 + Mathf.Clamp(movementScore, 0, 1f));

        // BONUS SCORE
        float nearMissBonus = escapesFromChase * nearMissBonusMultiplier;

        UnityEngine.Debug.Log($"Base Score: {baseScore}, Danger Multiplier: {dangerMultiplier}, Chase Multiplier: {chaseMultiplier}, Activity Multiplier: {activityMultiplier}, Near Miss Bonus: {nearMissBonus}");

        endScore = (int)(baseScore * dangerMultiplier * chaseMultiplier * activityMultiplier + nearMissBonus);
    }

    // Increments the coin counter by the given amount
    void AddCollectedCoin(int amount)
    {
        coinsCollected += amount;
    }

    // Begins the per-second data-gathering coroutine
    void StartTimer()
    {
        StartCoroutine(GatherInformationEverySecond());
    }

    // Stops the per-second data-gathering coroutine
    void StopTimer()
    {
        StopCoroutine(GatherInformationEverySecond());
    }

    // Tracks match duration, player distances, and proximity to the beast every second
    IEnumerator GatherInformationEverySecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            matchDuration++;

            //Human Distance
            if (oldHumanPosition == Vector3.zero)
            {
                oldHumanPosition = human.transform.position;
            }
            else
            {
                distanceTraveledHuman += Vector3.Distance(oldHumanPosition, human.transform.position);
                oldHumanPosition = human.transform.position;
            }

            //Beast Distance
            if (oldBeastPosition == Vector3.zero)
            {
                oldBeastPosition = beast.transform.position;
            }
            else
            {
                distanceTraveledBeast += Vector3.Distance(oldBeastPosition, beast.transform.position);
            }

            //Distance between Human and Beast
            if(Vector3.Distance(human.transform.position, beast.transform.position) < beastHumanDistanceTrigger)
            {
                timeNearBeast++;
            }
        }
    }

    // Records a chase escape and updates the average chase duration
    void OnPlayerEscapedChase()
    {
        // Add one escape
        escapesFromChase++;

        // Add stopwatch time to total time spent chasing
        chaseStopwatch.Stop();
        timeSpentChasing += (float)chaseStopwatch.Elapsed.TotalSeconds;

        // calculate new average chase duration
        averageChaseDuration = (averageChaseDuration + (int)chaseStopwatch.Elapsed.TotalSeconds) / 2;
    }

    // Flags that the human has reached the exit ladder
    void OnPlayerReachedLadder()
    {
        exitReached = true;
    }

    // Starts the stopwatch when a new chase begins
    void OnCommenceChase()
    {
        chaseStopwatch = Stopwatch.StartNew();
        chaseStopwatch.Start();
    }

    // Subscribe to all match-relevant events when this object becomes active
    private void OnEnable()
    {
        EventManager.GivePoints += AddCollectedCoin;
        EventManager.startGame += StartTimer;
        EventManager.gameOver += StopTimer;
        EventManager.gameOver += CalculateEndScore;
        EventManager.playerEnteredChase += OnCommenceChase;
        EventManager.playerEscapedChase += OnPlayerEscapedChase;
        EventManager.playerReachedLadder += OnPlayerReachedLadder;
    }

    // Unsubscribe from all match-relevant events when this object is deactivated
    private void OnDisable()
    {
        EventManager.GivePoints -= AddCollectedCoin;
        EventManager.startGame -= StartTimer;
        EventManager.gameOver -= StopTimer;
        EventManager.gameOver -= CalculateEndScore;
        EventManager.playerEnteredChase -= OnCommenceChase;
        EventManager.playerEscapedChase -= OnPlayerEscapedChase;
        EventManager.playerReachedLadder -= OnPlayerReachedLadder;
    }
}
