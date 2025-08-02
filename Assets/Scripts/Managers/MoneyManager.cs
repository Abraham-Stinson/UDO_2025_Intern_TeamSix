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
        LoadMoney();
        UpdateMoneyAndUI();
    }

    // Para yükleme methodu
    private void LoadMoney()
    {
        money = PlayerPrefs.GetInt(CURRENT_MONEY, 0); // Varsayılan değer 0
        Debug.Log($"Money loaded: {money}");
    }

    // Para kaydetme methodu
    private void SaveMoney()
    {
        PlayerPrefs.SetInt(CURRENT_MONEY, money);
        PlayerPrefs.Save();
        Debug.Log($"Money saved: {money}");
    }

    public void ReduceMoney(int amount)
    {
        money -= amount;
        money = Mathf.Max(money, 0); // Para negatif olmasın
        UpdateMoneyAndUI();
    }

    public void IncreaseMoney(int amount)
    {
        money += amount;
        UpdateMoneyAndUI();
    }

    private void UpdateMoneyAndUI()
    {
        if (moneyTextUI != null)
        {
            moneyTextUI.text = money.ToString();
        }
        SaveMoney();
    }

    public void BuyHealthWithMoney(int healthAmount, int moneyAmount)
    {
        if (money == 0 || money < moneyAmount)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("money");
            return;
        }
        else if (healthAmount>=10||HealthManager.health+healthAmount>10) {
            SectionAndLevelUI.Instance.WarningMesageUI("overHealth");
            return;
        }

        ReduceMoney(moneyAmount);
        HealthManager.instance.AddHealth(healthAmount);
    }

    // Para sıfırlama methodu (test için)
    public void ResetMoney()
    {
        money = 0;
        UpdateMoneyAndUI();
        Debug.Log("Money reset to 0");
    }

    // Para ayarlama methodu (test için)
    public void SetMoney(int amount)
    {
        money = Mathf.Max(amount, 0);
        UpdateMoneyAndUI();
        Debug.Log($"Money set to: {money}");
    }
}
