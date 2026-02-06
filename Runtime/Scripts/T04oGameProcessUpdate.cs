using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oGameProcessUpdate : UdonSharpBehaviour
    {
        public T04oGameProcess gameProcess;
        void Update() {
            if (gameProcess.isGameRunning == false) return;
            gameProcess.timer += Time.deltaTime;
            if (gameProcess.timer >= gameProcess.tickSpeed) {
                gameProcess.timer = 0;
                gameProcess.MovePieceDownGravity();
                gameProcess.isFloor = !gameProcess.currentPiece.IsCanMove(Vector2Int.down);
            }
            if (gameProcess.isFloor) {
                gameProcess.timerPlace += Time.deltaTime;
                if (gameProcess.timerPlace >= gameProcess.lockDelay) {
                    if (gameProcess.currentPiece.IsCanMove(Vector2Int.down)) return;
                    gameProcess.currentPiece.Place();
                    gameProcess.SpawnNewPiece();
                    gameProcess.CheckLinesCleared();
                }
            }
        }
    }
}