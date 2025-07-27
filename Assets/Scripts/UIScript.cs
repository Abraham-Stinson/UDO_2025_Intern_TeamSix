using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIScript : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject sectionPanelPrefab;
    [SerializeField] private Transform sectionPanelParent;
    [SerializeField] private GameObject sectionContainer;
    [Header("Levels")]
    [SerializeField] private GameObject levelPanelPrefab;
    [SerializeField] private Transform levelPanelParent;
    [SerializeField] private GameObject levelContainer;
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private bool isOnGame = true;
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject pauseMenuButton;
    
    void Awake()
    {
        pauseMenuButton.SetActive(false);
        pauseMenuUI.SetActive(false);
    }
    void Start()
    {
        SetupSectionPanel();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SetupSectionPanel()
    {
        sectionContainer.SetActive(true);
        foreach (var section in GameManager.Instance.levelData)
        {
            GameObject sectionPanel = Instantiate(sectionPanelPrefab, sectionPanelParent);
            var sectionIconUI = sectionPanel.transform.Find("SectionIcon")?.GetComponent<Image>();
            var sectionLabelUI = sectionPanel.transform.Find("SectionLabel")?.GetComponent<TextMeshProUGUI>();
            var sectionButtonUI = sectionPanel.transform.Find("SectionButton")?.GetComponent<Button>();
            sectionIconUI.sprite = section.sectionIcon;
            sectionLabelUI.text = section.sectionName;

            LevelData currentSection = section;
            sectionButtonUI.onClick.AddListener(() => SetupLevelPanel(currentSection));
        }

        backButton.onClick.AddListener(CreateSections);
    }

    void CreateSections()
    {
        levelContainer.SetActive(false);
        sectionPanelParent.gameObject.SetActive(true);
    }

    void SetupLevelPanel(LevelData sectionData)
    {
        levelContainer.SetActive(true);
        ClearLevelPanel();
        CreateLevelPanel(sectionData);

    }

    void CreateLevelPanel(LevelData sectionData)
    {
        for (int i = 0; i < sectionData.levels.Length; i++)
        {
            Levels level = sectionData.levels[i];
            GameObject levelPanel = Instantiate(levelPanelPrefab, levelPanelParent);
            var levelNameUI = levelPanel.transform.Find("LevelName")?.GetComponent<TextMeshProUGUI>();
            var levelButonUI = levelPanel.transform.Find("LevelButton")?.GetComponent<Button>();
            levelNameUI.text = level.levelName;
            levelButonUI.onClick.AddListener(() => GameManager.Instance.LoadLevel(level));

            Levels currentLevel = level;
            levelButonUI.onClick.AddListener(() => LoadLevel(currentLevel));

        }
    }
    void LoadLevel(Levels levelToLoad)
    {
        pauseMenuButton.SetActive(true);
        levelContainer.SetActive(false);
        sectionContainer.SetActive(false);
        //pauseMenuIcon.SetActive(true);

        

        //isOnGame = true;//for pause menu MAYBE
        ClearLevelPanel();
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
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
    }
    public void ExitLevel()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        levelContainer.SetActive(false);
        sectionContainer.SetActive(true);
        pauseMenuButton.SetActive(false);
        ClearLevelPanel();
        GameManager.Instance.DestroyCurrentLevel();

    }
}
