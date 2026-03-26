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

        // Убеждаемся, что индекс уровня корректный (от 0 и выше)
        int level = board != null ? Mathf.Max(0, board.level) : 0;
        int score = scoreMgr != null ? scoreMgr.score : 0;

        if (gameData != null && gameData.saveData != null)
        {
            var sd = gameData.saveData;

            // 1. Сохраняем лучший результат для текущего уровня
            if (sd.highScores != null && level < sd.highScores.Length)
                sd.highScores[level] = Mathf.Max(sd.highScores[level], score);

            // 2. Считаем полученные звезды
            int starsEarned = 0;
            if (board != null && board.scoreGoals != null)
                starsEarned = CalcStars(score, board.scoreGoals);

            // 3. КЛЮЧЕВОЙ МОМЕНТ: открываем следующий уровень и сохраняем всё в файл
            // Этот метод внутри GameData делает: isActive[level + 1] = true
            gameData.MarkLevelCompleted(level, starsEarned);
        }

        // Переход на следующий уровень или в меню
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