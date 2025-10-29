using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad;
    public int level;
    private GameData gameData;
    private int starsActive;
    private int highScore;

    [Header("UI Stuff")]
    public Image[] stars;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI starText;

    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadDataSafe();
        ActivateStarsSafe();
        SetTextSafe();
    }

    void LoadDataSafe()
    {
        starsActive = 0;
        highScore = 0;

        if (gameData == null || gameData.saveData == null || level <= 0)
            return;

        int idx = level - 1;

        var save = gameData.saveData;
        if (save.stars != null && idx >= 0 && idx < save.stars.Length)
            starsActive = Mathf.Clamp(save.stars[idx], 0, stars != null ? stars.Length : 3);

        if (save.highScores != null && idx >= 0 && idx < save.highScores.Length)
            highScore = Mathf.Max(0, save.highScores[idx]);
    }

    void ActivateStarsSafe()
    {
        if (stars == null) return;

        for (int i = 0; i < stars.Length; i++)
            if (stars[i] != null) stars[i].enabled = false;

        int count = Mathf.Min(starsActive, stars.Length);
        for (int i = 0; i < count; i++)
            if (stars[i] != null) stars[i].enabled = true;
    }

    void SetTextSafe()
    {
        if (highScoreText) highScoreText.text = highScore.ToString();
        if (starText) starText.text = $"{starsActive}/3";
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", Mathf.Max(0, level - 1));
        SceneManager.LoadScene(levelToLoad);
    }
}
