using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSection
{
    [Header("Section Info")]
    public string sectionName;
    public Sprite sectionIcon;

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

    private void SpawnLevel()
    {
        transform.Clear();
        
        activeItems.Clear();

        currentSection = sections[currentSectionIndex];
        int validatedLevelIndex = currentLevelIndex % currentSection.levels.Length;
        currentLevel = Instantiate(currentSection.levels[validatedLevelIndex], transform);

      
        activeItems.AddRange(currentLevel.GetComponentsInChildren<Item>());

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
        }
        else if (gameState == EGameState.LEVELCOMPLETE)
        {
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
    }

    public GameSection GetCurrentSection() => sections[currentSectionIndex];
    public Level GetCurrentLevel() => currentLevel;
    public GameSection[] GetAllSections() => sections;

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
}