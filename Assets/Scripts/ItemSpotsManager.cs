using System;
using UnityEngine;


public class ItemSpotsManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform itemSpot;

    [Header("Settings")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScalenOnSpot;
    
    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;
    }

    private void OnDestroy()
    {
        InputManager.itemClicked -= OnItemClicked;
    }

    private void OnItemClicked(Item item)
    {
        item.transform.SetParent(itemSpot);

        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScalenOnSpot;

        item.DisableShadows();
        item.DisablePhysics();
    }
}
