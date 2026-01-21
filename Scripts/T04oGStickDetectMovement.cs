using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;

public class T04oGStickDetectMovement : UdonSharpBehaviour
{
    public T04oGStick stick;
    public Transform grabbable;
    public Transform pivot;
    public float invokeRadius = 1f;
    
    float forwardDot;
    float rightDot;
    float upDot;
    public Vector3 positionGrabbableOriginal;
    public Quaternion rotationPivotOriginal;

    public float timeRepeatKey = 0.3f;
    float timer = 0f;
    bool isLeftPressed;
    bool isRightPressed;
    bool isUpPressed;
    bool isDownPressed;
    public bool isDisableRepeatUp = false;
    bool isTouchStick = false;

    void Update()
    {
        CalcDirection();
        ShowVisually();
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
                stick.Left();
            }
            if (isRightPressed) {
                stick.Right();
            }
            if (isUpPressed && isDisableRepeatUp == false) {
                stick.Up();
            }
            if (isDownPressed) {
                stick.Down();
            }
        } else {
            timer = 0;
        }
    }

    void ShowVisually() {
        pivot.LookAt(grabbable);
    }
    public void SetOriginalPosition() {
        grabbable.localPosition = positionGrabbableOriginal;
        pivot.localRotation = rotationPivotOriginal;
    }

    void CalcDirection() {
        if (grabbable == null || stick == null)
            return;

        Vector3 directionToGrabbable = grabbable.localPosition - positionGrabbableOriginal;
        float distance = directionToGrabbable.magnitude;
        
        // Only invoke if within radius
        if (distance < invokeRadius) {
            isLeftPressed = false;
            isRightPressed = false;
            isUpPressed = false;
            isDownPressed = false;
            return;
        }

            
        
        // Get the local direction relative to this object's orientation
        Vector3 localDirection = transform.TransformDirection(directionToGrabbable.normalized);
        
        // Determine which direction has the largest magnitude
        //forwardDot = Vector3.Dot(localDirection, Vector3.forward);
        //rightDot = Vector3.Dot(localDirection, Vector3.right);
        //upDot = Vector3.Dot(localDirection, Vector3.up);
        forwardDot = Vector3.Dot(localDirection, transform.forward);
        rightDot = Vector3.Dot(localDirection, transform.right);
        upDot = Vector3.Dot(localDirection, transform.up);
        
        // Find the dominant direction
        float maxDot = Mathf.Max(Mathf.Abs(forwardDot), Mathf.Abs(rightDot), Mathf.Abs(upDot));
        isTouchStick = false;
        if (maxDot == Mathf.Abs(forwardDot))
        {
            if (forwardDot > 0) {
                if (isUpPressed == false) stick.Up();
                isUpPressed = true;
                isDownPressed = false;
            } else {
                if (isDownPressed == false) stick.Down();
                isDownPressed = true;
                isUpPressed = false;
            }
            isTouchStick = true;
        }
        if (maxDot == Mathf.Abs(rightDot))
        {
            if (rightDot > 0) {
                if (isRightPressed == false) stick.Right();
                isRightPressed = true;
                isLeftPressed = false;
            } else {
                if (isLeftPressed == false) stick.Left();
                isLeftPressed = true;
                isRightPressed = false;
            }
            isTouchStick = true;
        }
        if (isTouchStick == false) {
            isLeftPressed = false;
            isRightPressed = false;
            isUpPressed = false;
            isDownPressed = false;
        } 
    }
}
