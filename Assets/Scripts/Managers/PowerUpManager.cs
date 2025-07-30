using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager instance;

    [Header("Vacuum Elements")] 
    [SerializeField] private Vacuum vacuum;
     [SerializeField] private Transform vacuumSuckPosition;
    // [SerializeField] private TextMeshPro vacuumAmountText;
    // [SerializeField] private GameObject vacuumVideoIcon;
    //[SerializeField] private Animator vacuumAnimator;


    [Header("Settings")]
     private bool isBusy; 
     private int vacuumItemsToCollect;
     private int vacuumCounter;
    //  private bool isUsingPowerup;


     [Header("Actions")]
     public static Action<Item> itemPickedUp;

     [Header("Data")]
     [SerializeField] private int initialVacuumPUCount;
    // [SerializeField] private Vacuum vacuumPowerup;
      private int vacuumPUCount;


    private void Awake()
    {
        LoadData();
        
        Vacuum.started += OnVacuumStarted;
        InputManager.powerupClicked += OnPowerupClicked;
    }

    private void OnDestroy()
    {
        Vacuum.started -= OnVacuumStarted;
        InputManager.powerupClicked -= OnPowerupClicked;
    }

    private void OnPowerupClicked(Powerup powerup)
    {
        if (isBusy)
            return;

        switch (powerup.Type)
        {
            case EPowerupType.Vacuum:
                
                HandleVacuumClicked();
                UpdateVacuumVisuals();
                break;
        }
    }

    private void HandleVacuumClicked()
    {
        if (vacuumPUCount <= 0)
        {
            // burada reklam gösteririz şimdilik powerup sayısını resetliyor
            vacuumPUCount = 3;
            SaveData();
        }
        else
        {
            isBusy = true;

            vacuumPUCount--;
            SaveData();
            
            vacuum.Play();
        }
        
    }

    private void OnVacuumStarted()
    {
        VacuumPowerup();
    }


    [Button]
    private void VacuumPowerup()
    {

        Item[] items = LevelManager.Instance.Items;
        ItemLevelData[] goals = GoalManager.instance.Goals;

        ItemLevelData? greatesGoal = GetGreatesGoal(goals);
        if (greatesGoal == null)
            return;

        ItemLevelData goal = (ItemLevelData)greatesGoal;

       
        vacuumCounter = 0;
        
        List<Item> itemsToCollect = new List<Item>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                continue;
            }
            
            if (items[i].ItemName == goal.itemPrefab.ItemName)
            {
                itemsToCollect.Add(items[i]);
                
                if(itemsToCollect.Count >= 3)
                    break;

            }
        }

        vacuumItemsToCollect = itemsToCollect.Count;
        
        for (int i = 0; i < itemsToCollect.Count; i++)
        {
            itemsToCollect[i].DisablePhysics();

            Item itemToCollect = itemsToCollect[i];

            List<Vector3> points = new List<Vector3>();
            points.Add(itemsToCollect[i].transform.position);
           // points.Add(itemsToCollect[i].transform.position);
            //bilerek 2 kere yazdık
           points.Add(itemsToCollect[i].transform.position + Vector3.up );
           
            points.Add(vacuumSuckPosition.position + Vector3.up);
            points.Add(vacuumSuckPosition.position);
            
            LeanTween.moveSpline(itemsToCollect[i].gameObject, points.ToArray(), 0.5f)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));


            /*
            LeanTween.move(itemsToCollect[i].gameObject, vacuumSuckPosition.position, 0.5f)
                .setEase(LeanTweenType.easeInCubic)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));
                */

            LeanTween.scale(itemsToCollect[i].gameObject, Vector3.zero, 0.5f);
            
        }
        
        for (int i = itemsToCollect.Count-1; i >= 0 ; i--)
        {
            itemPickedUp?.Invoke(itemsToCollect[i]);
            //Destroy(itemsToCollect[i].gameObject);
        }

    }

    private void ItemReachedVacuum(Item item)
    {
        vacuumCounter++;

        if (vacuumCounter >= vacuumItemsToCollect)
            isBusy = false;
        
        Destroy(item.gameObject);
    }

    private ItemLevelData? GetGreatesGoal(ItemLevelData[] goals)
    {
        int max = 0;
        int goalIndex = -1;
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].amount >= max)
            {
                max = goals[i].amount;
                goalIndex = i;
            }
        }

        if (goalIndex <= -1)
            return null;


        return goals[goalIndex];
    }


    private void UpdateVacuumVisuals()
    {
        vacuum.UpdateVisuals(vacuumPUCount);
    }
    private void LoadData()
    {
        vacuumPUCount = PlayerPrefs.GetInt("VacuumCount", initialVacuumPUCount);

        UpdateVacuumVisuals();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("VacuumCount",vacuumPUCount);
    }

}
