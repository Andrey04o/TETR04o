using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.Udon.Common;

public class T04oGPCControl : UdonSharpBehaviour
{
    public T04oMain main;
    public T04oGStation station;
    public float timeRepeatKey = 0.3f;
    float timer = 0f;
    bool isLeftPressed;
    bool isRightPressed;
    bool isUpPressed;
    bool isDownPressed;
    public bool isDisableRepeatUp = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        RepeatKeys();
    }
    void RepeatKeys() {
        if (isLeftPressed || isRightPressed || isUpPressed || isDownPressed) {
            timer += Time.deltaTime;
            if (timer < timeRepeatKey) {
                return;
            }
            timer = 0f;
            if (isLeftPressed) {
                main.controls.Left();
            }
            if (isRightPressed) {
                main.controls.Right();
            }
            if (isUpPressed && isDisableRepeatUp == false) {
                main.controls.Up();
            }
            if (isDownPressed) {
                main.controls.Down();
            }
        } else {
            timer = 0;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            isLeftPressed = true;
            main.controls.Left();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            isRightPressed = true;
            main.controls.Right();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            isDownPressed = true;
            main.controls.Down();
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            isUpPressed = true;
            main.controls.Up();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            isLeftPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            isRightPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            isDownPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            isUpPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            main.controls.RotateLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            main.controls.RotateRight();
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F)) {
            main.controls.Respawn();
        }
        if (Input.GetMouseButtonDown(1)) {
            if (VRC.SDKBase.InputManager.GetLastUsedInputMethod() == VRC.SDKBase.VRCInputMethod.Touch) {
                return;
            }
            station.Leave();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            main.controls.SpaceBar();
        }
    }
    /*
    public override void InputDrop(bool value, UdonInputEventArgs args)
    {
        station.Leave();
    }
    */
}
