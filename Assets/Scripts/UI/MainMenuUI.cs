using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject[] allMoreOptionPanels;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI(GameObject target)
    {
        foreach (var panel in allMoreOptionPanels)
        {
            if (panel != target)
            {
                panel.SetActive(false);
            }

        }
        bool isPanelOpen = target.activeSelf;
        target.SetActive(!isPanelOpen);
    }
}
