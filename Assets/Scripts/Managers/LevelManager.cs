using System;
using UnityEngine;

public class LevelManager : MonoBehaviour,IGameStateListener
{
    [Header("Data")]
    [SerializeField] private Level[] levels;
    private const string levelKey = "LevelReached";
    private int levelIndex;

    [Header("Settings")] 
    private Level currentLevel;

    [Header(" Action ")]
    public static Action<Level> levelSpawned;

    private void Awake()
    {
    LoadData();
    }

    private void Start()
    {
       // SpawnLevel();
    }

    private void SpawnLevel()
    {
        transform.Clear();

        int validatedLevelIndex = levelIndex % levels.Length;
        currentLevel = Instantiate(levels[validatedLevelIndex], transform);
        
        levelSpawned?.Invoke(currentLevel);

    }

    private void LoadData()
    {
        levelIndex = PlayerPrefs.GetInt(levelKey);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(levelKey,levelIndex);
    }

    public void GameStateChangedCallBack(EGameState gameState)
    {
        if(gameState == EGameState.GAME)
            SpawnLevel();
        else if (gameState == EGameState.LEVELCOMPLETE)
        {
            levelIndex++;
            SaveData();
        }
    }
}
