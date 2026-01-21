using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Components;
using VRC.SDKBase;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class T04oGSControls : UdonSharpBehaviour
{
    public Canvas canvasTouchScreen;
    public T04oMain main;
    public T04oGStation station;
    public override void OnInputMethodChanged(VRCInputMethod inputMethod)
    {
        base.OnInputMethodChanged(inputMethod);
        if (inputMethod == VRCInputMethod.Touch) {
            ShowTouchScreen(true);
        } else {
            ShowTouchScreen(false);
        }
    }
    public void ShowTouchScreen(bool value) {
        canvasTouchScreen.gameObject.SetActive(value);
    }
    public void Up() {
        main.controls.Up();
    }
    public void Left() {
        main.controls.Left();
    }
    public void Right() {
        main.controls.Right();
    }
    public void Down() {
        main.controls.Down();
    }
    public void RotateLeft() {
        main.controls.RotateLeft();
    }
    public void RotateRight() {
        main.controls.RotateRight();
    }
    public void Hold() {
        main.controls.Respawn();
    }
    public void Exit() {
        station.Leave();
    }
    

}
