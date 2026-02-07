using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
using System;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
namespace TETR04o {
    [CustomEditor(typeof(T04oHotkeys))]
    public class T04oHotKeysEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();
            T04oHotkeys myTarget = (T04oHotkeys)target;
            EditorGUILayout.Space();
            if (GUILayout.Button("Set hotkeys reference to all arcade machines"))
            {
                T04oMain[] mains = FindObjectsByType<T04oMain>(FindObjectsSortMode.None);
                SetReference(myTarget, mains);
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            if (GUILayout.Button("Set default keys"))
            {
                myTarget.SetDefault();
                EditorUtility.SetDirty(myTarget);
            }
            EditorGUILayout.LabelField("For development");
            if (GUILayout.Button("Get all enum key list"))
            {
                myTarget.indexKeyCodes = (int[])Enum.GetValues(typeof(KeyCode));
                EditorUtility.SetDirty(myTarget);
            }
        }
        public static void SetReference(T04oHotkeys myTarget, T04oMain[] mains) {
            foreach (T04oMain main in mains) {
                main.hotkeys = myTarget;
                EditorUtility.SetDirty(main);
            }
        }
    }
}
#endif
