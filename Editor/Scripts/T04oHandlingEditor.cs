#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
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
                SetReference(myTarget, mains);
            }
        }
        public static void SetReference(T04oHandling myTarget, T04oMain[] mains) {
            foreach (T04oMain main in mains) {
                main.handling = myTarget;
                EditorUtility.SetDirty(main);
            }
        }

        public static void SetReferenceSerialized(T04oHandling myTarget, T04oMain[] mains) {
            foreach (T04oMain main in mains) {
                SerializedObject mainObj = new SerializedObject(main);
                SerializedProperty handlingProp = mainObj.FindProperty("handling");
                handlingProp.objectReferenceValue = myTarget;
                mainObj.ApplyModifiedProperties();
            }
        }
    }
}
#endif
