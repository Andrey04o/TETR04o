using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.SDK3.Persistence;
namespace TETR04o {
    public enum HandlingType {
        arr,
        das,
        dcd,
        sdf
    }
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oHandling : UdonSharpBehaviour
    {
        public string nameForSavePersistance = "T04oH";
        public T04oSliderSetting slider_arr;
        public T04oSliderSetting slider_das;
        public T04oSliderSetting slider_dcd;
        public T04oSliderSetting slider_sdf;
        public float arr; // automatic repeat rate
        public float das; // delayet auto shift
        public float dcd; // DAS cut delay (after rotating, dropping piece)
        public float sdf; // soft drop factor
        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            base.OnPlayerRestored(player);
            if (Networking.LocalPlayer != player) {
                return;
            }
            if (PlayerData.HasKey(player, nameForSavePersistance + HandlingType.arr.ToString())) {
                LoadControls(player);
            } else {
                SetDefault();
            }
            ShowValues();
        }
        public void ShowValues() {
            slider_arr.SetValueWithoutNotify(arr);
            slider_das.SetValueWithoutNotify(das);
            slider_dcd.SetValueWithoutNotify(dcd);
            slider_sdf.SetValueWithoutNotify(sdf);
        }
        public void LoadControls(VRCPlayerApi playerApi) {
            LoadControl(playerApi, HandlingType.arr);
            LoadControl(playerApi, HandlingType.das);
            LoadControl(playerApi, HandlingType.dcd);
            LoadControl(playerApi, HandlingType.sdf);
        }
        public void LoadControl(VRCPlayerApi playerApi, HandlingType key) {
            float value = PlayerData.GetFloat(playerApi, nameForSavePersistance+key.ToString());
            SetValue(key, value);
        }
        public void SetValue(HandlingType key, float value) {
            switch (key)
            {
                case HandlingType.arr:
                arr = value;
                return;
                case HandlingType.das:
                das = value;
                return;
                case HandlingType.dcd:
                dcd = value;
                return;
                case HandlingType.sdf:
                sdf = value;
                return;
                default:
                return;
            }
        }
        public void SaveValue(VRCPlayerApi playerApi, HandlingType key, float value) {
            PlayerData.SetFloat(nameForSavePersistance+key.ToString(), value);
        }
        public void SaveValues() {
            VRCPlayerApi playerApi = Networking.LocalPlayer;
            SaveValue(playerApi, HandlingType.arr, arr);
            SaveValue(playerApi, HandlingType.das, das);
            SaveValue(playerApi, HandlingType.dcd, dcd);
            SaveValue(playerApi, HandlingType.sdf, sdf);
        }
        public void SetDefault() {
            SetValue(HandlingType.arr, 0.1f);
            SetValue(HandlingType.das, 0.15f);
            SetValue(HandlingType.dcd, 0.0f);
            SetValue(HandlingType.sdf, 0.1f);
            ShowValues();
        }
    }
}