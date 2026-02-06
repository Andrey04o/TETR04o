#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oGarbageMeter))]
    public class T04oGarbageMeterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oGarbageMeter myTarget = (T04oGarbageMeter)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Receive 1 garbage line"))
            {
                myTarget.ReceiveGarbageAttack(1);
            }

            if (GUILayout.Button("Receive 4 garbage lines"))
            {
                myTarget.ReceiveGarbageAttack(4);
            }
        }
    }
}
#endif
