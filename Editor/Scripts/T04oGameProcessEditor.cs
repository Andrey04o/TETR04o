#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oGameProcess))]
    public class T04oGameProcessEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oGameProcess myTarget = (T04oGameProcess)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Init indexes of pieces"))
            {
                myTarget.InitIndexes();
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
}
#endif
