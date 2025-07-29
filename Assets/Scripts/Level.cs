using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Elements")] 
    [SerializeField] private ItemPlacer itemplacer;

    [Header("Settings")]
    [SerializeField] private int duration;

    public int Duration => duration;

    public ItemLevelData[] GetGoals()
        => itemplacer.GetGoals();

}
