#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace TETR04o {
    [CustomEditor(typeof(T04oMultiplayer))]
    public class T04oMultiplayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oMultiplayer myTarget = (T04oMultiplayer)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Get all arcade machines and set ids"))
            {
                myTarget.machines = FindObjectsByType<T04oGameProcess>(FindObjectsSortMode.None);
                byte id = 0;
                foreach (T04oGameProcess machine in myTarget.machines) {
                    machine.id = id;
                    id++;
                    machine.main.multiplayer = myTarget;
                    EditorUtility.SetDirty(machine.main);
                    EditorUtility.SetDirty(machine);
                }
                myTarget.players = new byte[myTarget.machines.Length];
                myTarget.playersAlive = myTarget.players;
                for(int i = 0; i < myTarget.players.Length; i++) {
                    myTarget.players[i] = byte.MaxValue;
                }
                EditorUtility.SetDirty(myTarget);
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}
#endif
