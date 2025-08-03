using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class BeltSpawnConfig
{
    public string name;
    public Transform[] spawnPoints;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;
    public float materialSpeed;
    [HideInInspector] public float yOffset;
    public MeshRenderer[] meshRenderers;
    [HideInInspector] public float currentSpawnTime = 0f;
    [HideInInspector] public float nextSpawnTime = 0f;
}

public class ItemPlacer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private List<ItemLevelData> itemDatas;

    [Header("Settings")]
    [SerializeField] private float itemLifetime = 5f;
    

    [Header("Belt Configurations")]
    [SerializeField] private List<BeltSpawnConfig> beltConfigs = new();

    [Header("Data")]
    private Item[] items;

    [Header("Actions")]
    public static Action<Item> OnItemSpawned;

    
    [Header("Spawn Odds")]
    [SerializeField, Min(1)] private int goalItemWeightMultiplier = 2;
    
    [Header("Spawn Offset")]
    [SerializeField] private float maxHorizontalOffset = 0.1f;
    [SerializeField] private float maxDepthOffset = 0.1f;
    [SerializeField] private float maxYRotationOffset = 90f;
    
    public ItemLevelData[] GetGoals()
    {
        List<ItemLevelData> goals = new List<ItemLevelData>();
        foreach (ItemLevelData data in itemDatas)
        {
            if (data.isGoal)
                goals.Add(data);
        }
        return goals.ToArray();
    }

    public Item[] GetItems()
    {
        if (items == null)
            items = GetComponentsInChildren<Item>();
        return items;
    }

    void Start()
    {
        foreach (var belt in beltConfigs)
        {
            belt.currentSpawnTime = 0f;
            belt.nextSpawnTime = Random.Range(belt.minSpawnTime, belt.maxSpawnTime);
        }

        
    }

    void Update()
    {
        for (int i = 0; i < beltConfigs.Count; i++)
        {
            var belt = beltConfigs[i];
            belt.currentSpawnTime += Time.deltaTime;

            if (belt.currentSpawnTime >= belt.nextSpawnTime)
            {
                SpawnObjectsForBelt(belt);
                belt.currentSpawnTime = 0f;
                belt.nextSpawnTime = Random.Range(belt.minSpawnTime, belt.maxSpawnTime);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var belt in beltConfigs)
        {
            belt.yOffset -= Time.fixedDeltaTime * belt.materialSpeed;

            foreach (var renderer in belt.meshRenderers)
            {
                if (renderer != null)
                {
                    renderer.material.mainTextureOffset = new Vector2(0, belt.yOffset);
                }
            }
        }
    }

    private void SpawnObjectsForBelt(BeltSpawnConfig belt)
    {
        List<ItemLevelData> weightedList = new List<ItemLevelData>();

        foreach (var data in itemDatas)
        {
            int weight = data.isGoal ? goalItemWeightMultiplier : 1;
            for (int i = 0; i < weight; i++)
                weightedList.Add(data);
        }

        foreach (Transform spawnPoint in belt.spawnPoints)
        {
            ItemLevelData randomData = weightedList[Random.Range(0, weightedList.Count)];

          
            Vector3 offset = new Vector3(
                Random.Range(-maxHorizontalOffset, maxHorizontalOffset),
                0f,
                Random.Range(-maxDepthOffset, maxDepthOffset)
            );

            // Rastgele ufak Y rotasyonu (örneğin ±3 derece)
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(-10, 45), 0f);

  
            Vector3 spawnPosition = spawnPoint.position + offset;
            Quaternion spawnRotation = spawnPoint.rotation * randomRotation;

            Item itemInstance = Instantiate(randomData.itemPrefab, spawnPosition, spawnRotation, transform);

            OnItemSpawned?.Invoke(itemInstance);
            StartCoroutine(DestroyItemIfNotSelected(itemInstance, itemLifetime));
        }
    }

    private IEnumerator DestroyItemIfNotSelected(Item item, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (item != null && !item.IsSelected)
        {
            Destroy(item.gameObject);
        }
    }
}
