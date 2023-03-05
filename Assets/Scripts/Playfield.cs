using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScoreArgs : EventArgs
{
    public int PiecesBurnt { get; set; }
    public int PiecesCombo { get; set; }
}

public class Playfield : MonoBehaviour
{
    [SerializeField] static int playfieldWidht = 7;
    [SerializeField] static int playfieldHeight = 12;
    private Transform[,] objectGrid = new Transform[playfieldWidht, playfieldHeight];
    private int[,] blockCodeGrid = new int[playfieldWidht, playfieldHeight];
    private List<Vector3> directionList = new List<Vector3>() {new Vector3(1,0,0), new Vector3(-1,0,0), new Vector3(0,1,0), new Vector3(0,-1,0)};
    public static Playfield Instance;

    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    private IEnumerator currentCoroutine = null;
    private int piecesCombo = 1;

    public EventHandler<ScoreArgs> OnDestroyPieces;
    public EventHandler OnFlashAnimationStarted;
    public EventHandler OnFlashAnimationEnded;
    public EventHandler OnGameOver;

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

    private void Start() {
        GameModeManager.Instance.StartGame();
    }

    private void ResetCombo()
    {
        piecesCombo = 1; 
    }

    public static Vector2 roundVec2(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }

    public static bool IsInsideBorders(Vector2 position)
    {
        return ((int) position.x >= 0 && (int) position.x < playfieldWidht && (int) position.y > 0);
    }

    public bool DeletePieces(float _xPos, float _yPos)
    {
        int xPos = (int)_xPos;
        int yPos = (int)_yPos;
        List<Transform> piecesToDestroy = new List<Transform>();
        CheckPiecesToDestroy(xPos, yPos, piecesToDestroy);

        //Debug.Log(piecesToDestroy.Count);    
        if (piecesToDestroy.Count >= 4)
        {   
            ScoreArgs scoreArgs = new ScoreArgs();
            scoreArgs.PiecesBurnt = piecesToDestroy.Count;
            scoreArgs.PiecesCombo = piecesCombo;
            OnDestroyPieces?.Invoke(this, scoreArgs);

            // if (coroutineQueue.Count == 0)
            // {
            //     currentCoroutine = FlashDestroyPieces(piecesToDestroy);
            //     StartCoroutine(currentCoroutine);

            // }
            // else
            // {
                currentCoroutine = FlashDestroyPieces(piecesToDestroy);
                StartCoroutine(currentCoroutine);
                coroutineQueue.Enqueue(currentCoroutine);
            //}


            piecesCombo+=1;

            return true;
        }

        return false;

    }

    private List<Transform> CheckPiecesToDestroy(int xPos, int yPos, List<Transform> _piecesToDestroy)
    {
        if (objectGrid[xPos, yPos] != null && blockCodeGrid[xPos, yPos] != 0)
        {
            foreach(Vector3 dir in directionList)
            {
                if(xPos + (int)dir.x >= playfieldWidht || yPos + (int)dir.y >= playfieldHeight
                || xPos + (int)dir.x <0                || yPos + (int)dir.y < 0)
                {
                    continue;
                }
                if (blockCodeGrid[xPos + (int)dir.x, yPos + (int)dir.y] == blockCodeGrid[xPos, yPos])
                {
                    if(_piecesToDestroy.Contains(objectGrid[xPos + (int)dir.x, yPos + (int)dir.y]))
                    {
                        continue;
                    }
                    _piecesToDestroy.Add(objectGrid[xPos + (int)dir.x, yPos + (int)dir.y]);
                    _piecesToDestroy = CheckPiecesToDestroy(xPos + (int)dir.x, yPos + (int)dir.y, _piecesToDestroy);                    
                }
            }
        }
        return _piecesToDestroy;   
    }

    private void DecreasePieces()
    {
        bool isPieceDeleted = false;
        Vector3 movePos = new Vector3(0,-1,0);
        for (int y = 0; y < playfieldHeight; ++y)
            for (int x = 0; x < playfieldWidht; ++x)
                if (objectGrid[x, y] != null && objectGrid[x, y].parent != PieceController.Instance.GetCurrentPiece())
                {
                    Transform block = objectGrid[x,y];
                    if (IsValidGridPos(block.position + movePos, block))
                    {
                        objectGrid[x, y] = null;
                        blockCodeGrid[x, y] = 0;

                        block.position += movePos;

                        int newx = (int)block.position.x;
                        int newy = (int)block.position.y;

                        objectGrid[newx, newy] = block;
                        blockCodeGrid[newx, newy] = block.GetComponent<Block>().GetBlockCode();
                    }
                }

        for (int y = 0; y < playfieldHeight; ++y)
            for (int x = 0; x < playfieldWidht; ++x)
                if (objectGrid[x, y] != null && objectGrid[x, y].parent != PieceController.Instance.GetCurrentPiece())
                {
                    isPieceDeleted |= DeletePieces(x, y);
                }       

        if (!isPieceDeleted)
        {
            ResetCombo();
            //Debug.Log("ResetCombo!");
        }
    }


    public bool IsValidGridPos(Vector2 newPosition, Transform pieceTransform)
    {
        Vector2 blockPosition = Playfield.roundVec2(newPosition);
        if (!Playfield.IsInsideBorders(blockPosition))
        {
            //Debug.Log("Not in playfield :" + blockPosition.y );
            return false;
        }

        if (objectGrid[(int)blockPosition.x, (int)blockPosition.y] != null &&
            objectGrid[(int)blockPosition.x, (int)blockPosition.y].parent != pieceTransform)
        {
            //Debug.Log("Pieza debajo");
            return false;
        } 
        
        return true;
    }

    public void UpdatePlayField(Transform _piece)
    {
        for (int y = 0; y < playfieldHeight; ++y)
            for (int x = 0; x < playfieldWidht; ++x)
                if (objectGrid[x, y] != null)
                {
                    if (objectGrid[x, y].parent == _piece)
                    {
                        objectGrid[x, y] = null;
                        blockCodeGrid[x, y] = 0;
                    }
                }

        foreach (Transform block in _piece)
        {
            Vector2 blockPosition = Playfield.roundVec2(block.position);

            int x = (int)blockPosition.x;
            int y = (int)blockPosition.y;
            objectGrid[x,y] = block;
            blockCodeGrid[x,y] = block.GetComponent<Block>().GetBlockCode();
        }
        GameOver(_piece);     
    }

    public bool GameOver(Transform _piece)
    {   
        int maxY = playfieldHeight-2;
        for (int x = 0; x < playfieldWidht; ++x)
        {
            if (objectGrid[x,maxY] != null && objectGrid[x,maxY].parent != _piece)
            {
                OnGameOver?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }
        
        return false;
    }

    private IEnumerator FlashDestroyPieces(List<Transform> piecesList)
    {       
        foreach (Transform _block in piecesList)
        {
            _block.GetComponent<Animator>().SetTrigger("Flashing");
            objectGrid[(int)_block.position.x, (int)_block.position.y] = null;
            blockCodeGrid[(int)_block.position.x , (int)_block.position.y] = 0;
        }

        OnFlashAnimationStarted?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSecondsRealtime(1f);
        coroutineQueue.Dequeue();

        if (coroutineQueue.Count == 0)
        {
            OnFlashAnimationEnded?.Invoke(this, EventArgs.Empty);
        }

        foreach(Transform _block in piecesList)
        {   
            Destroy(_block.gameObject);
        }
        DecreasePieces();

    }
}
