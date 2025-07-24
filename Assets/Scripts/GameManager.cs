using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject objectPrefab;
    public Vector3 spawnPosition;
    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject();
        }
    }
    
    void SpawnObject()
    {
        Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    }   
}
