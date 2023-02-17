using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    pause,
    inGame,
    gameOver
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
    public GameState currentGameState = GameState.inGame;
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
        Playfield.Instance.OnFlashAnimationStarted += Playfield_OnFlashAnimationStarted;
        Playfield.Instance.OnFlashAnimationEnded += Playfield_OnFlashAnimationEnded;
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
}
