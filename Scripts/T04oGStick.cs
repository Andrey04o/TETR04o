using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oGStick : UdonSharpBehaviour
    {
        public T04oMain main;
        public Transform pivotRotation;
        public T04oGStickDetectMovement detectMovement;
        public void ActivateStickDetectionMovement() {
            main.controlsHandling.SetEnable(true);
            detectMovement.gameObject.SetActive(true);
        }
        public void DeactivateStickDetectionMovement() {
            main.controlsHandling.SetEnable(false);
            detectMovement.gameObject.SetActive(false);
            detectMovement.SetOriginalPosition();
        }

        public void Up() {
            if (main.controls.isInterface)
                main.controls.Up();
            else
                main.controlsHandling.PressHardDrop();
        }

        public void Left() {
            if (main.controls.isInterface)
                main.controls.Left();
            else
                main.controlsHandling.PressLeft();
        }

        public void Right() {
            if (main.controls.isInterface)
                main.controls.Right();
            else
                main.controlsHandling.PressRight();
        }

        public void Down() {
            if (main.controls.isInterface)
                main.controls.Down();
            else
                main.controlsHandling.PressSoftDrop();
        }
        public void UnUp() {

        }
        public void UnLeft() {
            main.controlsHandling.UnpressLeft();
        }
        public void UnRight() {
            main.controlsHandling.UnpressRight();
        }
        public void UnDown() {
            main.controlsHandling.UnpressSoftDrop();
        }

    }
}
