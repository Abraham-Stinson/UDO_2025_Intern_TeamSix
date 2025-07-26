using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalCard : MonoBehaviour
{
   [Header("Elements")]
   [SerializeField] private Image iconImage;
   [SerializeField] private TextMeshProUGUI amountText;


   public void Configure(int initialAmount,Sprite icon)
   {
      amountText.text = initialAmount.ToString();
      iconImage.sprite = icon;
   }

   public void UpdateAmount(int amount)
   {
      amountText.text = amount.ToString();
   }

   public void Complate()
   {
      gameObject.SetActive(false);
   }
}
