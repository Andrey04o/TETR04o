using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.Udon.Common;
namespace TETR04o {
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
            if (main.controls.isInterface) {
                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexLefts))
                {
                    isLeftPressed = true;
                    main.controls.Left();
                }
                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexRights))
                {
                    isRightPressed = true;
                    main.controls.Right();
                }

                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexDowns))
                {
                    isDownPressed = true;
                    main.controls.Down();
                }

                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexUps))
                {
                    isUpPressed = true;
                    main.controls.Up();
                }
                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexLefts))
                {
                    isLeftPressed = false;
                }

                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexRights))
                {
                    isRightPressed = false;
                }

                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexDowns))
                {
                    isDownPressed = false;
                }

                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexUps))
                {
                    isUpPressed = false;
                }
                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexConfirm))
                {
                    main.controls.RotateLeft();
                }
            } else {
                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexMoveLeft))
                {
                    isLeftPressed = true;
                    main.controls.Left();
                }
                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexMoveRight))
                {
                    isRightPressed = true;
                    main.controls.Right();
                }

                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexSoftDrops))
                {
                    isDownPressed = true;
                    main.controls.Down();
                }

                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexMoveLeft))
                {
                    isLeftPressed = false;
                }

                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexMoveRight))
                {
                    isRightPressed = false;
                }

                if (T04oHotkeys.CheckButtonUp(main.hotkeys.indexSoftDrops))
                {
                    isDownPressed = false;
                }


                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexRotateLefts))
                {
                    main.controls.RotateLeft();
                }

                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexRotateRights))
                {
                    main.controls.RotateRight();
                }

                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexHolds)) {
                    main.controls.Respawn();
                }
                if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexHardDrops)) {
                    main.controls.SpaceBar();
                }
            }
            
            
            if (T04oHotkeys.CheckButtonDown(main.hotkeys.indexExits)) {
                if (VRC.SDKBase.InputManager.GetLastUsedInputMethod() == VRC.SDKBase.VRCInputMethod.Touch) {
                    return;
                }
                station.Leave();
            }
            
        }

        public void UnholdButtons() {
            isDownPressed = false;
            isLeftPressed = false;
            isRightPressed = false;
            isUpPressed = false;
        }
        /*
        public override void InputDrop(bool value, UdonInputEventArgs args)
        {
            station.Leave();
        }
        */
    }
}