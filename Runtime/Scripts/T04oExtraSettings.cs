using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oExtraSettings : UdonSharpBehaviour
    {
        public bool is_PC_User = true;
        public GameObject windowSetting;
        public T04oHotkeysPCorVR hotkeysPCorVR;
        public T04oGStation[] stations;
        public void ToggleVRMod() {
            is_PC_User = !is_PC_User;
            hotkeysPCorVR.ShowKeyboardSettings(is_PC_User);
            foreach (T04oGStation station in stations) {
                station.gameObject.SetActive(is_PC_User);
            }
        }

        void Start() {
            if (Networking.LocalPlayer.IsUserInVR()) {
                windowSetting.SetActive(true);
                is_PC_User = false;
            } else {
                is_PC_User = true;
                windowSetting.SetActive(false);
            }
        }
    }
    
}
