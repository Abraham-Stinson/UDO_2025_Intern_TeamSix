using UnityEngine;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI sectionNameText;

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
    }

    private void Start()
    {
        UpdateLevelInfo();
    }

    public void UpdateLevelInfo()
    {
        if (LevelManager.Instance == null) return;
        
        int currentSectionIndex = LevelManager.Instance.GetCurrentSectionIndex();
        int currentLevelIndex = LevelManager.Instance.GetCurrentLevelIndex();
        
        GameSection[] allSections = LevelManager.Instance.GetAllSections();
        
        if (allSections == null || allSections.Length == 0) return;
        
        // Section index kontrolü
        if (currentSectionIndex >= allSections.Length)
            currentSectionIndex = 0;
        
        GameSection targetSection = allSections[currentSectionIndex];
        
        // Level index kontrolü
        if (currentLevelIndex >= targetSection.levels.Length)
            currentLevelIndex = 0;
        
        Level targetLevel = targetSection.levels[currentLevelIndex];
        
        UpdateUI(targetLevel.levelName, targetSection.sectionName);
    }

    public void ShowNextLevel()
    {
        if (LevelManager.Instance == null) return;
        
        GameSection[] allSections = LevelManager.Instance.GetAllSections();
        int currentSectionIndex = LevelManager.Instance.GetCurrentSectionIndex();
        int currentLevelIndex = LevelManager.Instance.GetCurrentLevelIndex();
        
        // Bir sonraki level'ı hesapla
        int nextLevelIndex = currentLevelIndex + 1;
        int nextSectionIndex = currentSectionIndex;
        
        // Section geçişi kontrolü
        if (nextLevelIndex >= allSections[currentSectionIndex].levels.Length)
        {
            nextLevelIndex = 0;
            nextSectionIndex++;
            
            if (nextSectionIndex >= allSections.Length)
                nextSectionIndex = 0;
        }
        
        // Bir sonraki level'ın bilgilerini al
        if (nextSectionIndex < allSections.Length && nextLevelIndex < allSections[nextSectionIndex].levels.Length)
        {
            GameSection nextSection = allSections[nextSectionIndex];
            Level nextLevel = nextSection.levels[nextLevelIndex];
            
            UpdateUI(nextLevel.levelName, nextSection.sectionName);
        }
    }

    private void UpdateUI(string levelName, string sectionName)
    {
        if (levelNameText != null)
            levelNameText.text = levelName;
        
        if (sectionNameText != null)
            sectionNameText.text = sectionName;
    }

    public void OnLevelCompleted()
    {
        // Level tamamlandığında bir sonraki levelı göster
        ShowNextLevel();
    }

    public void OnLevelFailed()
    {
        // Level kaybedildiğinde mevcut levelı göster
        UpdateLevelInfo();
    }

    public void OnLevelStarted()
    {
        UpdateLevelInfo();
    }
} 