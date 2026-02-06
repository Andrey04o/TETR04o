#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oGarbageSender))]
    public class T04oGarbageSenderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oGarbageSender myTarget = (T04oGarbageSender)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Send 1 garbage line"))
            {
                myTarget.SendGarbage(1);
            }

            if (GUILayout.Button("Send 4 garbage lines"))
            {
                myTarget.SendGarbage(4);
            }
        }
    }
}
#endif