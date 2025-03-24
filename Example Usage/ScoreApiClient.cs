using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Model that represents a high score.
/// </summary>
[Serializable]
public class HighScoreModel
{
    public string Name;
    public int Score;
}

/// <summary>
/// A Unity web client for interacting with the Score API.
/// </summary>
public class ScoreApiClient : MonoBehaviour
{
    /// <summary>
    /// Base URL of the Score API.
    /// </summary>
    public string baseApiUrl = "http://139.99.155.110:5000/api/score";

    /// <summary>
    /// Makes a GET request to retrieve the top scores.
    /// </summary>
    /// <param name="onSuccess">Callback with the list of high score models.</param>
    /// <param name="onError">Callback with error message.</param>
    public IEnumerator GetTopScores(Action<List<HighScoreModel>> onSuccess, Action<string> onError)
    {
        string endpoint = $"{baseApiUrl}";
        using (UnityWebRequest request = UnityWebRequest.Get(endpoint))
        {
            yield return request.SendWebRequest();

            if (HasRequestError(request))
            {
                onError?.Invoke(request.error);
            }
            else
            {
                // Deserialize JSON response into a list of scores.
                try
                {
                    // Unity's JsonUtility does not support top-level arrays, hence we wrap it.
                    string jsonResponse = request.downloadHandler.text;
                    // Wrap the array for deserialization.
                    string wrappedJson = "{\"scores\":" + jsonResponse + "}";
                    ScoresWrapper wrapper = JsonUtility.FromJson<ScoresWrapper>(wrappedJson);
                    onSuccess?.Invoke(wrapper.scores);
                }
                catch (Exception ex)
                {
                    onError?.Invoke("Error parsing scores: " + ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Makes a POST request to add a new score.
    /// </summary>
    /// <param name="newScore">The new high score model to add.</param>
    /// <param name="onSuccess">Callback on successful save.</param>
    /// <param name="onError">Callback with error message if any.</param>
    public IEnumerator AddScore(HighScoreModel newScore, Action onSuccess, Action<string> onError)
    {
        string endpoint = $"{baseApiUrl}";
        string jsonData = JsonUtility.ToJson(newScore);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (HasRequestError(request))
        {
            onError?.Invoke(request.error);
        }
        else
        {
            onSuccess?.Invoke();
        }
    }

    /// <summary>
    /// Checks for errors in the UnityWebRequest.
    /// </summary>
    /// <param name="request">The web request to evaluate.</param>
    /// <returns>True if there's an error, otherwise false.</returns>
    private bool HasRequestError(UnityWebRequest request)
    {
#if UNITY_2020_1_OR_NEWER
        return request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError;
#else
        return request.isNetworkError || request.isHttpError;
#endif
    }

    /// <summary>
    /// Helper wrapper class since Unity's JsonUtility cannot deserialize top-level arrays.
    /// </summary>
    [Serializable]
    private class ScoresWrapper
    {
        public List<HighScoreModel> scores;
    }
}