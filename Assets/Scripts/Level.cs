using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Elements")] 
    [SerializeField] private ItemPlacer itemplacer;

    public ItemLevelData[] GetGoals()
        => itemplacer.GetGoals();

}
