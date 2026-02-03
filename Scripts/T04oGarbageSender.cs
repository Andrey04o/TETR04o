using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
namespace TETR04o {
    public enum ClearType
    {
        Zero,
        Single,
        Double,
        Triple,
        Tetris,
        MiniT_SpinSingle,
        MiniT_SpinDouble,
        T_SpinSingle,
        T_SpinDouble,
        T_SpinTriple,
        Back_to_Back_Bonus,
        PerfectClear

    }
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oGarbageSender : UdonSharpBehaviour
    {
        public T04oGarbageMeter garbageMeter;
        public T04oMain main;
        [UdonSynced] public byte indexGarbage;

        public void SendGarbage(byte value) {
            indexGarbage += value;
            ProcessGarbageMeter();
        }
        public void SendGarbage(ClearType type) {
            indexGarbage += GetSentRow(type);
            ProcessGarbageMeter();
        }
        public void ClearGarbage() {
            indexGarbage = 0;
        }
        byte GetSentRow(ClearType type) {
            switch (type)
            {
                case ClearType.Single:
                return 0;
                case ClearType.Double:
                return 1;
                case ClearType.Triple:
                return 2;
                case ClearType.Tetris:
                return 4;
                case ClearType.MiniT_SpinSingle:
                return 0;
                case ClearType.MiniT_SpinDouble:
                return 1;
                case ClearType.T_SpinSingle:
                return 2;
                case ClearType.T_SpinDouble:
                return 4;
                case ClearType.T_SpinTriple:
                return 6;
                case ClearType.PerfectClear:
                return 10;
                default:
                return 0;
            }
            return 0;
        }
        void ProcessGarbageMeter() {
            indexGarbage = garbageMeter.DecreaseGarbageAttack(indexGarbage);
            
            main.multiplayer.GetRandomPlayerOpponent(main.gameProcess.id).garbageMeter.ReceiveGarbageAttackRequest(indexGarbage);

            ClearGarbage();
        }
    }
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
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
    #endif
}
