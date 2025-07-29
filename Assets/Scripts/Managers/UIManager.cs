using UnityEngine;

public class UIManager : MonoBehaviour,IGameStateListener
{
    [Header("Panels")] 
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject slotPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject sectionSelectionPanel;
    [SerializeField] private GameObject levelSelectionPanel;

  public void GameStateChangedCallBack(EGameState gameState)
  {
    menuPanel.SetActive(gameState == EGameState.MENU);
    gamePanel.SetActive(gameState == EGameState.GAME);
    slotPanel.SetActive(gameState == EGameState.GAME);
    levelCompletePanel.SetActive(gameState == EGameState.LEVELCOMPLETE);
    gameOverPanel.SetActive(gameState == EGameState.GAMEOVER);
    sectionSelectionPanel.SetActive(gameState == EGameState.SECTIONSELECTION);
    levelSelectionPanel.SetActive(gameState == EGameState.LEVELSELECTION);
  }
}
