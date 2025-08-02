using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyTextUI;
    public int money;
    public static MoneyManager instance;

    private const string CURRENT_MONEY = "CurrentMoney";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        UpdateMoneyAndUI();
    }
    public void ReduceMoney(int amount)
    {
        money -= amount;
        UpdateMoneyAndUI();
    }

    public void IncreaseMoney(int amount)
    {
        money += amount;
        UpdateMoneyAndUI();
    }

    private void UpdateMoneyAndUI()
    {
        moneyTextUI.text = money.ToString();
        PlayerPrefs.SetInt(CURRENT_MONEY, money);
        PlayerPrefs.Save();
    }

    public void BuyHealthWithMoney(int healthAmount, int moneyAmount)
    {
        if (money <= 0 || money < moneyAmount)
            SectionAndLevelUI.Instance.WarningMesageUI("money");

        ReduceMoney(moneyAmount);
        HealthManager.instance.AddHealth(healthAmount);
    }
    
}
