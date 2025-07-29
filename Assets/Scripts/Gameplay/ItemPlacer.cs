using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
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
        // Hedefleri tekrar etmeyecek şekilde al
        HashSet<ItemLevelData> uniqueGoals = new HashSet<ItemLevelData>();
        Debug.Log("Beni kaç kere çağırıyon amk");
        for (int i = 0; i < itemDatas.Count; i++)
        {
            uniqueGoals.Add(itemDatas[i]);
        }
        return uniqueGoals.ToArray();
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

        // Her spawn point için ayrı rastgele item seç
        foreach (Transform spawnPosition in spawnPoints)
        {
            // Rastgele bir item seç
            ItemLevelData randomData = itemDatas[Random.Range(0, itemDatas.Count)];
            Vector3 _spawnPosition = spawnPosition.position;

            Item itemInstance = Instantiate(randomData.itemPrefab, _spawnPosition, Quaternion.identity);
            itemInstance.transform.rotation = Quaternion.Euler(Vector3.zero); // Düz bir şekilde döndür
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