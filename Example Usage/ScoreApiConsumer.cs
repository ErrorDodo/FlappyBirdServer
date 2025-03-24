using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic consumer script for interacting with the Score API using ScoreApiClient.
/// Attach this script to the same GameObject that has the ScoreApiClient component.
/// </summary>
public class ScoreApiConsumer : MonoBehaviour
{
    private ScoreApiClient apiClient;

    // Start is called before the first frame update
    void Start()
    {
        // Get the ScoreApiClient component from the same GameObject.
        apiClient = GetComponent<ScoreApiClient>();
        
        if (apiClient == null)
        {
            Debug.LogError("ScoreApiClient component not found on the GameObject.");
            return;
        }
        
        // Start by fetching the top scores.
        StartCoroutine(apiClient.GetTopScores(OnFetchSuccess, OnFetchError));
    }

    /// <summary>
    /// Callback for successful fetching of top scores.
    /// </summary>
    /// <param name="highScores">The list of retrieved high scores.</param>
    void OnFetchSuccess(List<HighScoreModel> highScores)
    {
        Debug.Log("Fetched top scores:");
        foreach (var score in highScores)
        {
            Debug.Log($"{score.Name}: {score.Score}");
        }
    }

    /// <summary>
    /// Callback for errors during fetching of top scores.
    /// </summary>
    /// <param name="error">The error message.</param>
    void OnFetchError(string error)
    {
        Debug.LogError("Error fetching top scores: " + error);
    }

    /// <summary>
    /// Submits a new score using the API client.
    /// </summary>
    /// <param name="name">Player's name.</param>
    /// <param name="score">Player's score.</param>
    public void SubmitScore(string name, int score)
    {
        HighScoreModel newScore = new HighScoreModel
        {
            Name = name,
            Score = score
        };

        StartCoroutine(apiClient.AddScore(newScore, OnSubmitSuccess, OnSubmitError));
    }

    /// <summary>
    /// Callback called upon successful score submission.
    /// </summary>
    void OnSubmitSuccess()
    {
        Debug.Log("Score submitted successfully.");
    }

    /// <summary>
    /// Callback called when there is an error submitting a score.
    /// </summary>
    /// <param name="error">Error message returned by the API.</param>
    void OnSubmitError(string error)
    {
        Debug.LogError("Error submitting score: " + error);
    }
}