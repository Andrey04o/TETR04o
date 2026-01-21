using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    public class T04oGStick : UdonSharpBehaviour
    {
        public T04oMain main;
        public Transform pivotRotation;
        public T04oGStickDetectMovement detectMovement;
        public void ActivateStickDetectionMovement() {
            detectMovement.gameObject.SetActive(true);
        }
        public void DeactivateStickDetectionMovement() {
            detectMovement.gameObject.SetActive(false);
            detectMovement.SetOriginalPosition();
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

    }
}
