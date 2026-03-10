using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSubmitter : MonoBehaviour
{
    private string phpUrl = "https://joooooop.com/upload-score.php"; // Make sure this URL is accessible

    public void SubmitScore(string username, string role, int score)
    {
        StartCoroutine(SendScoreToDB(username, role, score));
    }

    private IEnumerator SendScoreToDB(string username, string role, int score)
    {
        Debug.Log("Starting score submission...");

        // Create form data
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("role", role);
        form.AddField("score", score);

        Debug.Log($"Sending to: {phpUrl}");
        Debug.Log($"Data: username={username}, role={role}, score={score}");

        // Send POST request
        using (UnityWebRequest request = UnityWebRequest.Post(phpUrl, form))
        {
            request.timeout = 10; // 10 second timeout
            yield return request.SendWebRequest();

            Debug.Log($"Request complete. Result: {request.result}");
            Debug.Log($"Response Code: {request.responseCode}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Success! Server response: " + response);
            }
            else if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Connection Error: " + request.error);
                Debug.LogError("Check if the URL is correct and the server is reachable");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Protocol Error: " + request.error);
                Debug.LogError("Server responded with error code: " + request.responseCode);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            else if (request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Data Processing Error: " + request.error);
            }
        }
    }
}