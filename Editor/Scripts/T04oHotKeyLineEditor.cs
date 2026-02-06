#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.Udon.Editor.ProgramSources.UdonGraphProgram.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oHotkeyLine))]
    public class T04oHotKeyLineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oHotkeyLine myTarget = (T04oHotkeyLine)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Set key type"))
            {
                myTarget.textMeshControl.text = myTarget.key.ToString().UppercaseFirst() + " -";
                foreach (T04oChangeHotkeys change in myTarget.changers) {
                    change.key = myTarget.key;
                    change.hotkeys = myTarget.hotkeys;
                    EditorUtility.SetDirty(change);
                }
                EditorUtility.SetDirty(myTarget);
                EditorSceneManager.MarkSceneDirty(myTarget.gameObject.scene);
            }
        }
    }
}
#endif
