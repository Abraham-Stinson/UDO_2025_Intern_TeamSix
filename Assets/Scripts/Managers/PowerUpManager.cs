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
    [SerializeField] private Transform vacuumSuckPosition;
    [SerializeField] private TextMeshPro vacuumAmountText;
    [SerializeField] private GameObject vacuumVideoIcon;
    [SerializeField] private Animator vacuumAnimator;
    

    [Header("Settings")] 
    private bool isBusy;
    private int vacuumItemsToCollect;
    private int vacuumCounter;
    private bool isUsingPowerup;
    
    
    [Header("Actions")] 
    public static Action<Item> itemPickedUp;

    [Header("Data")] 
    [SerializeField] private int initialVacuumPUCount;
    
    [SerializeField] private Vacuum vacuumPowerup;
    private int vacuumPUCount;
    
       private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            LoadData();

            Vacuum.started += OnVacuumStarted;
            InputManager.powerupClicked += OnPowerupClicked;
        }

        private void OnDestroy()
        {
            Vacuum.started -= OnVacuumStarted;
            InputManager.powerupClicked -= OnPowerupClicked;
        }

        private void OnPowerupClicked(Powerup clickedPowerup)
        {
            if (isUsingPowerup)
                return;

            int amount = 0;

            switch (clickedPowerup.Type)
            {
                case EPowerupType.Vacuum:
                    VacuumPowerupPressed();
                    amount = vacuumPUCount;
                    break;
            }

            clickedPowerup.UpdateVisuals(amount);
        }

        private void OnVacuumStarted()
        {
            StartCoroutine(VacuumCoroutine());            
        }

        IEnumerator VacuumCoroutine()
        {
           
            Item[] items = LevelManager.Instance.Items;
            ItemLevelData[] goals = GoalManager.instance.Goals;

          
            ItemLevelData greatestGoal = GetGreatestGoal(goals);

         
            List<Item> itemsToCollect = new List<Item>();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].name == greatestGoal.itemPrefab.name)
                {
                    itemsToCollect.Add(items[i]);

                    if (itemsToCollect.Count >= 3)
                        break;
                }
            }

            Vector3 finalTargetPosition = vacuumSuckPosition.position;

            int collectedItems = 0;
            float timer = 0;
            float animationDuration = .7f;
            float delayBetweenItems = .3f;

            LTBezierPath[] itemPaths = new LTBezierPath[3];

          
            for (int i = 0; i < itemsToCollect.Count; i++)
            {
                itemPaths[i] = new LTBezierPath();

                Vector3 p0 = itemsToCollect[i].transform.position;
                Vector3 p1 = finalTargetPosition;

                Vector3 c0 = p0 + Vector3.up * 2;
                Vector3 c1 = p1 + Vector3.up * 2;

                itemPaths[i].setPoints(new Vector3[] {p0, c1, c0, p1});
            }

            while(collectedItems < 3)
            {
                for (int i = 0; i < itemsToCollect.Count; i++)
                {
                    Item item = itemsToCollect[i];

                    if (item.transform.position == finalTargetPosition)
                        continue;

                    item.DisablePhysics();                    

                    float percent = Mathf.Clamp01((timer - (i * delayBetweenItems)) / animationDuration);

                    Vector3 targetPosition = itemPaths[i].point(percent);

                    item.transform.position = targetPosition;

                    // Multiplied percent by 1.1f to scale down faster
                    item.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, percent * 1.1f);


                    if (item.transform.position == finalTargetPosition)
                    {
                        itemPickedUp?.Invoke(item);
                        collectedItems++;
                    }
                }

                // If the item has reached, increase the collected items count
                timer += Time.deltaTime;
                yield return null;
            }

            for (int i = itemsToCollect.Count - 1; i >= 0; i--)
                Destroy(itemsToCollect[i].gameObject);

            isUsingPowerup = false;
        }

        [Button]
        public void VacuumPowerupPressed()
        {
        
            if(vacuumPUCount <= 0)
            {
               
                vacuumPUCount += 3;
                SaveData();
            }
            else
            {
            
                isUsingPowerup = true;
                
                vacuumPUCount--;
                SaveData();

                // Play the vac animation
               vacuumAnimator.Play("VacuumActive");
            }
        }

        private ItemLevelData GetGreatestGoal(ItemLevelData[] goals)
        {
            int max = 0;
            int goalIndex = -1;

            for (int i = 0; i < goals.Length; i++)
            {
                if(goals[i].amount >= max)
                {
                    max = goals[i].amount;
                    goalIndex = i;
                }
            }

            return goals[goalIndex];

            // We could have written, only use if the array is small due to sorting
            // return goals.OrderByDescending(g => g.amount).FirstOrDefault();
        }
        private void UpdateVacuumVisuals()
        {
            vacuumVideoIcon.SetActive(vacuumPUCount <= 0);

            if (vacuumPUCount <= 0)
                vacuumAmountText.text = "";
            else
                vacuumAmountText.text = vacuumPUCount.ToString();
        }

        private void LoadData()
        {
            vacuumPUCount = PlayerPrefs.GetInt("VacuumCount", initialVacuumPUCount);
            UpdateVacuumVisuals();           
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("VacuumCount", vacuumPUCount);
        }
}
