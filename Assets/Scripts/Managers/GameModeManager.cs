using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState
{
    pause,
    inGame,
    gameOver,
    mainMenu
}

public enum GameMode
{
    None,
    marathon,
    clearBlocks,
    timed
}

public class GameModeManager : MonoBehaviour
{
    public GameState currentGameState = GameState.mainMenu;
    public static GameModeManager Instance;

    
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        InputManager.Instance.OnAnyKeyPressed += InputManager_LeaveMainMenu;
    }


    private void Playfield_OnFlashAnimationEnded(object sender, EventArgs e)
    {
        currentGameState = GameState.inGame;
        Time.timeScale = 1;
        //Debug.Log("UnPause");
        
    }

    private void Playfield_OnFlashAnimationStarted(object sender, EventArgs e)
    {
        currentGameState = GameState.pause;
        Time.timeScale = 0;
        //Debug.Log("Pause");
    }

    private void Playfield_OnGameOver(object sender, EventArgs e)
    {
        currentGameState = GameState.gameOver;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {

        Playfield.Instance.OnFlashAnimationStarted += Playfield_OnFlashAnimationStarted;
        Playfield.Instance.OnFlashAnimationEnded += Playfield_OnFlashAnimationEnded;

        Playfield.Instance.OnGameOver += Playfield_OnGameOver;

        currentGameState = GameState.inGame;
    }

    private void InputManager_LeaveMainMenu(object sender, EventArgs e)
    {
        SceneManager.LoadScene(1);
    }

    
}
