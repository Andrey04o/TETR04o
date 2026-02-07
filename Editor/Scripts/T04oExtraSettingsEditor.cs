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
    }
    #endif
}