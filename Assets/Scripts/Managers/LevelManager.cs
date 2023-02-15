using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    private float levelScore = 0f;
    private int clearedLines = 0;
    private float elapsedSeconds = 0f;
    private int elapsedMinutes = 0;
    private Vector2Int actualTime = new Vector2Int(0,0);
    private Vector2Int previusTime = new Vector2Int(0,0);
    private int actualLevel = 0;
    private float difficultyScoreMultiplier = 1f;
    private float comboScoreMultiplier = 1f;
    public event EventHandler<Vector3Int> OnLineCleared;
    public event EventHandler<Vector2Int> OnTimeElapsed;

    private int fourBlockScore = 10;
    private int fiveBlockScore = 100;
    private int sixOrMoreBlockScore = 300;

    private void Awake() 
    {
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
        Playfield.Instance.OnDestroyPieces += Playfield_OnDestroyPieces;
    }

    private void Update() 
    {
        UpdateTime();
    }

    private void Playfield_OnDestroyPieces(object sender, int _lines)
    {
        clearedLines += _lines;
        Debug.Log(clearedLines);

        if (_lines == 4)
        {
            levelScore += fourBlockScore * difficultyScoreMultiplier * comboScoreMultiplier;
        }
        else if (_lines == 5)
        {
            levelScore += fiveBlockScore * difficultyScoreMultiplier * comboScoreMultiplier;
        }
        else if (_lines >= 6)
        {
            levelScore += sixOrMoreBlockScore * (_lines - 5) * difficultyScoreMultiplier * comboScoreMultiplier;
        }

        Debug.Log(levelScore);
        OnLineCleared?.Invoke(this, new Vector3Int((int)levelScore, clearedLines, actualLevel));
    }


    private void UpdateTime()
    {
        elapsedSeconds += Time.deltaTime;

        if (elapsedSeconds >= 60)
        {
            elapsedMinutes += 1;
            elapsedSeconds -=60;
        }

        previusTime = actualTime;
        actualTime = new Vector2Int(elapsedMinutes, (int)elapsedSeconds);

        if(actualTime != previusTime)
        {
            OnTimeElapsed?.Invoke(this, actualTime);
        }
    }
}
