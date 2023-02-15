using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    [SerializeField] private float stackBufferTime = 0.5f;
    [SerializeField] private float fallWaitTime = 1f;
    private float stackTimer = 0f;
    private float fallTimer = 0f;

    private Transform currentPiece;
    public static PieceController Instance;

    
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
        InputManager.Instance.OnMovementRightRecieved += Input_OnMovementRightRecieved;
        InputManager.Instance.OnMovementLeftRecieved += Input_OnMovementLeftRecieved;
        InputManager.Instance.OnMovementDownRecieved += Input_OnMovementDownRecieved;
        InputManager.Instance.OnRotateRecieved += Input_OnRotateRecieved;
        InputManager.Instance.OnMovementStackRecieved += Input_OnMovementStackRecieved;

        SpawnNextPiece();
    }


    private void Update()
    {
        Fall();
        StackPiece();
    }

    private void Input_OnMovementRightRecieved(object sender, EventArgs e)
    {
        MoveHorizontal(+1);
    }
    private void Input_OnMovementLeftRecieved(object sender, EventArgs e)
    {
        MoveHorizontal(-1);
    }
    private void Input_OnRotateRecieved(object sender, EventArgs e)
    {
        RotatePiece();
    }
    private void Input_OnMovementDownRecieved(object sender, EventArgs e)
    {
        MoveDownwards();
    }

    private void Input_OnMovementStackRecieved(object sender, EventArgs e)
    {
        MoveStackDown();
    }

    public void MoveHorizontal(int _input)
    {   Vector3 movePos = new Vector3(_input, 0, 0);
        foreach (Transform block in currentPiece.transform)
        {
            if (!Playfield.Instance.IsValidGridPos(block.position + movePos, currentPiece.transform))
            {
                //Debug.Log("Not valid horizontal");
                return;
            }

        }

        currentPiece.transform.position += movePos;
        Playfield.Instance.UpdatePlayField(currentPiece);
    }

    public void RotatePiece()
    {
        Vector3 movePos = new Vector3(0,0,0);
        foreach (Transform block in currentPiece.transform)
        {
            if (block.localPosition.x > 0)
            {
                movePos = new Vector3(-2, 0, 0);
            }
            else if (block.localPosition.x < 0)
            {
                movePos = new Vector3(+2, 0, 0);
            }
            else
            {
                movePos = new Vector3(0,0,0);
            }
            if (!Playfield.Instance.IsValidGridPos(block.position + movePos, currentPiece.transform))
            {
                //Debug.Log("Not valid rotation");
                return;
            }
        }

        foreach (Transform block in currentPiece.transform)
        {
            if (block.localPosition.x > 0)
            {
                movePos = new Vector3(-2, 0, 0);
            }
            else if (block.localPosition.x < 0)
            {
                movePos = new Vector3(+2, 0, 0);
            }
            else
            {
                movePos = new Vector3(0,0,0);
            }

            block.localPosition += movePos;
        }

        Playfield.Instance.UpdatePlayField(currentPiece);
    }

    public void MoveDownwards()
    {
        Vector3 movePos = new Vector3(0,-1,0);
        foreach (Transform block in currentPiece.transform)
        {
            if (!Playfield.Instance.IsValidGridPos(block.position + movePos, currentPiece))
            {
                //Debug.Log("Not valid down");
                return;
            }
        }

        fallTimer = 0f;
        currentPiece.position += movePos;
        Playfield.Instance.UpdatePlayField(currentPiece);
    }

    public void SpawnNextPiece()
    {
        currentPiece = Spawner.Instance.CreatePiece();
        if (currentPiece.childCount == 0)
        {
            Destroy(currentPiece.gameObject);
            Debug.Log("Skipped 0 piece");
            SpawnNextPiece();
        }
        Playfield.Instance.UpdatePlayField(currentPiece);
    }

    private void Fall()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= fallWaitTime)
        {
            fallTimer = 0f;
            MoveDownwards();
        }
    }

    private void MoveStackDown()
    {
        Vector3 movePos = new Vector3(0,-1,0);

        while(true)
        {
            foreach (Transform block in currentPiece.transform)
            {
                if (!Playfield.Instance.IsValidGridPos(block.position + movePos, currentPiece))
                {
                    //Debug.Log("Not valid down");
                    return;
                }
            }
            MoveDownwards();
            stackTimer = stackBufferTime;
        }

    }

    private void StackPiece()
    {
        if (stackTimer >= stackBufferTime)
        {
            List<Transform> stackList= DisbandPiece(currentPiece);
            foreach(Transform block in stackList)
            {
                Playfield.Instance.DeletePieces(block.position.x, block.position.y);
            }
            DestroyImmediate(currentPiece.gameObject);
            SpawnNextPiece();
            stackTimer = 0f;
        }

        Vector3 movePos = new Vector3(0, -1, 0);
        foreach (Transform block in currentPiece.transform)
        {
            if (!Playfield.Instance.IsValidGridPos(block.position + movePos, currentPiece))
            {
                stackTimer += Time.deltaTime;
                return;
            }
        }
        stackTimer = 0f;
    }

    private List<Transform> DisbandPiece(Transform _piece)
    {   
        List<Transform> disbandList = new List<Transform>();
        foreach (Transform block in _piece)
        {
            disbandList.Add(block);
        }
        foreach (Transform block in disbandList)
        {
            block.parent = null;
        }
        return disbandList;
    }

    public Transform GetCurrentPiece()
    {
        return currentPiece;
    }
}
