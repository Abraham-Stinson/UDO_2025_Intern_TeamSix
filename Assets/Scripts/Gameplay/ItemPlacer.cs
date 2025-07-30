using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemPlacer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private List<ItemLevelData> itemDatas;

    [Header("Settings")]
    [SerializeField] private float itemLifetime = 5f;

    /*[Header("Settings")] 
    [SerializeField] private BoxCollider spawnZone;

    [SerializeField] private int seed;*/

    // bu sistem güzel çalışıyor ama bizim oyun için işe yarayacağını sanmıyorum düzenlememiz lazım ya da başka bir sistem yapmamız lazım 
    [SerializeField] private Transform[] spawnPoints;

    [Header("Data")]
    private Item[] items;

    [Header("Actions")]
    public static Action<Item> OnItemSpawned;

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

    public Item[] GetItems()
    {
        if (items == null)
        {
            items = GetComponentsInChildren<Item>();
        }
        return items;
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

        foreach (Transform spawnPoint in spawnPoints)
        {
            // Her spawn point için farklı random item seç
            ItemLevelData randomData = itemDatas[Random.Range(0, itemDatas.Count)];

            // Item'ı oluştur
            Item itemInstance = Instantiate(randomData.itemPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            itemInstance.transform.forward = spawnPoint.forward;

            // Event'i tetikle
            OnItemSpawned?.Invoke(itemInstance);

            // Belirli süre sonra yok et (sadece seçilmemiş item'lar için)
            StartCoroutine(DestroyItemIfNotSelected(itemInstance, itemLifetime));
        }
    }

    private IEnumerator DestroyItemIfNotSelected(Item item, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Sadece seçilmemiş item'ları yok et
        if (item != null && !item.IsSelected)
        {
            Destroy(item.gameObject);
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
    }

#endif*/

/*private Vector3 GetSpawnPosition()
{
    float x = Random.Range(-spawnZone.size.x / 2, spawnZone.size.x / 2);
    float y = Random.Range(-spawnZone.size.y / 2, spawnZone.size.y / 2);
    float z = Random.Range(-spawnZone.size.z / 2, spawnZone.size.z / 2);

    Vector3 localPosition = spawnZone.center + new Vector3(x, y, z);
    Vector3 spawnPosition = transform.TransformPoint(localPosition);

    return spawnPosition;
}*/