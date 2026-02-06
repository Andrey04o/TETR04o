using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oControlsHandling : UdonSharpBehaviour
    {
        public T04oMain main;
        float timer_arr = 0f;
        float timer_das = 0f;
        float timer_dcd = 0f;
        float timer_sdf = 0f;
        bool isLeftPressed;
        bool isRightPressed;
        //bool isHardDropPressed;
        bool isSoftDropPressed;

        void Update()
        {
            ProcessSoftDrop();
            ProcessMoving();
        }

        public void SetEnable(bool value) {
            gameObject.SetActive(value);
        }

        void ProcessSoftDrop() {
            if (isSoftDropPressed == false) return;
            if (timer_sdf > 0) {
                timer_sdf -= Time.deltaTime;
            } else {
                main.controls.Down();
                timer_sdf = main.handling.sdf;
            }
        }
        bool ItCanRepeatMoving() {
            bool value = true;
            if (timer_das > 0) {
                timer_das -= Time.deltaTime;
                value = false;
            }
            if (timer_dcd > 0) {
                timer_dcd -= Time.deltaTime;
                value = false;
            }
            return value;
        }
        void ProcessMoving() {
            if ((isLeftPressed || isRightPressed) == false) return;
            if (ItCanRepeatMoving() == false) return;
            if (timer_arr > 0) {
                timer_arr -= Time.deltaTime;
            } else {
                if (isLeftPressed) {
                    main.controls.Left();
                }
                if (isRightPressed) {
                    main.controls.Right();
                }
                timer_arr = main.handling.arr;
            }
        }
        public void ProcessUnholdButtonsLeftRight() {
            if ((isLeftPressed || isRightPressed) == false) {
                timer_arr = main.handling.arr;
                timer_das = main.handling.das;
            }
        }
        public void ProcessUnholdButtonSoftDrop() {
            timer_sdf = main.handling.sdf;
        }
        public void SetDcd() {
            Debug.Log("setDCD");
            timer_dcd = main.handling.dcd;
        }
        public void ProcessRemoveDcd() {
            Debug.Log("removeDCD");
            timer_dcd = 0;
        }
        public void PressLeft() {
            main.controls.Left();
            isLeftPressed = true;
            isRightPressed = false;
            ProcessRemoveDcd();
        }
        public void PressRight() {
            main.controls.Right();
            isRightPressed = true;
            isLeftPressed = false;
            ProcessRemoveDcd();
        }
        public void PressSoftDrop() {
            main.controls.Down();
            isSoftDropPressed = true;
        }
        public void UnpressLeft() {
            isLeftPressed = false;
            ProcessUnholdButtonsLeftRight();
        }
        public void UnpressRight() {
            isRightPressed = false;
            ProcessUnholdButtonsLeftRight();
        }
        public void UnpressSoftDrop() {
            isSoftDropPressed = false;
            ProcessUnholdButtonSoftDrop();
        }
        public void PressHardDrop() {
            main.controls.SpaceBar();
            //SetDcd(); // need to replace from place
        }
        public void PressLeftRotate() {
            main.controls.RotateLeft();
            //SetDcd(); need set das after wallkick
        }
        public void PressRightRotate() {
            main.controls.RotateRight();
            //SetDcd();
        }
        public void PressHold() {
            main.controls.Respawn();
        }

    }
}
