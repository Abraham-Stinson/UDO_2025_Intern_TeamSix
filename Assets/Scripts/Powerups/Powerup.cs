using TMPro;
using UnityEngine;

public enum EPowerupType
{
    Vacuum = 0,
    Spring = 1,
    Fan = 2,
    FreezeGun= 3
    
}
public abstract class Powerup : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private EPowerupType type;
    public EPowerupType Type => type;

    [Header("Elements")] 
    [SerializeField] private TextMeshPro amountText;
    [SerializeField] private GameObject videoIcon;

    public void UpdateVisuals(int amount)
    {
        videoIcon.SetActive(amount <= 0);

        if (amount <= 0)
            amountText.text = "";
        else
            amountText.text = amount.ToString();
    }
}
