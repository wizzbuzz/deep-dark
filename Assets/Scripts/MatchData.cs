using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MatchData : MonoBehaviour
{
    public static MatchData Instance { get; private set; }

    [Header("Match Data")]
    [SerializeField] private int coinsCollected = 0;
    [SerializeField] private int matchDuration = 0;
    [SerializeField] private float distanceTraveledHuman = 0;
    [SerializeField] private float distanceTraveledBeast = 0;
    [SerializeField] private int timeNearBeast = 0;
    [SerializeField] private int escapesFromChase = 0;
    [SerializeField] private bool exitReached = false;
    [SerializeField] private float timeSpentChasing = 0;
    [SerializeField] private float averageChaseDuration = 0;

    [Header("Comparisons")]
    [SerializeField] private int minimumDuration = 180; // Example minimum duration for comparison
    [SerializeField] private int minimumDistance = 0; // Example minimum distance for comparison
    [SerializeField] private int minimumChases = 0;
    [Range(0f, 10f)]
    [SerializeField] private float beastHumanDistanceTrigger = 3;

    [Header("Bonuses")]
    [SerializeField] private const int nearMissBonusMultiplier = 2;

    [Header("Prefabs")]
    [SerializeField] private GameObject human; 
    [SerializeField] private GameObject beast;

    [Header("End")]
    public int endScore;

    private Vector3 oldHumanPosition = Vector3.zero;
    private Vector3 oldBeastPosition = Vector3.zero;

    private bool nearBeast = false;

    private Stopwatch chaseStopwatch;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        Gizmos.DrawSphere(beast.transform.position, beastHumanDistanceTrigger);
    }

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

    void AddCollectedCoin(int amount)
    {
        coinsCollected += amount;
    }

    void StartTimer()
    {
        StartCoroutine(GatherInformationEverySecond());
    }

    void StopTimer()
    {
        StopCoroutine(GatherInformationEverySecond());
    }

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

    void OnPlayerReachedLadder()
    {
        exitReached = true;
    }

    void OnCommenceChase()
    {
        chaseStopwatch = Stopwatch.StartNew();
        chaseStopwatch.Start();
    }

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
