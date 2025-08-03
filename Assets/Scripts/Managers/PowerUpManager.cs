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

    [Header("Freeze Elements")]
    [SerializeField] private Freeze freeze;

    [Header("Spring Elements")] 
    [SerializeField] private Spring spring;
    

    [Header("Settings")]
     private bool isBusy; 
     private int vacuumItemsToCollect;
     private int vacuumCounter;
    //  private bool isUsingPowerup;


     [Header("Actions")]
     public static Action<Item> itemPickedUp;
     public static Action<Item> itemBackToGame;

     [Header("Data")]
     [SerializeField] private int initialVacuumPUCount;
    
    // [SerializeField] private Vacuum vacuumPowerup;
      private int vacuumPUCount;
      private int freezePUCount;
      private int springPUCount;


    private void Awake()
    {
        LoadData();
        
        Vacuum.started += OnVacuumStarted;
        Freeze.started += OnFreezeStarted;
        InputManager.powerupClicked += OnPowerupClicked;
        Spring.started += OnSpringStarted;
    }

    private void OnDestroy()
    {
        Vacuum.started -= OnVacuumStarted;
        Freeze.started -= OnFreezeStarted;
        InputManager.powerupClicked -= OnPowerupClicked;
        Spring.started -= OnSpringStarted;

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
            
            case EPowerupType.Freeze:
                HandleFreezeClicked(); 
                UpdateFreezeVisuals();
                break;
            
            case EPowerupType.Spring:
                HandleSpringClicked();
                UpdateSpringVisuals();
                break;
            
        }
    }

  

    private void HandleSpringClicked()
    {
        if (springPUCount <= 0)
        {

            springPUCount = 3;
            SaveData();
        }
        else
        {
            isBusy = true;

            springPUCount--;
            SaveData();
            spring.Play();
            StartCoroutine(ResetBusyAfterDelay());
        }
    }

    [Button]
    public void SpringPowerup()
    {
        ItemSpot spot = ItemSpotsManager.Instance.GetRandomOccupiedSpot();
        if(spot == null)
            return;
        
        isBusy = true;

        Item itemToRelease = spot.Item;
        
        spot.Clear();
        itemToRelease.UnassignSpot();
        itemToRelease.EnablePhysics();
        itemToRelease.transform.parent = LevelManager.Instance.itemParent;
        itemToRelease.transform.localPosition = Vector3.up * 3;
        itemToRelease.transform.localScale = Vector3.one;
        
        itemBackToGame?.Invoke(itemToRelease);
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
            StartCoroutine(ResetBusyAfterDelay());
        }
        
    }

    private void OnSpringStarted()
    {
        SpringPowerup();
    }
    
    private void OnVacuumStarted()
    {
        VacuumPowerup();
    }

    private void OnFreezeStarted()
    {
        FreezeGunPowerUp();
    }
    private IEnumerator ResetBusyAfterDelay()
    {
        yield return new WaitForSeconds(1f); 
        isBusy = false;
    }
    
    [Button]
    private void VacuumPowerup()
    {

        Item[] items = LevelManager.Instance.Items;
        ItemLevelData[] goals = GoalManager.instance.Goals;

        ItemLevelData? greatesGoal = GetGreatesGoal(goals);
        if (greatesGoal == null)
        {
            isBusy = false;
            return;
        }

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
        
        if (vacuumItemsToCollect == 0)
        {
            isBusy = false;
            return;
        }
        
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
            
            LeanTween.moveSpline(itemsToCollect[i].gameObject, points.ToArray(), 0.7f)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));


            /*
            LeanTween.move(itemsToCollect[i].gameObject, vacuumSuckPosition.position, 0.5f)
                .setEase(LeanTweenType.easeInCubic)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));
                */

            LeanTween.scale(itemsToCollect[i].gameObject, Vector3.zero, 0.7f);
            
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

    public void ResetPowerupState()
    {
        isBusy = false;
        vacuumCounter = 0;
        vacuumItemsToCollect = 0;
    }
    
    private void HandleFreezeClicked()
    {
        if (freezePUCount <= 0)
        {

            freezePUCount = 3;
            SaveData();
        }
        else
        {
            isBusy = true;

            freezePUCount--;
            SaveData();
            freeze.Play();
            StartCoroutine(ResetBusyAfterDelay());
        }
    }
    
   
    
    private void UpdateFreezeVisuals()
    {
       freeze.UpdateVisuals(freezePUCount);
    }
    
    private void UpdateSpringVisuals()
    {
        spring.UpdateVisuals(springPUCount);
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
    
    

    #region Freeze

    [Button]
    public void FreezeGunPowerUp()
    {
        TimerManager.instance.FreezeTimer();

    }

    #endregion
    
    
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
