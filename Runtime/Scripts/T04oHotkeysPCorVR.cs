using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
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
