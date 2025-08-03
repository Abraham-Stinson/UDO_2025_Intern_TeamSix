using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameSection
{
    [Header("Section Info")]
    public string sectionName;
    public Sprite sectionIcon;
    public Sprite sectionBackground;

    [Header("Levels")]
    public Level[] levels;

    [Header("Section Settings")]
    public bool isUnlocked = false;
    public Color sectionColor = Color.white;


}

public class LevelManager : MonoBehaviour, IGameStateListener
{
    public static LevelManager Instance { get; private set; }


    private List<Item> activeItems = new List<Item>();


    public Item[] Items => activeItems.ToArray();

    public Transform itemParent => currentLevel.ItemParent;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadData();
    }


    private void OnEnable()
    {
        ItemPlacer.OnItemSpawned += HandleItemSpawned;
        PowerUpManager.itemPickedUp += HandleItemRemoved;
    }


    private void OnDisable()
    {
        ItemPlacer.OnItemSpawned -= HandleItemSpawned;
        PowerUpManager.itemPickedUp -= HandleItemRemoved;
    }


    [Header("Data")]
    [SerializeField] public GameSection[] sections;

    private const string currentSectionKey = "CurrentSection";
    private const string currentLevelKey = "CurrentLevel";
    private const string unlockedSectionsKey = "UnlockedSections";

    private int currentSectionIndex;
    private int currentLevelIndex;

    [Header("Settings")]
    private Level currentLevel;
    private GameSection currentSection;

    [Header("Actions")]
    public static Action<Level> levelSpawned;
    public static Action<GameSection> sectionChanged;

    [Header("MainMenuBackground")]
    [SerializeField] private Sprite mainMenuBackground;
    [Header("In Game Level Data")]
    [SerializeField] private TextMeshProUGUI levelDataText;

    private void SpawnLevel()
    {
        // Health kontrolü
        if (HealthManager.health <= 0)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("health");
            GameManager.instance.SetGameState(EGameState.MENU);
            return;
        }

        transform.Clear();
        activeItems.Clear();
        currentSection = sections[currentSectionIndex];

        SectionAndLevelUI.Instance.backgroundImage.sprite = currentSection.sectionBackground;
        int validatedLevelIndex = currentLevelIndex % currentSection.levels.Length;
        currentLevel = Instantiate(currentSection.levels[validatedLevelIndex], transform);

        levelDataText.text = currentLevel.levelName;
        activeItems.AddRange(currentLevel.GetComponentsInChildren<Item>());

        // LevelSystem'i güncelle
        LevelSystem.Instance?.OnLevelStarted();

        levelSpawned?.Invoke(currentLevel);
        sectionChanged?.Invoke(currentSection);
    }


    private void HandleItemSpawned(Item newItem)
    {
        if (newItem != null && !activeItems.Contains(newItem))
        {
            activeItems.Add(newItem);
        }
    }


    private void HandleItemRemoved(Item removedItem)
    {

        activeItems.RemoveAll(item => item == null || item == removedItem);
    }

    private void LoadData()
    {
        currentSectionIndex = PlayerPrefs.GetInt(currentSectionKey, 0);
        currentLevelIndex = PlayerPrefs.GetInt(currentLevelKey, 0);
        LoadUnlockedSections();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(currentSectionKey, currentSectionIndex);
        PlayerPrefs.SetInt(currentLevelKey, currentLevelIndex);
        SaveUnlockedSections();
    }

    private void LoadUnlockedSections()
    {
        if (sections.Length > 0)
            sections[0].isUnlocked = true;

        for (int i = 1; i < sections.Length; i++)
        {
            sections[i].isUnlocked = PlayerPrefs.GetInt($"Section_{i}_Unlocked", 0) == 1;
        }
    }

    private void SaveUnlockedSections()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            PlayerPrefs.SetInt($"Section_{i}_Unlocked", sections[i].isUnlocked ? 1 : 0);
        }
    }

    public void GameStateChangedCallBack(EGameState gameState)
    {
        if (gameState == EGameState.GAME)
        {
            SpawnLevel();
            PowerUpManager.instance?.ResetPowerupState();
        }
        else if (gameState == EGameState.LEVELCOMPLETE)
        {
            // LevelSystem'i güncelle
            LevelSystem.Instance?.OnLevelCompleted();

            int rewardAmount = currentLevel.rewardCoin;
            MoneyManager.instance.IncreaseMoney(rewardAmount);
            SectionAndLevelUI.Instance.ShowRewardOnWinUI(rewardAmount);

            currentLevelIndex++;

            if (currentLevelIndex >= sections[currentSectionIndex].levels.Length)
            {
                currentLevelIndex = 0;
                currentSectionIndex++;

                if (currentSectionIndex < sections.Length)
                {
                    sections[currentSectionIndex].isUnlocked = true;
                }
                else
                {
                    currentSectionIndex = 0;
                }
            }
            SaveData();
        }
        else if (gameState == EGameState.GAMEOVER)
        {
            // LevelSystem'i güncelle
            LevelSystem.Instance?.OnLevelFailed();

            // Level kaybedildiğinde health azalt
            HealthManager.instance.ReduceHealth(1);
        }
    }

    public GameSection GetCurrentSection() => sections[currentSectionIndex];
    public Level GetCurrentLevel() => currentLevel;
    public GameSection[] GetAllSections() => sections;
    public int GetCurrentLevelIndex() => currentLevelIndex;
    public int GetCurrentSectionIndex() => currentSectionIndex;

    public bool IsSectionUnlocked(int sectionIndex)
    {
        if (sectionIndex >= 0 && sectionIndex < sections.Length)
            return sections[sectionIndex].isUnlocked;
        return false;
    }

    public void UnlockSection(int sectionIndex)
    {
        if (sectionIndex >= 0 && sectionIndex < sections.Length)
        {
            sections[sectionIndex].isUnlocked = true;
            SaveData();
        }
    }

    public void SelectLevel(int sectionIndex, int levelIndex)
    {
        if (sectionIndex >= 0 && sectionIndex < sections.Length &&
            sections[sectionIndex].isUnlocked &&
            levelIndex >= 0 && levelIndex < sections[sectionIndex].levels.Length)
        {
            currentSectionIndex = sectionIndex;
            currentLevelIndex = levelIndex;
            SaveData();
        }
    }

    public static implicit operator LevelManager(GameSection v)
    {
        throw new NotImplementedException();
    }

    public void LoadLevel(Level levelToLoad)
    {
        if (levelToLoad == null)
        {
            Debug.LogError("Level to load is null!");
            return;
        }

        // Geçerli level kontrolü
        bool levelFound = false;
        for (int i = 0; i < sections.Length; i++)
        {
            for (int j = 0; j < sections[i].levels.Length; j++)
            {
                if (sections[i].levels[j] == levelToLoad)
                {
                    currentSectionIndex = i;
                    currentLevelIndex = j;
                    levelFound = true;
                    break;
                }
            }
            if (levelFound) break;
        }

        if (!levelFound)
        {
            Debug.LogError("Level not found in any section!");
            return;
        }

        currentLevel = levelToLoad;
        // SpawnLevel() çağrısını kaldırıyoruz çünkü GameStateChangedCallBack'te çağrılacak
    }

    public void ExitLevel()
    {
        GameManager.instance.SetGameState(EGameState.MENU);
        DestroyCurrentLevel();
        SectionAndLevelUI.Instance.backgroundImage.sprite = mainMenuBackground;
    }
    public void DestroyCurrentLevel()
    {
        if (ItemSpotsManager.Instance != null)
        {
            ItemSpotsManager.Instance.ClearAllSpots();
        }
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
            currentLevel = null;
        }
    }

    public void NextLevel()
    {
        if (ItemSpotsManager.Instance != null)
        {
            ItemSpotsManager.Instance.ClearAllSpots();
        }
        // SpawnLevel() çağrısını kaldırıyoruz çünkü GameStateChangedCallBack'te çağrılacak
        GameManager.instance.SetGameState(EGameState.GAME);
    }

    // Mevcut seviyeyi tekrar başlatma methodu
    public void RestartCurrentLevel()
    {
        if (ItemSpotsManager.Instance != null)
        {
            ItemSpotsManager.Instance.ClearAllSpots();
        }

        // Mevcut seviyeyi tekrar başlat (level index'ini değiştirmeden)
        GameManager.instance.SetGameState(EGameState.GAME);
    }

    // Anamenüdeki "Son Level" butonu için metod
    public void PlayLastLevel()
    {
        // Health kontrolü
        if (HealthManager.health <= 0)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("health");
            return;
        }

        if (sections.Length == 0) return;

        // İlk section'ın ilk levelını varsayılan olarak ayarla
        currentSectionIndex = 0;
        currentLevelIndex = 0;
        
        // Eğer daha önce level oynandıysa, kaydedilen section ve level'ı yükle
        if (PlayerPrefs.HasKey(currentSectionKey) && PlayerPrefs.HasKey(currentLevelKey))
        {
            currentSectionIndex = PlayerPrefs.GetInt(currentSectionKey, 0);
            currentLevelIndex = PlayerPrefs.GetInt(currentLevelKey, 0);
        }

        // Section ve level index'lerinin geçerli olduğundan emin ol
        if (currentSectionIndex >= sections.Length)
            currentSectionIndex = 0;
        
        if (currentLevelIndex >= sections[currentSectionIndex].levels.Length)
            currentLevelIndex = 0;

        // LevelSystem'i güncelle
        LevelSystem.Instance?.UpdateLevelInfo();
        
        // Oyunu başlat
        GameManager.instance.SetGameState(EGameState.GAME);
    }
}