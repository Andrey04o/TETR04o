using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC;
using TMPro;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using VRC.Udon.Editor.ProgramSources.UdonGraphProgram.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UdonSharpEditor;
#endif
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oHotkeyLine : UdonSharpBehaviour
    {
        public keyType key;
        public TextMeshProUGUI textMeshControl;
        public T04oChangeHotkeys[] changers;
        public T04oHotkeys hotkeys;

    }
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
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
                    change.MarkDirty();
                    EditorUtility.SetDirty(change.gameObject);
                }
                myTarget.MarkDirty();
                EditorUtility.SetDirty(myTarget);
                EditorSceneManager.MarkSceneDirty(myTarget.gameObject.scene);
            }
        }
    }
    #endif
}