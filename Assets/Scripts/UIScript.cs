using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIScript : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject sectionPanelPrefab;
    [SerializeField] private Transform sectionPanelParent;
    [Header("Levels")]
    [SerializeField] private GameObject levelPanelPrefab;
    [SerializeField] private Transform levelPanelParent;
    [SerializeField] private GameObject levelContainer;
    [Header("Navigation")]
    [SerializeField] private Button backButton;
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

        backButton.onClick.AddListener(ShowSections);
    }

    void ShowSections()
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
        GameObject currentLevelInstance = Instantiate(levelToLoad.levelPrefab);

        ClearLevelPanel();
        levelContainer.SetActive(false);
    }

    void ClearLevelPanel()
    {
        foreach (Transform child in levelPanelParent)
        {
            Destroy(child.gameObject);
        }
    }
}
