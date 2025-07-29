using System;
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

    private void Start()
    {
        // SpawnLevel();
    }

    private void SpawnLevel()
    {
        // Null ve array length kontrolleri
        if (sections == null || sections.Length == 0)
        {
            Debug.LogError("No sections defined!");
            return;
        }

        if (currentSectionIndex >= sections.Length)
        {
            Debug.LogError($"Invalid section index: {currentSectionIndex}");
            return;
        }

        transform.Clear();

        // Current section'ı al
        currentSection = sections[currentSectionIndex];

        if (currentSection.levels == null || currentSection.levels.Length == 0)
        {
            Debug.LogError($"No levels defined in section: {currentSection.sectionName}");
            return;
        }

        // Section içindeki level index'ini validate et
        int validatedLevelIndex = currentLevelIndex;
        if (currentSection.levels.Length > 0)
        {
            validatedLevelIndex = currentLevelIndex % currentSection.levels.Length;
        }

        // Level'ı spawn et
        currentLevel = Instantiate(currentSection.levels[validatedLevelIndex], transform);

        // Event'leri tetikle
        levelSpawned?.Invoke(currentLevel);
        sectionChanged?.Invoke(currentSection);

    }

    public void DestroyCurrentLevel()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
            currentLevel = null;
        }
    }

    private void LoadData()
    {
        currentSectionIndex = PlayerPrefs.GetInt(currentSectionKey, 0);
        currentLevelIndex = PlayerPrefs.GetInt(currentLevelKey, 0);

        // Unlocked sections'ları yükle
        LoadUnlockedSections();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(currentSectionKey, currentSectionIndex);
        PlayerPrefs.SetInt(currentLevelKey, currentLevelIndex);

        // Unlocked sections'ları kaydet
        SaveUnlockedSections();
    }

    private void LoadUnlockedSections()
    {
        // İlk section her zaman açık
        if (sections.Length > 0)
            sections[0].isUnlocked = true;

        // Diğer section'ların unlock durumunu yükle
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

            // Eğer current section'daki tüm leveller bittiyse bir sonraki section'a geç
            if (currentLevelIndex >= sections[currentSectionIndex].levels.Length)
            {
                currentLevelIndex = 0;
                currentSectionIndex++;

                // Yeni section'ı unlock et
                if (currentSectionIndex < sections.Length)
                {
                    sections[currentSectionIndex].isUnlocked = true;
                }
                else
                {
                    // Tüm section'lar bitti, başa dön veya özel bir durum
                    currentSectionIndex = 0;
                }
            }

            SaveData();
        }
    }

    // Utility methods
    public GameSection GetCurrentSection()
    {
        return sections[currentSectionIndex];
    }

    public Level GetCurrentLevel()
    {
        return currentLevel;
    }

    public GameSection[] GetAllSections()
    {
        return sections;
    }

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

    // Manuel section/level seçimi için (level selection UI için)
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
        SpawnLevel();
    }

    public void ExitLevel()
    {
        DestroyCurrentLevel();
        GameManager.instance.SetGameState(EGameState.MENU);
        
    }
}