using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject[] objectPrefab;
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

    }
}
