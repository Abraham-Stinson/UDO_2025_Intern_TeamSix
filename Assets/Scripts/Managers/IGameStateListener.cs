using UnityEngine;

public interface IGameStateListener
{
  void GameStateChangedCallBack(EGameState gameState);
}
