#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oGameField))]
    public class T04oGameFieldEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oGameField myTarget = (T04oGameField)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Spawn 1 garbage line"))
            {
                myTarget.ReceiveGarbageAttack(1);
            }

            if (GUILayout.Button("Spawn 4 garbage lines"))
            {
                myTarget.ReceiveGarbageAttack(4);
            }
        }
    }
}
#endif