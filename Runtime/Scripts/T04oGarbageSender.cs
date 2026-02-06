using UnityEngine;
using UdonSharp;
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
            if (main.multiplayerMenu.isPlayerJoined)
                main.multiplayer.GetRandomPlayerOpponent(main.gameProcess.id).garbageMeter.ReceiveGarbageAttackRequest(indexGarbage);

            ClearGarbage();
        }
    }
}
