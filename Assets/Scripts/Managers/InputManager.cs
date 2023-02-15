using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public static InputManager Instance {get; private set;}
    public event EventHandler OnMovementRightRecieved;
    public event EventHandler OnMovementLeftRecieved;
    public event EventHandler OnRotateRecieved;
    public event EventHandler OnMovementDownRecieved;
    public event EventHandler OnMovementStackRecieved;

    private float movementTimer = 0.2f;
    private bool isRightPressed = false;
    private float rightTimer = 0f;
    private bool isLeftPressed = false;
    private float leftTimer = 0f;
    private bool isDownPressed = false;
    private float downTimer = 0f;


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

    private void Update() 
    {
        if (GameModeManager.Instance.currentGameState != GameState.pause)
        {
            PieceMovement();
        }
    }

    public void PieceMovement()
    {
        if (Input.GetButtonDown("Stack"))
        {
            OnMovementStackRecieved?.Invoke(this, EventArgs.Empty);
        }
        else if (Input.GetButton("Right"))
        {
            if (!isRightPressed)
            {
                OnMovementRightRecieved?.Invoke(this, EventArgs.Empty);
                isRightPressed = true;
            }
            else
            {
                rightTimer += Time.deltaTime;
                if (rightTimer >= movementTimer)
                {
                    rightTimer = 0;
                    isRightPressed = false;
                }
            }
            
        }
        else if (Input.GetButton("Left"))
        {
            if (!isLeftPressed)
            {
                OnMovementLeftRecieved?.Invoke(this, EventArgs.Empty);
                isLeftPressed = true;
            }
            else
            {
                leftTimer += Time.deltaTime;
                if (leftTimer >= movementTimer)
                {
                    leftTimer = 0;
                    isLeftPressed = false;
                }
            }
        }
        else if (Input.GetButtonDown("Rotate"))
        {
            OnRotateRecieved?.Invoke(this, EventArgs.Empty);
        }
        else if (Input.GetButton("Down"))
        {

            if (!isDownPressed)
            {
                OnMovementDownRecieved?.Invoke(this, EventArgs.Empty);
                isDownPressed = true;
            }
            else
            {
                downTimer += Time.deltaTime;
                if (downTimer >= movementTimer)
                {
                    downTimer = 0;
                    isDownPressed = false;
                }
            }
        }
        else
        {
            rightTimer = 0;
            isRightPressed = false;

            leftTimer = 0;
            isLeftPressed = false;

            downTimer = 0;
            isDownPressed = false;
        }

    }

    IEnumerator BufferInput()
    {
        yield return new WaitForSeconds(1f);
    }

}
