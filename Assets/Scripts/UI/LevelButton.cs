using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{

    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    private int starsActive;

    [Header("Level UI")]
    public Image[] stars;
    public TextMeshProUGUI levelText;
    public int level;
    public GameObject confirmPanel;

    private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    void LoadData()
    {
        if (gameData == null || gameData.saveData == null || level <= 0)
            return;

        int idx = level - 1;

        var act = gameData.saveData.isActive;
        var st = gameData.saveData.stars;

        if (act == null || st == null) return;
        if (idx < 0 || idx >= act.Length || idx >= st.Length) return;

        isActive = act[idx];
        starsActive = Mathf.Max(0, st[idx]);
    }

    void ActivateStars()
    {
        if (stars == null) return;
        int n = Mathf.Min(starsActive, stars.Length);
        for (int i = 0; i < n; i++)
            if (stars[i] != null) stars[i].enabled = true;
    }


    void DecideSprite()
    {
        if (isActive)
        {
               buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel()
    {
        if (levelText != null) levelText.text = level.ToString();
    }



    public void ConfirmPanel(int level)
    {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}
