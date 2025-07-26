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
    private Dictionary<EItemName, ItemMergeData> itemMergeDataDictionary = new Dictionary<EItemName, ItemMergeData>();
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType animationEasing;

    [Header("Actions")] 
    public static Action<List<Item>> mergeStarted;
    public static Action<Item> itemPickedUp;
    
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
        
        itemPickedUp?.Invoke(item);
        
        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        if (itemMergeDataDictionary.ContainsKey(item.ItemName))
            HandleItemMergeDataFound(item);
        else
         MoveItemToFirstFreeSpot(item);
    }

    public void HandleItemMergeDataFound(Item item)
    {
        ItemSpot idealSpot = GetIdealSpotFor(item);

        itemMergeDataDictionary[item.ItemName].Add(item);

        TryMoveToItemIdealSpot(item, idealSpot);
    }

    private ItemSpot GetIdealSpotFor(Item item)
    {
        List<Item> items = itemMergeDataDictionary[item.ItemName].items;
        List<ItemSpot> itemSpots = new List<ItemSpot>();

        for (int i = 0; i < items.Count; i++)
            itemSpots.Add(items[i].Spot);

        if (items.Count >= 2)
            itemSpots.Sort((a,b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex())); //burayı tam anlamadım tekrar bakıcam
        
        int idealSpotIndex = itemSpots[0].transform.GetSiblingIndex() + 1;

        return spots[idealSpotIndex];



    }
    private void TryMoveToItemIdealSpot(Item item, ItemSpot targetSpot)
    {
        if (!targetSpot.IsEmpty())
        {
            HandleIdealSpotFull(item, targetSpot);
            return;
        }

        MoveItemToSpot(item, targetSpot, () => HandleItemReachedSpot(item));
    }

    private void MoveItemToSpot(Item item,ItemSpot idealSpot,Action completeCallback)
    {
        idealSpot.Populate(item);
        // item.transform.localPosition = itemLocalPositionOnSpot;
        // item.transform.localScale = itemLocalScalenOnSpot;
        // item.transform.localRotation=quaternion.identity;

       
        LeanTween.moveLocal(item.gameObject, itemLocalPositionOnSpot, .15f)
            .setEase(animationEasing);
        LeanTween.scale(item.gameObject, itemLocalScalenOnSpot, animationDuration)
            .setEase(animationEasing);;
        LeanTween.rotateLocal(item.gameObject, Vector3.zero, animationDuration)
            .setOnComplete(completeCallback);
        
        
        item.DisableShadows();
        item.DisablePhysics();
        
        
       
        //HandleItemReachedSpot(item,checkForMerge);
    }

    private void HandleItemReachedSpot(Item item,bool checkForMerge=true)
    {
        item.Spot.BumpDown();
        
        if (!checkForMerge)
            return;
        
        if (itemMergeDataDictionary[item.ItemName].CanMergeItems())
        {
            MergeItems(itemMergeDataDictionary[item.ItemName]);
        }
        else
        {
            CheckForGameOver();
        }
        


    }

    private void MergeItems(ItemMergeData itemMergeData)
    {
        List<Item> items = itemMergeData.items;
        itemMergeDataDictionary.Remove(itemMergeData.itemName);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Spot.Clear();
            
        }
        //remove this line after moving the items to the left
        //isBusy = false;
        
        if (itemMergeDataDictionary.Count<=0)
        {
            isBusy = false;
        }
        else
        {
            MoveAllItemsToTheLeft(HandleAllItemsMovedToTheLeft);
        }
        
        mergeStarted?.Invoke(items);
        
    }

    private void MoveAllItemsToTheLeft(Action completeCallback)
    {
        bool callbackTriggered = false;
 
        for (int i = 3; i < spots.Length; i++)
        {
            ItemSpot spot = spots[i];
 
            if (spot.IsEmpty())
                continue;
 
            Item item = spot.Item;
            ItemSpot targetSpot = spots[i - 3];
 
            if(!targetSpot.IsEmpty())
            {
                Debug.LogWarning($"{targetSpot.name} is full");
                isBusy = false;
                return;
            }
 
            spot.Clear();
 
            completeCallback += () => HandleItemReachedSpot(item, false);
            MoveItemToSpot(item, targetSpot, completeCallback);
 
            callbackTriggered = true;
        }
 
        if (!callbackTriggered)
        {
            completeCallback?.Invoke();
        }
    } 

    private void HandleAllItemsMovedToTheLeft()
    {
        isBusy = false;
    }


    private void HandleIdealSpotFull(Item item, ItemSpot idealSpot)
    {
        MoveAllItemsToTheRightFrom(idealSpot, item);
    }

    private void MoveAllItemsToTheRightFrom(ItemSpot idealSpot, Item itemToPlace)
    {
        int spotIndex = idealSpot.transform.GetSiblingIndex();

        for (int i = spots.Length - 2; i >= spotIndex; i--)
        {
            ItemSpot spot = spots[i];
            if (spot.IsEmpty())
            {
                continue;
            }
            
            if (spots[i].IsEmpty())
            {
                continue;
            }
            
            Item item = spots[i].Item;
            
            spot.Clear();

            ItemSpot targetSpot = spots[i + 1];

            if (!targetSpot.IsEmpty())
            {
                Debug.LogError("Error");
                isBusy = false;
                return;
            }
            
            MoveItemToSpot(item, targetSpot, () => HandleItemReachedSpot(item, false));
        }
        
        MoveItemToSpot(itemToPlace, idealSpot, () => HandleItemReachedSpot(itemToPlace));
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetSpot = GetFreeSpot();

        if (targetSpot == null)
        {
            Debug.LogError("Targetspot is null");
            return;
        }

        CreateItemMergeData(item);
        
        MoveItemToSpot(item,targetSpot, () => HandleFirstItemReachedSpot(item));
        
        /*
        targetspot.Populate(item);
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScalenOnSpot;
        item.transform.localRotation=quaternion.identity;

        item.DisableShadows();
        item.DisablePhysics();

        HandleFirstItemReachedSpot(item);
        */
    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        item.Spot.BumpDown();
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (GetFreeSpot() == null)
            GameManager.instance.SetGameState(EGameState.GAMEOVER);
            //Debug.LogWarning("Game Over");
        else
            isBusy = false;
    }
    private void CreateItemMergeData(Item item)
    {
        itemMergeDataDictionary.Add(item.ItemName,new ItemMergeData(item));
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
