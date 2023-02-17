using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    private float levelScore = 0f;
    private int clearedLines = 0;
    private int levelClearedLines = 0;
    private float elapsedSeconds = 0f;
    private int elapsedMinutes = 0;
    private Vector2Int actualTime = new Vector2Int(0,0);
    private Vector2Int previusTime = new Vector2Int(0,0);
    private const int _MAXLEVEL = 9;
    private int selectedDifficultyLevel = 0;
    private int currentDifficultyLevel = 0;
    private float difficultyScoreMultiplier = 1f;
    private float comboScoreMultiplier = 1f;
    public event EventHandler<Vector3Int> OnLineCleared;
    public event EventHandler<Vector2Int> OnTimeElapsed;
    public event EventHandler<int> OnDifficultyLevelIncreased;

    private int blocksToLevelUp = 20;
    private int blocksToNextLevel;

    private int fourBlockScore = 40;
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
        currentDifficultyLevel = selectedDifficultyLevel;
        difficultyScoreMultiplier = (currentDifficultyLevel + 1);
        blocksToNextLevel = (currentDifficultyLevel + 1) * blocksToLevelUp;
    }

    private void Update() 
    {
        UpdateTime();
    }

    private void Playfield_OnDestroyPieces(object sender, ScoreArgs _scoreArgs)
    {
        int _lines = _scoreArgs.PiecesBurnt;
        comboScoreMultiplier = _scoreArgs.PiecesCombo;
        
        clearedLines += _lines;
        levelClearedLines += _lines;
        //Debug.Log(clearedLines);

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

        LevelUpDifficulty();

        Debug.Log(comboScoreMultiplier);
        OnLineCleared?.Invoke(this, new Vector3Int((int)levelScore, clearedLines, currentDifficultyLevel));
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

    private void LevelUpDifficulty()
    {
        if(currentDifficultyLevel >= _MAXLEVEL)
        {
            return;
        }

        blocksToNextLevel = (currentDifficultyLevel + 1) * blocksToLevelUp;

        
        if(levelClearedLines >= blocksToNextLevel)
        {
            currentDifficultyLevel += 1;
            levelClearedLines -= blocksToNextLevel;
            Debug.Log("LevelUp");
        }

        OnDifficultyLevelIncreased?.Invoke(this, currentDifficultyLevel);
    }
    
}
