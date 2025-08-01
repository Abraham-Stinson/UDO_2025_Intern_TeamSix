using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalCard : MonoBehaviour
{
   [Header("Elements")]
   [SerializeField] private Image iconImage;
   [SerializeField] private TextMeshProUGUI amountText;
   [SerializeField] private GameObject checkMark;
   [SerializeField] private GameObject backFace;
   [SerializeField] private Animator animator;

   public static event Action complete;
   
   private void Start()
   {
      animator.enabled = false;
   }

   private void Update()
   {
      backFace.SetActive(Vector3.Dot(Vector3.forward, transform.forward) <0);
   }

   public void Configure(int initialAmount,Sprite icon)
   {
      amountText.text = initialAmount.ToString();
      iconImage.sprite = icon;
   }

   public void UpdateAmount(int amount)
   {
      amountText.text = amount.ToString();
   }

   private void Bump()
   {
      LeanTween.cancel(gameObject);
      transform.localScale = Vector3.one;
      LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.1f)
         .setLoopPingPong(1);
   }

   public void Complate()
   {
     // gameObject.SetActive(false);
     animator.enabled = true;
     
      checkMark.SetActive(true);
      amountText.text = "";
      animator.Play("Complete");
      
      complete?.Invoke();
   }
}
