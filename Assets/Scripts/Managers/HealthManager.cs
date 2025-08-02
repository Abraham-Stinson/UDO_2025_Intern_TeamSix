using UnityEngine;
using System;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance; // intiliaze yerine instance
    public static int health;
    public int maxHealth = 10;
    public int minHealth = 0;
    public int regenIntervalMinutes = 10;

    private const string LAST_SAVE_TIME_KEY = "LastSaveTime";
    private const string CURRENT_LIVES_KEY = "CurrentLives";

    private DateTime nextRegenTime;

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
        UpdateLives();
        StartCoroutine(UpdateHealthRoutine());
    }

    void UpdateLives()
    {
        DateTime lastSaveTime = GetLastSaveTime();
        DateTime currentTime = DateTime.Now;

        health = PlayerPrefs.GetInt(CURRENT_LIVES_KEY, maxHealth); // currentLives yerine health kullan

        if (health < maxHealth)
        {
            TimeSpan timeDifference = currentTime - lastSaveTime;
            int minutesPassed = (int)timeDifference.TotalMinutes;

            int livesToAdd = minutesPassed / regenIntervalMinutes;
            health = Mathf.Min(health + livesToAdd, maxHealth);

            PlayerPrefs.SetInt(CURRENT_LIVES_KEY, health);

            // Sonraki regen zamanını hesapla
            if (health < maxHealth)
            {
                int remainingMinutes = regenIntervalMinutes - (minutesPassed % regenIntervalMinutes);
                nextRegenTime = currentTime.AddMinutes(remainingMinutes);
            }
        }
        else
        {
            nextRegenTime = currentTime.AddMinutes(regenIntervalMinutes);
        }

        SaveCurrentTime();
    }

    IEnumerator UpdateHealthRoutine()
    {
        while (true)
        {
            if (health < maxHealth)
            {
                DateTime currentTime = DateTime.Now;

                if (currentTime >= nextRegenTime)
                {
                    health++;
                    PlayerPrefs.SetInt(CURRENT_LIVES_KEY, health);

                    if (health < maxHealth)
                    {
                        nextRegenTime = currentTime.AddMinutes(regenIntervalMinutes);
                    }

                    SaveCurrentTime();
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void ReduceHealth(int reduceAmount)
    {
        health -= reduceAmount;
        health = Mathf.Max(health, minHealth);

        // İlk can kaybında timer'ı başlat
        if (health == maxHealth - reduceAmount)
        {
            nextRegenTime = DateTime.Now.AddMinutes(regenIntervalMinutes);
        }

        PlayerPrefs.SetInt(CURRENT_LIVES_KEY, health);
        SaveCurrentTime();
    }

    public float GetHealthPercentage()
    {
        return (float)health / maxHealth;
    }

    public TimeSpan GetTimeUntilNextRegen()
    {
        if (health >= maxHealth)
            return TimeSpan.Zero;

        TimeSpan timeLeft = nextRegenTime - DateTime.Now;
        return timeLeft.TotalSeconds > 0 ? timeLeft : TimeSpan.Zero;
    }

    private void SaveCurrentTime()
    {
        string timeString = DateTime.Now.ToBinary().ToString();
        PlayerPrefs.SetString(LAST_SAVE_TIME_KEY, timeString);
        PlayerPrefs.Save();
    }

    private DateTime GetLastSaveTime()
    {
        string timeString = PlayerPrefs.GetString(LAST_SAVE_TIME_KEY, "");
        if (string.IsNullOrEmpty(timeString))
        {
            return DateTime.Now;
        }

        long timeLong = Convert.ToInt64(timeString);
        return DateTime.FromBinary(timeLong);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveCurrentTime();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveCurrentTime();
        else UpdateLives();
    }

    public void AddHealth(int amount)
    {
        if (health >= 0 && health + amount <= 10)
            health += amount;
        else if (health + amount > 10)
            SectionAndLevelUI.Instance.WarningMesageUI("overHealth");
    }
}

