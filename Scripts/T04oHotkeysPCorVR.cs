using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC;
using VRC.SDKBase;
using VRC.SDK3.Persistence;
namespace TETR04o {
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oHotkeysPCorVR : UdonSharpBehaviour
    {
        public GameObject pcSettings;
        public GameObject vrSettings;
        void Start()
        {
            if (Networking.LocalPlayer.IsUserInVR()) {
                ShowKeyboardSettings(false);
            } else {
                ShowKeyboardSettings(true);
            }
        }
        public void ShowKeyboardSettings(bool value) {
            vrSettings.SetActive(!value);
            pcSettings.SetActive(value);
        }

    }
}
