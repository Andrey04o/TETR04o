#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
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
        public static void SetReference(T04oExtraSettings myTarget) {
            myTarget.stations = FindObjectsByType<T04oGStation>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EditorUtility.SetDirty(myTarget);
        }
        public static void SetReferenceSerialized(T04oExtraSettings myTarget) {
            T04oGStation[] stations = FindObjectsByType<T04oGStation>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            SerializedObject serialized = new SerializedObject(myTarget);
            SerializedProperty stationsProp = serialized.FindProperty("stations");
            stationsProp.arraySize = stations.Length;
            for (int i = 0; i < stations.Length; i++) {
                stationsProp.GetArrayElementAtIndex(i).objectReferenceValue = stations[i];
            }
            stationsProp.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif