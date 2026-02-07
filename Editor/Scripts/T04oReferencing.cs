using UnityEditor;
using UnityEngine;
namespace TETR04o {
    public static class T04oReferencing
    {
        //public int callbackOrder { get {return 100; } }
        static T04oMultiplayer multiplayer;
        static T04oHotkeys hotkeys;
        static T04oHandling handling;
        static T04oExtraSettings extraSettings;
        static T04oGameProcess[] machines;
        static T04oMain[] mains;
        public static bool isSetup = false;
        public static bool isSuccess = false;

        public static void Setup() {
            isSetup = true;
            isSuccess = false;
            if (FindObjects() == false) return;
            if (CheckObjects() == false) {
                return;
            }
            SetReferences();
        }

        static bool FindObjects() {
            mains = Object.FindObjectsByType<T04oMain>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            if (mains.Length == 0) {
                return false;
            }
            multiplayer = Object.FindAnyObjectByType<T04oMultiplayer>(FindObjectsInactive.Include);
            hotkeys = Object.FindAnyObjectByType<T04oHotkeys>(FindObjectsInactive.Include);
            handling = Object.FindAnyObjectByType<T04oHandling>(FindObjectsInactive.Include);
            extraSettings = Object.FindAnyObjectByType<T04oExtraSettings>(FindObjectsInactive.Include);
            machines = Object.FindObjectsByType<T04oGameProcess>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            return true;
        }
        public static bool CheckObjects() {
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

        static void SetReferences() {
            T04oMultiplayerEditor.SetReferenceAndIDS(multiplayer, machines);
            T04oHotKeysEditor.SetReference(hotkeys, mains);
            T04oHandlingEditor.SetReference(handling, mains);
            T04oExtraSettingsEditor.SetReference(extraSettings);
            isSuccess = true;
        }
    }
}