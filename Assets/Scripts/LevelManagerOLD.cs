using UnityEditor.Search;
using UnityEngine;


public class LevelManagerOLD : MonoBehaviour
{
    [Header("SpawnPoints")]
    [SerializeField] private Transform[] spawnPoints;
    [Header("Spawnable Objects")]
    [SerializeField] public GameObject[] spawnableObjects;

    [SerializeField] public float minSpawnTime;
    [SerializeField] public float maxSpawnTime;
    
    private GameObject spawnItems;
    private float spawnTime;
    void Start()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        InvokeRepeating("SpawnObjects", minSpawnTime, spawnTime);
    }

    void Update()
    {

    }
    void SpawnObjects()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        foreach (Transform spawnPosition in spawnPoints)
        {
            Vector3 _spawnPosition = new Vector3(spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z);
            int randomIndex = Random.Range(0, spawnableObjects.Length);
            spawnItems = Instantiate(spawnableObjects[randomIndex], _spawnPosition, Quaternion.identity);
        }
    }
    void OnDestroy()
    {
        
    }
}
