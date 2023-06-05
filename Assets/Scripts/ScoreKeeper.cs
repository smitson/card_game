using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    //TODO holding scores 
    private int currentScore;
    private int bestScore = 52;
    private int totalGames = 0;
    private int scoreRange1to5 = 0;
    private int scoreRange6to10 = 0;
    private int scoreRange11to15 = 0;
    private int scoreRange16to20 = 0;

    private const string BestScoreKey = "BestScore";
    private const string ScoreRangeKey1to5 = "ScoreRange1to5";
    private const string ScoreRangeKey6to10 = "ScoreRange6to10";
    private const string ScoreRangeKey11to15 = "ScoreRange11to15";
    private const string ScoreRangeKey16to20 = "ScoreRange16to20";
   
    private const string TotalGames = "TotalGames";

    public bool isGameOver;

    private void Start()
    {
        // Load best score and ranges

        if (PlayerPrefs.HasKey(BestScoreKey))
        {
            bestScore = PlayerPrefs.GetInt(BestScoreKey);
            scoreRange1to5 = PlayerPrefs.GetInt(ScoreRangeKey1to5);
            scoreRange6to10 = PlayerPrefs.GetInt(ScoreRangeKey6to10);
            scoreRange11to15 = PlayerPrefs.GetInt(ScoreRangeKey11to15);
            scoreRange16to20 = PlayerPrefs.GetInt(ScoreRangeKey16to20);
            totalGames = PlayerPrefs.GetInt(TotalGames);
        }

        isGameOver = false;
    }

    private void Update()
    {
        if (isGameOver)
        {
            EndGame();
        }

    }

    private void OnApplicationQuit()
    {
        EndGame();
    }

    public void EndGame()
    {

        totalGames++;
        PlayerPrefs.SetInt(TotalGames, totalGames);

        // Update best score if current score is higher
        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            // Save new best score
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            
        }

        // Update score ranges
                if (currentScore >= 1 && currentScore <= 5)
        {
            scoreRange1to5++;
        }
        else if (currentScore >= 6 && currentScore <= 10)
        {
            scoreRange6to10++;
        }
        else if (currentScore >= 11 && currentScore <= 15)
        {
            scoreRange11to15++;
        }
        else if (currentScore >= 16 && currentScore <= 20)
        {
            scoreRange16to20++;
        }

        // Save score ranges
        PlayerPrefs.SetInt(ScoreRangeKey1to5, scoreRange1to5);
        PlayerPrefs.SetInt(ScoreRangeKey6to10, scoreRange6to10);
        PlayerPrefs.SetInt(ScoreRangeKey11to15, scoreRange11to15);
        PlayerPrefs.SetInt(ScoreRangeKey16to20, scoreRange16to20);

        PlayerPrefs.Save();
    }
}
