using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Actions")] 
    public static Action<Item> itemPickedUp;
    

    [Button]
    private void VacumPowerUp()
    {

        Item[] items = LevelManager.Instance.Items;
        ItemLevelData[] goals = GoalManager.instance.Goals;

        ItemLevelData? greatesGoal = GetGreatesGoal(goals);
        
        if(greatesGoal == null)
            return;

        ItemLevelData goal = (ItemLevelData)greatesGoal;
        List<Item> itemsToCollect = new List<Item>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ItemName == goal.itemPrefab.ItemName)
            {
                itemsToCollect.Add(items[i]);

                if (itemsToCollect.Count >= 3)
                    break;
            }
        }

        for (int i = itemsToCollect.Count-1; i >=0; i--)
        {
            itemPickedUp?.Invoke(itemsToCollect[i]);
            Destroy(itemsToCollect[i].gameObject);
        }
     

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

}
