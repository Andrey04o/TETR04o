using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
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
            if (main.controls.isInterface) {
                main.controls.RotateLeft();
                return;
            }
            if (isRotateLeft) {
                main.controlsHandling.PressLeftRotate();
            }
            if (isRotateRight) {
                main.controlsHandling.PressRightRotate();
            }
            if (isButtonSwap) {
                main.controlsHandling.PressHold();
            }
        }
    }
}