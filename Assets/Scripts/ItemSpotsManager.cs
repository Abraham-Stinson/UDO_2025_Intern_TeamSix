using System;
using Unity.Mathematics;
using UnityEngine;


public class ItemSpotsManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform itemSpotsParent;

    private ItemSpot[] spots;

    [Header("Settings")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScalenOnSpot;
    
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
        if (!IsFreePotAvailable())
        {
            Debug.LogWarning("no free spots avaiable!");
            return;
        }
        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        MoveItemToFirstFreeSpot(item);
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetspot = GetFreeSpot();

        if (targetspot == null)
        {
            Debug.LogError("Targetspot is null");
            return;
        }
        targetspot.Populate(item);
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScalenOnSpot;
        item.transform.localRotation=quaternion.identity;

        item.DisableShadows();
        item.DisablePhysics();
        
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
