using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour
{
    [SerializeField] private string splashSceneName = "Splash";

    public void WinOk()
    {
        var board = FindObjectOfType<Board>();
        var scoreMgr = FindObjectOfType<ScoreManager>();
        var gameData = FindObjectOfType<GameData>();

        int level = board != null ? Mathf.Max(0, board.level) : 0;
        int score = scoreMgr != null ? scoreMgr.score : 0;

        if (gameData != null && gameData.saveData != null)
        {
            var sd = gameData.saveData;

            if (sd.highScores != null && level < sd.highScores.Length)
                sd.highScores[level] = Mathf.Max(sd.highScores[level], score);

            if (sd.stars != null && level < sd.stars.Length && board != null && board.scoreGoals != null)
                sd.stars[level] = Mathf.Max(sd.stars[level], CalcStars(score, board.scoreGoals));

            gameData.Save();
        }

        int nextLevel = level + 1;
        if (board != null && board.world != null && board.world.levels != null && nextLevel < board.world.levels.Length)
        {
            PlayerPrefs.SetInt("Current Level", nextLevel);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(splashSceneName);
        }
    }

    private int CalcStars(int score, int[] goals)
    {
        if (goals == null || goals.Length == 0) return 0;
        int stars = 0;
        for (int i = 0; i < goals.Length; i++)
            if (score >= goals[i]) stars = i + 1;
        return stars;
    }
}
