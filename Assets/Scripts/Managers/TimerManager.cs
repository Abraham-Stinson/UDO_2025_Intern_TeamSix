using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TimerManager : MonoBehaviour, IGameStateListener
{
    public static TimerManager instance;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timeFillImage;

    private float currentTimer; // Use float for more precise timing
    private float maxTimer; // Başlangıç timer değerini saklamak için
    private bool isTimerRunning;
    [SerializeField] private float masscotDisapearPercent = 20f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LevelManager.levelSpawned += OnLevelSpawned;
    }

    private void OnDestroy()
    {
        LevelManager.levelSpawned -= OnLevelSpawned;
    }

    private void OnLevelSpawned(Level level)
    {
        currentTimer = level.Duration;
        maxTimer = level.Duration; // Maksimum süreyi kaydet
        UpdateTimerDisplay();
        StartTimer();
    }

    private void StartTimer()
    {
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTimer -= Time.deltaTime; // Decrease timer by the time passed since last frame

            if (currentTimer <= 0)
            {
                currentTimer = 0; // Ensure timer doesn't go negative
                TimerFinished();
            }

            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        // Timer text'ini güncelle
        UpdateTimerText();
        // Fill image'ı güncelle
        UpdateTimerFill();
        //Masscot
        CheckMasscotMustBeActive();
    }

    private void UpdateTimerText()
    {
        timerText.text = SecondToString((int)currentTimer);
    }

    private void UpdateTimerFill()
    {
        if (timeFillImage != null && maxTimer > 0)
        {
            // Fill amount'u kalan sürenin oranına göre ayarla (0 ile 1 arasında)
            float fillAmount = currentTimer / maxTimer;
            timeFillImage.fillAmount = fillAmount;
        }
    }

    private void TimerFinished()
    {
        isTimerRunning = false; // Stop the timer
        GameManager.instance.SetGameState(EGameState.GAMEOVER);

        SectionAndLevelUI.Instance.ShowLoseScreen(1);
    }

    private string SecondToString(int seconds)
    {
        return TimeSpan.FromSeconds(seconds).ToString().Substring(3);
    }

    public void GameStateChangedCallBack(EGameState gameState)
    {
        if (gameState != EGameState.GAME)
        {
            SectionAndLevelUI.Instance.ShowAnxiousMasscot(false);
            StopTimer();
        }
            
    }

    private void StopTimer()
    {
        isTimerRunning = false; // Stop the timer
    }

    public void FreezeTimer()
    {
        StopTimer();
        Invoke("StartTimer", 5);
    }
    private void CheckMasscotMustBeActive()
    {
        float timePercent = (currentTimer / maxTimer) * 100;
        if (timePercent > masscotDisapearPercent)
        {
            return;
        }
        else
        {
            SectionAndLevelUI.Instance.ShowAnxiousMasscot(true);
        }
    }
}