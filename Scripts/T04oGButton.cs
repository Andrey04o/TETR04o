using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    public class T04oGButton : UdonSharpBehaviour
    {
        public T04oMain main;
        public bool isRotateLeft = false;
        public bool isRotateRight = false;
        public bool isButtonSwap = false;
        public override void Interact()
        {
            base.Interact();
            main.SetOwnerIfNot();
            main.resizerButton.StartTheTimerIfSizeDifferent();
            if (isRotateLeft) {
                main.controls.RotateLeft();
            }
            if (isRotateRight) {
                main.controls.RotateRight();
            }
            if (isButtonSwap) {
                main.controls.Respawn();
            }
        }
    }
}