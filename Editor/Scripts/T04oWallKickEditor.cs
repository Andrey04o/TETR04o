#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UdonSharpEditor;
using UnityEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oWallKick))]
    public class T04oWallKickEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oWallKick myTarget = (T04oWallKick)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Invert Y"))
            {
                myTarget.InvertY();
                EditorUtility.SetDirty(myTarget);
            }
            if (GUILayout.Button("Invert X"))
            {
                myTarget.InvertX();
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
}
#endif