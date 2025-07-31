using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    //[SerializeField] private GameObject moreOptionPanel;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI(GameObject target)
    {
        bool isPanelOpen = target.activeSelf;
        target.SetActive(!isPanelOpen);
    }
}
