using System;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class ItemSpotsManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform itemSpotsParent;

    private ItemSpot[] spots;

    [Header("Settings")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScalenOnSpot;
    private bool isBusy;
    
    [Header("Data")]
    private Dictionary<EItemName, ItemMergeData> itemMergeDataDictionnary = new Dictionary<EItemName, ItemMergeData>();
    
    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;

        StoreSports();
    }

    private void OnDestroy()
    {
        InputManager.itemClicked -= OnItemClicked;
    }

    private void OnItemClicked(Item item)
    {
        if (isBusy)
        {
         Debug.LogWarning("ItemSpotsManager is busy"); 
         return;
        }
        
        if (!IsFreePotAvailable())
        {
            Debug.LogWarning("no free spots avaiable!");
            return;
        }

        isBusy = true;
        
        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        if (itemMergeDataDictionnary.ContainsKey(item.ItemName))
            HandleItemMergeDataFound(item);
        else
         MoveItemToFirstFreeSpot(item);
    }

    public void HandleItemMergeDataFound(Item item)
    {
        ItemSpot idealSpot = GetIdealSpotFor(item);

        itemMergeDataDictionnary[item.ItemName].Add(item);

        TryMoveToItemIdealSpot(item, idealSpot);
    }

    private ItemSpot GetIdealSpotFor(Item item)
    {
        List<Item> items = itemMergeDataDictionnary[item.ItemName].items;
        List<ItemSpot> itemSpots = new List<ItemSpot>();

        for (int i = 0; i < items.Count; i++)
            itemSpots.Add(items[i].Spot);

        if (items.Count >= 2)
            itemSpots.Sort((a,b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex())); //burayı tam anlamadım tekrar bakıcam
        
        int idealSpotIndex = itemSpots[0].transform.GetSiblingIndex() + 1;

        return spots[idealSpotIndex];



    }
    private void TryMoveToItemIdealSpot(Item item, ItemSpot idealSpot)
    {
        if (!idealSpot.IsEmpty())
        {
            HandleIdealSpotFull(item, idealSpot);
        }

        MoveItemToSpot(item, idealSpot);
    }

    private void MoveItemToSpot(Item item,ItemSpot targetSpot)
    {
        targetSpot.Populate(item);
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScalenOnSpot;
        item.transform.localRotation=quaternion.identity;

        item.DisableShadows();
        item.DisablePhysics();

        HandleFirstItemReachedSpot(item);
        
        HandleItemReachedSpot(item);
    }

    private void HandleItemReachedSpot(Item item)
    {
        if (itemMergeDataDictionnary[item.ItemName].CanMergeItems())
            MergeItems(itemMergeDataDictionnary[item.ItemName]);
        else
        {
            CheckForGameOver();
        }


    }

    private void MergeItems(ItemMergeData itemMergeData)
    {
        List<Item> items = itemMergeData.items;
        itemMergeDataDictionnary.Remove(itemMergeData.itemName);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Spot.Clear();
            Destroy(items[i].gameObject);
        }

        //remove this line after moving the items to the left
        isBusy = false;
    }

    private void HandleIdealSpotFull(Item item, ItemSpot idealSpot)
    {
        throw new NotImplementedException();
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetspot = GetFreeSpot();

        if (targetspot == null)
        {
            Debug.LogError("Targetspot is null");
            return;
        }

        CreateItemMergeData(item);
        
        targetspot.Populate(item);
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScalenOnSpot;
        item.transform.localRotation=quaternion.identity;

        item.DisableShadows();
        item.DisablePhysics();

        HandleFirstItemReachedSpot(item);

    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (GetFreeSpot() == null)
            Debug.LogWarning("Game Over");
        else
            isBusy = false;
    }
    private void CreateItemMergeData(Item item)
    {
        itemMergeDataDictionnary.Add(item.ItemName,new ItemMergeData(item));
    }
    
    private void StoreSports()
    {
        spots = new ItemSpot[itemSpotsParent.childCount];

        for (int i = 0; i < itemSpotsParent.childCount; i++)
        {
            spots[i] = itemSpotsParent.GetChild(i).GetComponent<ItemSpot>();
        }
    }

    private ItemSpot GetFreeSpot()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
            {
                return spots[i];
            }
        }
        return null;

    }
    private bool IsFreePotAvailable()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return true;
        }

        return false;
    }
}
