using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectionAndLevelUI : MonoBehaviour
{
    #region Singleton
    public static SectionAndLevelUI Instance { get; private set; }
    #endregion

    #region Variables
    public EGameState gameState;

    [Header("MainMenu")]
    [SerializeField] private GameObject sectionPanelButton;
    [Header("Sections")]
    [SerializeField] private GameObject sectionPanelPrefab;
    [SerializeField] private Transform sectionPanelParent;

    [Header("Levels")]
    [SerializeField] private GameObject levelPanelPrefab;
    [SerializeField] private Transform levelPanelParent;

    [Header("Navigation")]
    [SerializeField] private Button backButton;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject pauseMenuButton;
    
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        //InitializeSections();
    }
    #endregion

    #region Initialization Methods
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*private void InitializeSections()
    {
        if (LevelManager.Instance != null)
        {
            SetupSectionPanel();
        }
        else
        {
            Debug.LogError("LevelManager instance is null!");
        }
    }*/
    #endregion

    #region Section Management
    public void SetupSectionPanel()
    {
        sectionPanelButton.SetActive(false); // Section panel butonunu gizle
        if (!ValidateSectionComponents()) return;

        sectionPanelParent.gameObject.SetActive(true); // Section paneli aç
        backButton.gameObject.SetActive(true); // Back butonu göster
        ClearLevelPanel();
        CreateSectionPanels();
        SetupBackButtonForSection();
    }

    private bool ValidateSectionComponents()
    {
        if (sectionPanelPrefab == null || sectionPanelParent == null)
        {
            Debug.LogError("Section panel prefab or parent is not assigned!");
            return false;
        }

        if (LevelManager.Instance == null || LevelManager.Instance.sections == null)
        {
            Debug.LogError("LevelManager or sections array is null!");
            return false;
        }

        return true;
    }

    private void ClearSectionPanel()
    {
        foreach (Transform child in sectionPanelParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateSectionPanels()
    {
        foreach (var section in LevelManager.Instance.sections)
        {
            if (section == null) continue;
            CreateSingleSectionPanel(section);
        }
    }

    private void CreateSingleSectionPanel(GameSection section)
    {
        GameObject sectionPanel = Instantiate(sectionPanelPrefab, sectionPanelParent);
        
        var sectionIconUI = sectionPanel.transform.Find("SectionIcon")?.GetComponent<Image>();
        var sectionLabelUI = sectionPanel.transform.Find("SectionLabel")?.GetComponent<TextMeshProUGUI>();
        var sectionButtonUI = sectionPanel.transform.Find("SectionButton")?.GetComponent<Button>();

        if (sectionIconUI != null) sectionIconUI.sprite = section.sectionIcon;
        if (sectionLabelUI != null) sectionLabelUI.text = section.sectionName;
        if (sectionButtonUI != null)
        {
            sectionButtonUI.onClick.AddListener(() => SetupLevelPanel(section));
        }
    }

    private void SetupBackButtonForSection()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnSectionBackButtonPressed);
        }
    }

    private void OnSectionBackButtonPressed()
    {
        sectionPanelButton.SetActive(true); // Section panel butonunu göster

        sectionPanelParent.gameObject.SetActive(false); // Section paneli kapat
        backButton.gameObject.SetActive(false); // Back butonu gizle
        SetGameState(EGameState.MENU); // Menu state'ine geç
        ClearSectionPanel(); // Section paneli temizle
    }

    #endregion

    #region Level Management
    private void SetupLevelPanel(GameSection sectionData)
    {
        sectionPanelButton.SetActive(false); // Section panel butonunu gizle
        SetGameState(EGameState.LEVELSELECTION);
        ClearSectionPanel(); // Section paneli temizle
        CreateLevelPanel(sectionData);
        SetupBackButtonForLevel(); // Level için back button
    }

    private void CreateLevelPanel(GameSection sectionData)
    {
        for (int i = 0; i < sectionData.levels.Length; i++)
        {
            CreateSingleLevelPanel(sectionData.levels[i]);
        }
    }

    private void CreateSingleLevelPanel(Level level)
    {
        GameObject levelPanel = Instantiate(levelPanelPrefab, levelPanelParent);
        
        var levelNameUI = levelPanel.transform.Find("LevelName")?.GetComponent<TextMeshProUGUI>();
        var levelButtonUI = levelPanel.transform.Find("LevelButton")?.GetComponent<Button>();

        if (levelNameUI != null) levelNameUI.text = level.name;
        if (levelButtonUI != null) levelButtonUI.onClick.AddListener(() => LoadLevel(level));
    }

    private void LoadLevel(Level levelToLoad)
    {
        // UI elementlerini güncelle
        levelPanelParent.gameObject.SetActive(false);  // Level seçme panelini kapat
        backButton.gameObject.SetActive(false);         // Back butonunu kapat
        pauseMenuButton.SetActive(true);                // Pause butonunu göster
        
        // Temizlik ve level yükleme
        ClearLevelPanel();
        LevelManager.Instance.LoadLevel(levelToLoad);
        
        // Game state'i güncelle
        SetGameState(EGameState.GAME);
    }

    private void ClearLevelPanel()
    {
        foreach (Transform child in levelPanelParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetupBackButtonForLevel()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnLevelBackButtonPressed);
        }
    }

    private void OnLevelBackButtonPressed()
    {
        ClearLevelPanel(); // Level paneli temizle
        SetupSectionPanel(); // Section paneline geri dön
        SetGameState(EGameState.SECTIONSELECTION); // Game state'i güncelle
    }
    #endregion

    #region Game State Management
    private void SetGameState(EGameState newState)
    {
        gameState = newState;
        GameManager.instance.SetGameState(newState);
    }

    public void PauseGame()
    {
        // Time.timeScale = 0;
        // pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        // Time.timeScale = 1;
        // pauseMenuUI.SetActive(false);
    }

    public void ExitLevel()
    {
        // Time.timeScale = 1;
        // pauseMenuUI.SetActive(false);
        // pauseMenuButton.SetActive(false);
        // ClearLevelPanel();
        // GameManagerOLD.Instance.DestroyCurrentLevel();
    }
    #endregion
}
