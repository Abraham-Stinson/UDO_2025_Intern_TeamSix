using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;
    
    [Header("Elements")] 
    [SerializeField] private Transform goalCardParent;
    [SerializeField] private GoalCard goalCardPrefab;
    
    [Header("Data")] 
    private ItemLevelData[] goals;

    private List<GoalCard> goalCards = new List<GoalCard>();
    private Level lastLoadedLevel; // Aynı level'ın tekrar yüklenmesini önlemek için

    public ItemLevelData[] Goals => goals;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        LevelManager.levelSpawned += OnLevelSpawned;
        ItemSpotsManager.itemPickedUp += OnItemPickedUp;
        PowerUpManager.itemPickedUp += OnItemPickedUp;
    }

    private void OnDestroy()
    {
        LevelManager.levelSpawned -= OnLevelSpawned;
        ItemSpotsManager.itemPickedUp -= OnItemPickedUp;
        PowerUpManager.itemPickedUp -= OnItemPickedUp;
    }

    private void OnLevelSpawned(Level level)
    {
        // Aynı level tekrar yükleniyorsa işlem yapma
        if (lastLoadedLevel == level)
        {
            Debug.Log("Aynı level tekrar yüklendi, goals zaten mevcut!");
            return;
        }

        lastLoadedLevel = level;
        Debug.Log($"Yeni level yüklendi: {level.name}");
        
        goals = level.GetGoals();
        GenerateGoalCards();
    }

    private void GenerateGoalCards()
    {
        // ÖNEMLİ: Önce eski goal card'ları temizle
        ClearGoalCards();
        
        // Yeni goal card'ları oluştur
        for (int i = 0; i < goals.Length; i++)
            GenerateGoalCard(goals[i]);
            
        Debug.Log($"Goal cards oluşturuldu: {goalCards.Count} adet");
    }

    private void ClearGoalCards()
    {
        // Mevcut goal card'ları yok et
        foreach (var card in goalCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        
        // Listeyi temizle
        goalCards.Clear();
        
        Debug.Log("Eski goal cards temizlendi");
    }

    private void GenerateGoalCard(ItemLevelData goal)
    {
        GoalCard cardInstance = Instantiate(goalCardPrefab, goalCardParent);
        
        cardInstance.Configure(goal.amount, goal.itemPrefab.Icon);
        
        goalCards.Add(cardInstance);
    }

    private void OnItemPickedUp(Item item)
    {
        if (goals == null || goals.Length == 0) return;

        for (int i = 0; i < goals.Length; i++)
        {
            if(!goals[i].itemPrefab.ItemName.Equals(item.ItemName))
                continue;

            goals[i].amount--;

            if (goals[i].amount <= 0)
                CompleteGoals(i);
            else
            {
                if (i < goalCards.Count && goalCards[i] != null)
                    goalCards[i].UpdateAmount(goals[i].amount);
            }

            break;
        }
    }

    private void CompleteGoals(int goalIndex)
    {
       Debug.Log("Goal Complete : " + goals[goalIndex].itemPrefab.ItemName);
       
       if (goalIndex < goalCards.Count && goalCards[goalIndex] != null)
           goalCards[goalIndex].Complate();

       CheckForLevelComplete();
    }

    private void CheckForLevelComplete()
    {
        for (int i = 0; i < goals.Length; i++)
        {
            if(goals[i].amount > 0)
                return;
        }
        
        GameManager.instance.SetGameState(EGameState.LEVELCOMPLETE);
    }

    // Level değişiminde manuel temizleme için
    public void ClearCurrentGoals()
    {
        ClearGoalCards();
        goals = null;
        lastLoadedLevel = null;
    }
}