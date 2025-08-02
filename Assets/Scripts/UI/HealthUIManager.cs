using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image healthBar; // Fill amount için Image component
    public TextMeshProUGUI healthText; // "5/10" şeklinde gösterim
    public TextMeshProUGUI timerText; // "09:45" şeklinde süre gösterimi
    public GameObject timerPanel; // Timer gösterilecek panel (opsiyonel)
    
    [Header("Bar Colors")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public Color mediumHealthColor = Color.yellow;
    
    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("Health Bar Image atanmamış!");
            return;
        }
        
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (HealthManager.instance == null || healthBar == null)
            return;

        // Health bar fill amount güncelle
        float healthPercentage = HealthManager.instance.GetHealthPercentage();
        healthBar.fillAmount = healthPercentage;
        
        // Health bar rengini güncelle
        UpdateHealthBarColor(healthPercentage);

        // Health text güncelle
        if (healthText != null)
        {
            healthText.text = HealthManager.health + "/" + HealthManager.instance.maxHealth;
        }

        // Timer güncelle
        UpdateTimer();
    }

    void UpdateHealthBarColor(float percentage)
    {
        Color targetColor;
        
        if (percentage > 0.6f)
        {
            // Yeşil (sağlıklı)
            targetColor = fullHealthColor;
        }
        else if (percentage > 0.3f)
        {
            // Sarı (orta)
            targetColor = mediumHealthColor;
        }
        else
        {
            // Kırmızı (tehlikeli)
            targetColor = lowHealthColor;
        }

        healthBar.color = targetColor;
    }

    void UpdateTimer()
    {
        if (HealthManager.health >= HealthManager.instance.maxHealth)
        {
            // Can dolu, timer gizle
            if (timerPanel != null)
                timerPanel.SetActive(false);
                
            if (timerText != null)
                timerText.text = "FULL";
        }
        else
        {
            // Can dolu değil, timer göster
            if (timerPanel != null)
                timerPanel.SetActive(true);

            if (timerText != null)
            {
                TimeSpan timeUntilRegen = HealthManager.instance.GetTimeUntilNextRegen();
                
                if (timeUntilRegen.TotalSeconds > 0)
                {
                    timerText.text = string.Format("{0:D2}:{1:D2}", 
                        timeUntilRegen.Minutes, timeUntilRegen.Seconds);
                }
                else
                {
                    timerText.text = "00:00";
                }
            }
        }
    }

    // Test için butona bağlanabilir
    public void TestReduceHealth()
    {
        if (HealthManager.instance != null)
        {
            HealthManager.instance.ReduceHealth(1);
        }
    }
}