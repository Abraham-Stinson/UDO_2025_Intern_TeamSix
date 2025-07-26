using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public EGameState gameState;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
     SetGameState(EGameState.MENU);
    }


    public void SetGameState(EGameState gameState)
    {
        this.gameState = gameState;
        IEnumerable<IGameStateListener> gameStateListeners
            = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IGameStateListener>();

        foreach (IGameStateListener dependency in gameStateListeners)
        {
            dependency.GameStateChangedCallBack(gameState);
        }
    }

    public void StartGame()
    {
        SetGameState(EGameState.GAME);
    }

    public void NextButtonCallBack()
    {
        SceneManager.LoadScene(0);
    }

    public void RetryButtonCallBack()
    {
        SceneManager.LoadScene(0);

    }
    

    public bool IsGame() => gameState == EGameState.GAME;
}
