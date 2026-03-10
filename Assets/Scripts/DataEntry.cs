using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class ScoreData
{
    public string username;
    public string role;
    public int score;
    public string date;
}

public class DataEntry : MonoBehaviour
{
    private int winnerScore = 0;
    private int winnerIdentifier = 0;
    private string winnerRole = "None";
    private string winnerName = "Player";
    [SerializeField]
    private string apiUrl = "https://dungeon-cleaner.vercel.app/api/db/enter";

    private void GetName(string name)
    {
        winnerName = name;
    }

    private void GatherData()
    {
        winnerRole = GameDetails.Instance.winner.ToString();
        GetComponent<MatchData>()?.CalculateEndScore();
        winnerScore = (int)(GetComponent<MatchData>()?.endScore);
        Debug.Log("Winnerscore " + winnerScore);

        Debug.Log("Winner: " + winnerName + " Role: " + winnerRole + " Score: " + winnerScore);

        EventManager.DataGathered?.Invoke();
    }

    public void UploadScore()
    {
        Debug.Log($"Attempting to upload score to: {apiUrl}");
        Debug.Log($"Winner: {winnerName}, Role: {winnerRole}, Score: {winnerScore}");
        StartCoroutine(SubmitScoreToLeaderboard());
    }

    private IEnumerator SubmitScoreToLeaderboard()
    {
        // Create JSON payload using a proper serializable class
        ScoreData scoreData = new ScoreData
        {
            username = winnerName,
            role = winnerRole.ToLower(),
            score = winnerScore,
            date = DateTime.Now.ToString("yyyy-MM-dd")
        };

        string json = JsonUtility.ToJson(scoreData);
        Debug.Log("Submitting JSON payload: " + json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending request to server...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Score submitted successfully!");
                Debug.Log("Server response: " + request.downloadHandler.text);
                EventManager.scoreSubmitted?.Invoke();
            }
            else
            {
                Debug.LogError("❌ Failed to submit score!");
                Debug.LogError($"Error Type: {request.result}");
                Debug.LogError($"Error Message: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");

                if (!string.IsNullOrEmpty(request.downloadHandler?.text))
                {
                    Debug.LogError($"Server response: {request.downloadHandler.text}");
                }

                if (request.responseCode == 0)
                {
                    Debug.LogError("⚠️ Response Code 0 means: Server not reachable!");
                    Debug.LogError("   → Check if your backend server is running on " + apiUrl);
                    Debug.LogError("   → If using WebGL, localhost won't work - use Standalone build or host the API online");
                }
                else if (request.responseCode == 400)
                {
                    Debug.LogError("⚠️ 400 Bad Request - Server rejected the data format!");
                    Debug.LogError($"   → JSON sent: {json}");
                    Debug.LogError("   → Check if server expects different field names or data types");
                }
            }
        }
    }

    void OnEnable()
    {
        EventManager.gameOver += GatherData;
        EventManager.NameChanged += GetName;
    }

    void OnDisable()
    {
        EventManager.gameOver -= GatherData;
        EventManager.NameChanged -= GetName;
    }
}
