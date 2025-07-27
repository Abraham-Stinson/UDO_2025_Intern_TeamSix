using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;


[System.Serializable]
public class LevelData
{
    [Header("Section Settings")]
    public Sprite sectionIcon;
    public string sectionName;

    [Space(10)]
    [Header("Levels in This Section")]
    public Levels[] levels;
}

[System.Serializable]
public class Levels
{
    [Header("Level Configuration")]
    public string levelName; // optional

    [Space(5)]
    public GameObject levelPrefab;

    [Space(5)]
    [Header("Spawnable Objects")]
    public SpawnableObject[] spawnableObjects;

    [Header("Spawn Time")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    [Space(5)]
    [Header("Level Status")]
    public bool isLevelComplete;
    public bool isLevelUnlocked;
}

[System.Serializable]
public class SpawnableObject
{
    [Header("Object Settings")]
    public GameObject objectPrefab;
    [Space(5)]
    public bool isTargetObject;
    public int targetCount;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Level System Configuration")]
    [Space(10)]
    [SerializeField]
    public List<LevelData> levelData = new List<LevelData>();
    public List<Levels> allLevels = new List<Levels>();
    public List<SpawnableObject> currentSpawnableObjects = new List<SpawnableObject>();

    private GameObject currentLevelInstance;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadLevel(Levels level)
    {
        currentSpawnableObjects = new List<SpawnableObject>(level.spawnableObjects);
        currentLevelInstance = Instantiate(level.levelPrefab);//IMPORTANT
        var levelManager = currentLevelInstance.GetComponentInChildren<LevelManager>();
        levelManager.spawnableObjects = level.spawnableObjects.Select(obj => obj.objectPrefab).ToArray();
        levelManager.minSpawnTime = level.minSpawnTime;
        levelManager.maxSpawnTime = level.maxSpawnTime;

    }
    public void DestroyCurrentLevel()
    {
        Destroy(currentLevelInstance);
        currentLevelInstance = null;
    }

}
