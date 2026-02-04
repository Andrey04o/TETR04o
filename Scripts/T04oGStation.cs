using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Components;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oGStation : UdonSharpBehaviour
    {
        public VRC.SDK3.Components.VRCStation station;
        public T04oGPCControl pcControls;
        public GameObject textsMovement;
        public T04oGSControls touchControls;
        void Start() {
            if (Networking.LocalPlayer.IsUserInVR()) {
                gameObject.SetActive(false);
            }
        }
        public override void Interact()
        {
            base.Interact();
            station.UseStation(Networking.LocalPlayer);
            pcControls.gameObject.SetActive(true);
            pcControls.main.controlsHandling.SetEnable(true);
            textsMovement.gameObject.SetActive(true);
            touchControls.gameObject.SetActive(true);
            pcControls.main.controls.StartedUsing(Networking.LocalPlayer);
            if (VRC.SDKBase.InputManager.GetLastUsedInputMethod() == VRCInputMethod.Touch) {
                touchControls.ShowTouchScreen(true);
            }
        }

        public void Leave() {
            station.ExitStation(Networking.LocalPlayer);
            pcControls.main.controlsHandling.SetEnable(false);
            pcControls.gameObject.SetActive(false);
            textsMovement.gameObject.SetActive(false);
            touchControls.gameObject.SetActive(false);
            pcControls.main.controls.LeaveControls();
        }

        public void DisableInteractions(bool value) {
            DisableInteractive = value;
        }
    }
}
