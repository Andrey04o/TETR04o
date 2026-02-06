#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oPiece))]
    public class T04oPieceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oPiece myTarget = (T04oPiece)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("SetPositionsOffset"))
            {
                myTarget.SetCellOffset();
            }
            if (GUILayout.Button("SetPieceColorToCells"))
            {
                myTarget.SetPieceColorsToCells();
            }
        }
    }
}
#endif
