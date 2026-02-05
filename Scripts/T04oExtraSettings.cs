using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
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
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oExtraSettings))]
    public class T04oExtraSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oExtraSettings myTarget = (T04oExtraSettings)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Get all T04oGStations"))
            {
                myTarget.stations = FindObjectsByType<T04oGStation>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
    #endif
}
