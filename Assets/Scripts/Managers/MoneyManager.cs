using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BuyableHealth
{
    public int healthAmount;
    public int moneyAmount;
}
public class MoneyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyTextUI;
    public int money;
    public static MoneyManager instance;
    [SerializeField] public BuyableHealth[] buyableHealths;
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
        
        // Check if buyableHealths array is properly initialized
        if (buyableHealths == null || buyableHealths.Length == 0)
        {
            Debug.LogWarning("buyableHealths array is null or empty! Please check the inspector.");
        }
        else
        {
            Debug.Log($"buyableHealths array initialized with {buyableHealths.Length} elements");
        }
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

    public void BuyHealthWithGold(int indexOfList)
    {
        // Null check for buyableHealths array
        if (buyableHealths == null)
        {
            Debug.LogError("buyableHealths array is null!");
            return;
        }

        // Check if index is valid
        if (indexOfList < 0 || indexOfList >= buyableHealths.Length)
        {
            Debug.LogError($"Invalid index: {indexOfList}. Array length: {buyableHealths.Length}");
            return;
        }

        // Check if the specific element is null
        if (buyableHealths[indexOfList] == null)
        {
            Debug.LogError($"buyableHealths[{indexOfList}] is null!");
            return;
        }

        if (money == 0 || money < buyableHealths[indexOfList].moneyAmount)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("money");
            return;
        }
        else if (buyableHealths[indexOfList].healthAmount >= 10 || HealthManager.health + buyableHealths[indexOfList].healthAmount > 10)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("overHealth");
            return;
        }

        ReduceMoney(buyableHealths[indexOfList].moneyAmount);
        HealthManager.instance.AddHealth(buyableHealths[indexOfList].healthAmount);
        UpdateMoneyAndUI();
        HealthUIManager.Instance.UpdateUI();
    }
    public void ReduceMoney(int amount)
    {
        if (money <= 0 && money < amount)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("money");
            return;
        }
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
