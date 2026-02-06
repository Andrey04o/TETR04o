#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oCell))]
    public class T04oCellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oCell myTarget = (T04oCell)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("SetMaterialMeshRenderers"))
            {
                //myTarget.SetMaterialMeshRenderers();
                //myTarget.materialRenderer = new Material(myTarget.materialWhite);
                foreach (MeshRenderer mr in myTarget.meshRenderers) {
                    
                    mr.material = myTarget.materialRenderer;
                    EditorUtility.SetDirty(mr);
                }
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
    
}
#endif