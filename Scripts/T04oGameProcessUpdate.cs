using System.Collections;
using System.Collections.Generic;
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
            }
            if (gameProcess.isFloor) {
                gameProcess.timerPlace += Time.deltaTime;
                if (gameProcess.timerPlace >= gameProcess.tickSpeed) {
                    gameProcess.timerPlace = 0;
                    gameProcess.ResetTimer();
                    gameProcess.currentPiece.Place();
                    gameProcess.SpawnNewPiece();
                    gameProcess.CheckLinesCleared();
                }
            }
        }
    }
}