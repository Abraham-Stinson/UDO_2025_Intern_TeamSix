using UnityEngine;

public class UIManager : MonoBehaviour,IGameStateListener
{
    [Header("Panels")] 
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject sectionSelectionPanel;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject pasueMenuPanel;
    [SerializeField] private GameObject pauseButton;

  public void GameStateChangedCallBack(EGameState gameState)
  {
    gamePanel.SetActive(gameState == EGameState.GAME);
    levelCompletePanel.SetActive(gameState == EGameState.LEVELCOMPLETE);
    gameOverPanel.SetActive(gameState == EGameState.GAMEOVER);
    sectionSelectionPanel.SetActive(gameState == EGameState.SECTIONSELECTION);
    levelSelectionPanel.SetActive(gameState == EGameState.LEVELSELECTION);
    pasueMenuPanel.SetActive(gameState == EGameState.PAUSEMENU);
    pauseButton.SetActive(gameState == EGameState.PAUSEBUTTON);
  }
}
