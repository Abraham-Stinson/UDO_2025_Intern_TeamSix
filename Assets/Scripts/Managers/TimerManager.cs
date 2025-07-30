using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour, IGameStateListener
{
    public static TimerManager instance;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float currentTimer; // Use float for more precise timing
    private bool isTimerRunning;

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
        UpdateTimerText();
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

            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        timerText.text = SecondToString((int)currentTimer);
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
        if (gameState == EGameState.LEVELCOMPLETE || gameState == EGameState.GAMEOVER)
            StopTimer();
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
}
