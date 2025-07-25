using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject sectionPanelPrefab;
    [SerializeField] private Transform sectionPanelParent;
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
            sectionIconUI.sprite = section.sectionIcon;
            sectionLabelUI.text = section.sectionName;
        
        }
    }
}
