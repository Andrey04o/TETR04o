using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Persistence;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
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
            SetValue(HandlingType.arr, 0.5f);
            SetValue(HandlingType.das, 0.5f);
            SetValue(HandlingType.dcd, 0.5f);
            SetValue(HandlingType.sdf, 0.5f);
            ShowValues();
        }
    }
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oHandling))]
    public class T04oHandlingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();
            T04oHandling myTarget = (T04oHandling)target;
            EditorGUILayout.Space();
            if (GUILayout.Button("Set handling reference to all arcade machines"))
            {
                T04oMain[] mains = FindObjectsByType<T04oMain>(FindObjectsSortMode.None);
                foreach (T04oMain main in mains) {
                    main.handling = myTarget;
                    EditorUtility.SetDirty(main);
                }
            }
        }
    }
    #endif
}
