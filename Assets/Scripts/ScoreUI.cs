using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] Sprite nullImage;
    [SerializeField] List<Sprite> numberList = new List<Sprite>();
    [SerializeField] List<Image> scoreBoard = new List<Image>();
    [SerializeField] List<Image> blocksBoard = new List<Image>();
    [SerializeField] List<Image> timeBoard = new List<Image>();
    [SerializeField] List<Image> levelBoard = new List<Image>();

    public static ScoreUI Instance;
    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one InputManager! " + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        LevelManager.Instance.OnLineCleared += LevelManager_OnLineCleared;
        LevelManager.Instance.OnTimeElapsed += LevelManager_OnTimeElapsed;

        RemoveImages(scoreBoard);
        RemoveImages(blocksBoard);
        RemoveImages(levelBoard);
    }


    private void RemoveImages(List<Image> _imageList)
    {
        for (int i = 1; i < _imageList.Count; i++)
        {
            _imageList[i].sprite = nullImage;
        }
    }

    private void LevelManager_OnLineCleared(object sender, Vector3Int args)
    {
        string displayScore = args.x.ToString();
        string displayBlocks = args.y.ToString();
        string displayLevel = args.z.ToString();
        
        UpdateUI(displayScore, displayBlocks, displayLevel);
    }

    private void LevelManager_OnTimeElapsed(object sender, Vector2Int args)
    {
        int seconds = args.y;
        string secondsString = seconds.ToString();
        
        if(seconds < 10)
        {
            secondsString = "0" + secondsString;
        }

        string minutesSeconds = args.x.ToString() + secondsString;
        
        UpdateSingleUIElement(minutesSeconds, ref timeBoard);        
    }

    private List<int> GetIntsFromString(string str)
    {
        List<int> ints = new List<int>();
        
        string[] slpitString = str.Split();

        foreach (string item in slpitString)
        {
            ints.Add(Convert.ToInt32(item));
        }
        return ints;
    }

    private void UpdateUI(string _score, string _blocks, string _level)
    {
        UpdateSingleUIElement(_score, ref scoreBoard);
        UpdateSingleUIElement(_blocks, ref blocksBoard);
        UpdateSingleUIElement(_level, ref levelBoard);
    }

    private void UpdateSingleUIElement(string _intUpdate, ref List<Image> _imageList)
    {
        int position = _intUpdate.Length-1;
        if(_intUpdate.Length > _imageList.Count)
        {
            _intUpdate = "";
            
            foreach(Image im in _imageList)
            {
                _intUpdate += "9";
            }
            position = _intUpdate.Length-1;
        }
        foreach (char c in _intUpdate)
        {
            if(position < 0)
            {
                Debug.Log("Error: Position < 0");
                return;
            }

            int number = (int)(c - '0');

            _imageList[position].sprite = numberList[number];

            position --;
        }
    }
}
