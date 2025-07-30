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
    
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField]  private MeshRenderer meshRenderer;
    [SerializeField]  private float materialSpeed;
    private float yOffset;

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
        materialSpeed = 1f / spawnTime; 
        InvokeRepeating("SpawnObjects", minSpawnTime, spawnTime);
    }

    private void FixedUpdate()
    {
        yOffset -= Time.fixedDeltaTime*materialSpeed;
        meshRenderer.material.mainTextureOffset = new Vector2(0, yOffset);
    }

    void SpawnObjects()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);

        foreach (Transform spawnPoint in spawnPoints)
        {
           
            ItemLevelData randomData = itemDatas[Random.Range(0, itemDatas.Count)];
            
            Item itemInstance = Instantiate(randomData.itemPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            itemInstance.transform.forward = spawnPoint.forward;
            
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
