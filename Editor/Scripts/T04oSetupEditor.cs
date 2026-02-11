#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace TETR04o {
    [CustomEditor(typeof(T04oSetup))]
    public class T04oSetupEditor : Editor
    {
        T04oMultiplayer multiplayer;
        T04oHotkeys hotkeys;
        T04oHandling handling;
        T04oExtraSettings extraSettings;
        T04oGameProcess[] machines;
        T04oMain[] mains;
        public static bool isSetup = false;
        public static bool isSuccess = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            T04oSetup myTarget = (T04oSetup)target;
            Color originalColor = GUI.backgroundColor;
            if (isSetup == false) {
                GUI.backgroundColor = Color.yellow;
            }
            
            if (GUILayout.Button("Setup TETR04o arcade machines"))
            {
                Setup();
            }
            if (isSetup == true) {
                
                if (isSuccess == true) {
                    GUI.backgroundColor = Color.green;
                    EditorGUILayout.HelpBox("Setup succesfull!", MessageType.Info);
                } else {
                    GUI.backgroundColor = Color.red;
                    CheckObjects();
                }
            }
            GUI.backgroundColor = originalColor;
            
        }
        public void Setup() {
            isSetup = true;
            isSuccess = false;
            if (FindObjects() == false) return;
            if (CheckObjects() == false) {
                return;
            }
            SetReferences();
        }

        bool FindObjects() {
            mains = Object.FindObjectsByType<T04oMain>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            if (mains.Length == 0) {
                return false;
            }
            multiplayer = FindAnyObjectByType<T04oMultiplayer>(FindObjectsInactive.Include);
            hotkeys = FindAnyObjectByType<T04oHotkeys>(FindObjectsInactive.Include);
            handling = FindAnyObjectByType<T04oHandling>(FindObjectsInactive.Include);
            extraSettings = FindAnyObjectByType<T04oExtraSettings>(FindObjectsInactive.Include);
            machines = FindObjectsByType<T04oGameProcess>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            return true;
        }
        public bool CheckObjects() {
            bool value = true;
            if (multiplayer == null) {
                EditorGUILayout.HelpBox("TETR04o: Can't find Multiplayer object in the scene. You can put Multiplayer prefab to the scene from \"TETR04o\\Runtime\\Prefabs\\Multiplayer.prefab\"", MessageType.Error);
                value = false;
            }
            if (hotkeys == null) {
                EditorGUILayout.HelpBox("TETR04o: Can't find Hotkey object in the scene. You can put Controls settings prefab to the scene from \"TETR04o\\Runtime\\Prefabs\\Controls settings.prefab\"", MessageType.Error);
                value = false;
            }
            if (handling == null) {
                EditorGUILayout.HelpBox("TETR04o: Can't find Handling object in the scene. You can put Controls settings prefab to the scene from \"TETR04o\\Runtime\\Prefabs\\Controls settings.prefab\"", MessageType.Error);
                value = false;
            }
            if (handling == null) {
                EditorGUILayout.HelpBox("TETR04o: Can't find Extra object in the scene. You can put Controls settings prefab to the scene from \"TETR04o\\Runtime\\Prefabs\\Controls settings.prefab\"", MessageType.Error);
                value = false;
            }
            return value;
        }

        void SetReferences() {
            T04oMultiplayerEditor.SetReferenceAndIDSSerialized(multiplayer, machines);
            T04oHotKeysEditor.SetReferenceSerialized(hotkeys, mains);
            T04oHandlingEditor.SetReferenceSerialized(handling, mains);
            T04oExtraSettingsEditor.SetReferenceSerialized(extraSettings);
            isSuccess = true;
        }
    }
}
#endif