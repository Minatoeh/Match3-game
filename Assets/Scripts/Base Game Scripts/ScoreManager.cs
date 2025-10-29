using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    private Board board;
    public TMPro.TextMeshProUGUI scoreText;
    public int score;
    public int[] scoreGoals;
    public int currentLevel = 1;
    public int[] stars;
    public Image scoreBar;
    private GameData gameData;
    private int numberStars;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        UpdateBar();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    /*public void IncreasesScore(int amountToIncrease)
    {
        score += amountToIncrease;
        for(int i = 0; i < board.scoreGoals.Length; i++)
        {
            if(score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }

            int currentStars = gameData.saveData.stars[board.level];
            if(numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
            }

            gameData.Save();
        }
        UpdateBar();
    }*/

    public void IncreasesScore(int amountToIncrease)
    {
        score = Mathf.Max(0, score + amountToIncrease);

        if (board != null && board.scoreGoals != null && board.scoreGoals.Length > 0)
        {
            for (int i = 0; i < board.scoreGoals.Length; i++)
            {
                if (score >= board.scoreGoals[i] && numberStars < i + 1)
                    numberStars = i + 1;
            }
        }
        else
        {
            Debug.LogWarning("ScoreManager: board.scoreGoals is null or empty.");
        }

        if (gameData != null && gameData.saveData != null && board != null)
        {
            int level = Mathf.Max(0, board.level);

            if (gameData.saveData.highScores != null && level < gameData.saveData.highScores.Length)
            {
                if (score > gameData.saveData.highScores[level])
                    gameData.saveData.highScores[level] = score;
            }
            else
            {
                Debug.LogWarning($"ScoreManager: highScores is null or too short for level {level}.");
            }

            if (gameData.saveData.stars != null && level < gameData.saveData.stars.Length)
            {
                if (numberStars > gameData.saveData.stars[level])
                    gameData.saveData.stars[level] = numberStars;
            }
            else
            {
                Debug.LogWarning($"ScoreManager: stars is null or too short for level {level}.");
            }

            gameData.Save();
        }

        if (scoreText != null) scoreText.text = score.ToString();
        UpdateBar();
    }




    private void UpdateBar()
    {
        if (board != null && scoreBar != null && board.scoreGoals != null && board.scoreGoals.Length > 0)
        {
            int last = board.scoreGoals[board.scoreGoals.Length - 1];
            if (last > 0)
                scoreBar.fillAmount = Mathf.Clamp01((float)score / last);
            else
                scoreBar.fillAmount = 0f;
        }
    }
}
