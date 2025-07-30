using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Elements")] 
    [SerializeField] private ItemPlacer itemplacer;

    [Header("Settings")]
    public string levelName;
    public int Duration => duration;
    [SerializeField] private int duration;

    

    public ItemLevelData[] GetGoals()
        => itemplacer.GetGoals();


    public Item[] GetItems()
    {
        return itemplacer.GetItems();
    }
}
