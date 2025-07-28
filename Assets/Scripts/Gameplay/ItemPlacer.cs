using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ItemPlacer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private List<ItemLevelData> itemDatas;

    /*[Header("Settings")] 
    [SerializeField] private BoxCollider spawnZone;

    [SerializeField] private int seed;*/

    // bu sistem güzel çalışıyor ama bizim oyun için işe yarayacağını sanmıyorum düzenlememiz lazım ya da başka bir sistem yapmamız lazım 
    [SerializeField] private Transform[] spawnPoints;

    public ItemLevelData[] GetGoals()
    {
        List<ItemLevelData> goals = new List<ItemLevelData>();

        foreach (ItemLevelData data in itemDatas)
        {
            if (data.isGoal)
            {
                goals.Add(data);
            }
        }
        return goals.ToArray();

    }

    [SerializeField] public float minSpawnTime;
    [SerializeField] public float maxSpawnTime;

    private float spawnTime;
    void Start()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        InvokeRepeating("SpawnObjects", minSpawnTime, spawnTime);
    }

    void SpawnObjects()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        ItemLevelData data = itemDatas[Random.Range(0, itemDatas.Count)];
        foreach (Transform spawnPosition in spawnPoints)
        {
            Vector3 _spawnPosition = new Vector3(spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z);

            Item itemInstance = Instantiate(data.itemPrefab, _spawnPosition, Quaternion.identity);
            itemInstance.transform.rotation = Quaternion.Euler(Random.onUnitSphere * 360);
        }

    }
}

/*#if UNITY_EDITOR

    [Button]
    private void GenerateItems()
    {
        while (transform.childCount > 0)
        {
            Transform t = transform.GetChild(0);
            t.SetParent(null);
                DestroyImmediate(t.gameObject);
        }
        
        //Random.InitState(seed);

        for (int i = 0; i < itemDatas.Count; i++)
        {
            ItemLevelData data = itemDatas[i];

            for (int j = 0; j < data.amount; j++)
            {
                Vector3 spawnPosition = GetSpawnPosition();

                Item itemInstance = PrefabUtility.InstantiatePrefab(data.itemPrefab,transform) as Item;
                itemInstance.transform.position = spawnPosition;
                itemInstance.transform.rotation=Quaternion.Euler(Random.onUnitSphere* 360);
            }
        }
        
            
        
    }*/

    /*private Vector3 GetSpawnPosition()
    {
        float x = Random.Range(-spawnZone.size.x / 2, spawnZone.size.x / 2);
        float y = Random.Range(-spawnZone.size.y / 2, spawnZone.size.y / 2);
        float z = Random.Range(-spawnZone.size.z / 2, spawnZone.size.z / 2);

        Vector3 localPosition = spawnZone.center + new Vector3(x, y, z);
        Vector3 spawnPosition = transform.TransformPoint(localPosition);

        return spawnPosition;

    }*/