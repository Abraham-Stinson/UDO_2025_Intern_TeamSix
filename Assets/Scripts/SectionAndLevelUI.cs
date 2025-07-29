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
    [SerializeField] private GameObject mainMenuScreenUI;
    [Header("Sections")]
    [SerializeField] private GameObject sectionPanelPrefab;
    [SerializeField] private Transform sectionPanelParent;

    [Header("Levels")]
    [SerializeField] private GameObject levelPanelPrefab;
    [SerializeField] private Transform levelPanelParent;

    [Header("Navigation")]
    [SerializeField] private Button backButton;

    [Header("Pause Menu")]
    [SerializeField] private GameObject inGameScreenUI;
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

    public void ShowMainMenu()
    {
        GameManager.instance.SetGameState(EGameState.MENU);
    }

    #region Section Management
    public void SetupSectionPanel()
    {
        if (!ValidateSectionComponents()) return;

        // Ana menüyü gizle ve section paneli göster
        mainMenuScreenUI.SetActive(false);
        sectionPanelParent.gameObject.SetActive(true);

        // Eğer section panelin içeriği boşsa yeni içerik oluştur
        if (sectionPanelParent.childCount == 0)
        {
            CreateSectionPanels();
        }

        // Back button'u göster ve event'ini ayarla
        backButton.gameObject.SetActive(true);
        SetupBackButtonForSection();

        GameManager.instance.SetGameState(EGameState.SECTIONSELECTION);
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
        // Önceki içeriği temizle
        foreach (Transform child in sectionPanelParent)
        {
            Destroy(child.gameObject);
        }

        // Yeni section panelleri oluştur
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
        // Section paneli gizle ama içeriğini silme
        sectionPanelParent.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        mainMenuScreenUI.SetActive(true);
        GameManager.instance.SetGameState(EGameState.MENU);
    }

    #endregion

    #region Level Management
    private void SetupLevelPanel(GameSection sectionData)
    {
        mainMenuScreenUI.SetActive(false); // Section panel butonunu gizle
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
        /*// UI elementlerini güncelle
        levelPanelParent.gameObject.SetActive(false);  // Level seçme panelini kapat*/
        backButton.gameObject.SetActive(false);         // Back butonunu kapat
        GameManager.instance.SetGameState(EGameState.GAME); // Oyun state'ini güncelle  
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
        InputManager.instance.enabled = false; // Inputları devre dışı bırak
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        InputManager.instance.enabled = true; // Inputları aktİf et
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
    }

    public void ExitLevel()
    {
        InputManager.instance.enabled = true; // Inputları aktİf et
        ShowMainMenu();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        GameManager.instance.SetGameState(EGameState.MENU);
        ClearLevelPanel();
        LevelManager.Instance.ExitLevel();
    }
    #endregion
}
