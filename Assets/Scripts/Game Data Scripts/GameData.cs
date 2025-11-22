using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}
public class GameData : MonoBehaviour
{
    [Header("World Reference")]
    public World world;


    public static GameData gameData;
    public SaveData saveData;
    // Start is called before the first frame update
 void Awake()
{
    if (gameData != null && gameData != this)
    {
        Debug.LogWarning($"[GameData] Duplicate destroyed in scene: {gameObject.scene.name}");
        Destroy(gameObject);
        return;
    }
    gameData = this;
    DontDestroyOnLoad(gameObject);
    Debug.Log($"[GameData] Alive in DDOL from scene: {gameObject.scene.name}");
    Load();
    EnsureInitialized();
}


    void EnsureInitialized()
    {
        if (saveData == null) saveData = new SaveData();

        int levelsCount = (world != null && world.levels != null) ? world.levels.Length : 50; 

        if (saveData.isActive == null || saveData.isActive.Length != levelsCount)
            Array.Resize(ref saveData.isActive, levelsCount);

        if (saveData.highScores == null || saveData.highScores.Length != levelsCount)
            Array.Resize(ref saveData.highScores, levelsCount);

        if (saveData.stars == null || saveData.stars.Length != levelsCount)
            Array.Resize(ref saveData.stars, levelsCount);

        if (Array.TrueForAll(saveData.isActive, a => !a))
            saveData.isActive[0] = true;
    }

    private void Start()
    {
    }

    public void Save()
    {
        try
        {
            var path = Path.Combine(Application.persistentDataPath, "player.dat");
            using (var file = File.Open(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                formatter.Serialize(file, saveData);
#pragma warning restore SYSLIB0011
            }
            Debug.Log($"Saved: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e}");
        }
    }

    public void Load()
    {
        try
        {
            var path = Path.Combine(Application.persistentDataPath, "player.dat");
            if (!File.Exists(path)) return;

            using (var file = File.Open(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                saveData = formatter.Deserialize(file) as SaveData;
#pragma warning restore SYSLIB0011
            }
            Debug.Log($"Loaded: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e}");
            saveData = null; 
        }
    }


    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    public void MarkLevelCompleted(int levelIndex, int starsEarned)
    {
        if (saveData == null) return;
        if (levelIndex < 0 || levelIndex >= saveData.isActive.Length) return;

        saveData.stars[levelIndex] = Mathf.Max(saveData.stars[levelIndex], starsEarned);

        if (levelIndex + 1 < saveData.isActive.Length)
            saveData.isActive[levelIndex + 1] = true;

        Save();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
    