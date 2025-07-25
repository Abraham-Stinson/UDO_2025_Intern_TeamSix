using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;


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

    [Space(5)]
    [Header("Level Status")]
    public bool isLevelComplete;
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
    private List<Levels> allLevels = new List<Levels>();
    private List<SpawnableObject> currentSpawnableObjects = new List<SpawnableObject>();


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
    }

    /*public GameObject[] objectPrefab;
    public Vector3[] spawnPosition;
    [Range(0f, 10f)] public float spawnTime = 3f;
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnTime);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        foreach (Vector3 spawnPosition in spawnPosition)
        {
            int randomIndex = Random.Range(0, objectPrefab.Length);
            Instantiate(objectPrefab[randomIndex], spawnPosition, Quaternion.identity);
        }

    }*/


}
