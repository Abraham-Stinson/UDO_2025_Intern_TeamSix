using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SectionAndLevelUI : MonoBehaviour
{
    public static SectionAndLevelUI Instance { get; private set; }
    public EGameState gameState;

    [Header("Sections")]
    [SerializeField] private GameObject sectionPanelPrefab;
    [SerializeField] private Transform sectionPanelParent;
    [Header("Levels")]
    [SerializeField] private GameObject levelPanelPrefab;
    [SerializeField] private Transform levelPanelParent;
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private bool isOnGame = true;
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject pauseMenuButton;

    void Awake()
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

    void Start()
    {
        // LevelManager'ın hazır olduğundan emin olalım
        if (LevelManager.Instance != null)
        {
            SetupSectionPanel();
        }
        else
        {
            Debug.LogError("LevelManager instance is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetupSectionPanel()
    {
        // Null kontrolleri ekleyelim
        if (sectionPanelPrefab == null || sectionPanelParent == null)
        {
            Debug.LogError("Section panel prefab or parent is not assigned!");
            return;
        }

        if (LevelManager.Instance == null || LevelManager.Instance.sections == null)
        {
            Debug.LogError("LevelManager or sections array is null!");
            return;
        }

        // Mevcut panelleri temizleyelim
        foreach (Transform child in sectionPanelParent)
        {
            Destroy(child.gameObject);
        }

        // Yeni panelleri oluşturalım
        foreach (var section in LevelManager.Instance.sections)
        {
            if (section == null) continue;

            GameObject sectionPanel = Instantiate(sectionPanelPrefab, sectionPanelParent);
            var sectionIconUI = sectionPanel.transform.Find("SectionIcon")?.GetComponent<Image>();
            var sectionLabelUI = sectionPanel.transform.Find("SectionLabel")?.GetComponent<TextMeshProUGUI>();
            var sectionButtonUI = sectionPanel.transform.Find("SectionButton")?.GetComponent<Button>();

            if (sectionIconUI != null) sectionIconUI.sprite = section.sectionIcon;
            if (sectionLabelUI != null) sectionLabelUI.text = section.sectionName;

            GameSection currentSection = section;
            if (sectionButtonUI != null)
            {
                sectionButtonUI.onClick.AddListener(() => SetupLevelPanel(currentSection));
            }
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(CreateSections);
        }
    }

    void CreateSections()
    {
        sectionPanelParent.gameObject.SetActive(true);
    }

    void SetupLevelPanel(GameSection sectionData)
    {
        SetGameState(EGameState.LEVELSELECTION);
        ClearLevelPanel();
        CreateLevelPanel(sectionData);

    }
    void SetGameState(EGameState newState)
    {
        gameState = newState;
        GameManager.instance.SetGameState(newState);
    }

    void CreateLevelPanel(GameSection sectionData)
    {
        for (int i = 0; i < sectionData.levels.Length; i++)
        {
            Level level = sectionData.levels[i];
            GameObject levelPanel = Instantiate(levelPanelPrefab, levelPanelParent);
            var levelNameUI = levelPanel.transform.Find("LevelName")?.GetComponent<TextMeshProUGUI>();
            var levelButonUI = levelPanel.transform.Find("LevelButton")?.GetComponent<Button>();

            if (levelNameUI != null) levelNameUI.text = level.name; // veya level'ın göstermek istediğiniz özelliği
            if (levelButonUI != null) levelButonUI.onClick.AddListener(() => LoadLevel(level));
        }
    }
    void LoadLevel(Level levelToLoad)
    {
        pauseMenuButton.SetActive(true);
        ClearLevelPanel();
        sectionPanelParent.gameObject.SetActive(false);
        levelPanelParent.gameObject.SetActive(false);
        
        int sectionIndex = -1;
        int levelIndex = -1;

        GameSection[] sections = LevelManager.Instance.GetAllSections();

        for (int s = 0; s < sections.Length; s++)
        {
            for (int l = 0; l < sections[s].levels.Length; l++)
            {
                if (sections[s].levels[l] == levelToLoad)
                {
                    sectionIndex = s;
                    levelIndex = l;
                    break;
                }
            }
        }

        if (sectionIndex != -1 && levelIndex != -1)
        {
            LevelManager.Instance.SelectLevel(sectionIndex, levelIndex);
            GameManager.instance.SetGameState(EGameState.GAME); 
        }
        else
        {
            Debug.LogWarning("Level is not found in any section.");
        }
    }

    void ClearLevelPanel()
    {
        foreach (Transform child in levelPanelParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void PauseGame()
    {
        /*Time.timeScale = 0;
        pauseMenuUI.SetActive(true);*/
    }
    public void ResumeGame()
    {
        /*Time.timeScale = 1;
        pauseMenuUI.SetActive(false);*/
    }
    public void ExitLevel()
    {
        /*Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        pauseMenuButton.SetActive(false);
        ClearLevelPanel();
        GameManagerOLD.Instance.DestroyCurrentLevel();*/

    }

  
}
