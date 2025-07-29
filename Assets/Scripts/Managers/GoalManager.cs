using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform goalCardParent;
    [SerializeField] private GoalCard goalCardPrefab;
    
    [Header("Data")] 
    private ItemLevelData[] goals;

    private List<GoalCard> goalCards = new List<GoalCard>();
    private Level currentLevel;

    private void Awake()
    {
        LevelManager.levelSpawned += OnLevelSpawned;
        ItemSpotsManager.itemPickedUp += OnItemPickedUp;
    }

    private void OnDestroy()
    {
        LevelManager.levelSpawned -= OnLevelSpawned;
        ItemSpotsManager.itemPickedUp -= OnItemPickedUp;
    }

    private void OnLevelSpawned(Level level)
    {
        // Eğer aynı level tekrar yükleniyorsa işlem yapma
        if (currentLevel == level && goals != null && goals.Length > 0)
        {
            Debug.Log("Aynı level, goals zaten yüklendi.");
            return;
        }

        currentLevel = level;
        
        // Eski goal card'ları temizle
        ClearGoalCards();
        
        // Yeni goals'ları al
        goals = level.GetGoals();
        Debug.Log($"Goals yüklendi: {goals.Length} adet");

        GenerateGoalCards();
    }

    private void ClearGoalCards()
    {
        foreach (var card in goalCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        goalCards.Clear();
    }

    private void GenerateGoalCards()
    {
        for (int i = 0; i < goals.Length; i++)
            GenerateGoalCard(goals[i]);
    }

    private void GenerateGoalCard(ItemLevelData goal)
    {
        GoalCard cardInstance = Instantiate(goalCardPrefab, goalCardParent);
        
        cardInstance.Configure(goal.amount, goal.itemPrefab.Icon);
        
        goalCards.Add(cardInstance);
    }

    private void OnItemPickedUp(Item item)
    {
        for (int i = 0; i < goals.Length; i++)
        {
            if(!goals[i].itemPrefab.ItemName.Equals(item.ItemName))
                continue;

            goals[i].amount--;

            if (goals[i].amount <= 0)
                CompleteGoals(i);
            else
            {
                goalCards[i].UpdateAmount(goals[i].amount);
            }

            break;
        }
    }

    private void CompleteGoals(int goalIndex)
    {
       Debug.Log("Goal Complete : " + goals[goalIndex].itemPrefab.ItemName);
       
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
        Debug.Log("Level Complete!");
        GameManager.instance.SetGameState(EGameState.LEVELCOMPLETE);
    }
}