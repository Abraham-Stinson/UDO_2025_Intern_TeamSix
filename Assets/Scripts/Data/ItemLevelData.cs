using UnityEngine;

[System.Serializable]
public struct ItemLevelData
{
    public Item itemPrefab;
    public bool isGoal;

    [NaughtyAttributes.ValidateInput("ValidateAmount", "amount must be multiple of 3")]
    [NaughtyAttributes.AllowNesting]
    [Range(0, 100)]
    public int amount;

    

    private bool ValidateAmount()
    {
        return amount % 3 == 0;
    }
}
