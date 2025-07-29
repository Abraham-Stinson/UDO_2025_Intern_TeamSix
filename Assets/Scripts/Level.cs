using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Elements")] 
    [SerializeField] private ItemPlacer itemplacer;

    [Header("Settings")]
    public int Duration => duration;
    [SerializeField] private int duration;

    

    public ItemLevelData[] GetGoals()
        => itemplacer.GetGoals();

}
