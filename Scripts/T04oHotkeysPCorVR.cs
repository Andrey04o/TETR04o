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
            vrSettings.SetActive(false);
            pcSettings.SetActive(false);
            if (Networking.LocalPlayer.IsUserInVR()) {
                vrSettings.SetActive(true);
            } else {
                pcSettings.SetActive(true);
            }
        }

    }
}
