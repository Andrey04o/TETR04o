using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    public class T04oGControls : T04oControls
    {
        public T04oMain main;

        override public void Up() {
            main.SetOwner();
            main.gameProcess.currentPiece.HardDrop();
        }
        override public void Down() {
            main.SetOwner();
            main.gameProcess.currentPiece.SoftDrop();
        }
        override public void Left() {
            main.SetOwner();
            main.gameProcess.currentPiece.Move(Vector2Int.left);
            main.gameProcess.ResetLockDelay();
        }
        override public void Right() {
            main.SetOwner();
            main.gameProcess.currentPiece.Move(Vector2Int.right);
            main.gameProcess.ResetLockDelay();
        }
        override public void RotateLeft() {
            main.SetOwner();
            main.gameProcess.currentPiece.Rotate(false);
            main.gameProcess.ResetLockDelay();
        }
        override public void RotateRight() {
            main.SetOwner();
            main.gameProcess.currentPiece.Rotate(true);
            main.gameProcess.ResetLockDelay();
        }
        override public void Respawn() {
            main.SetOwner();
            main.gameProcess.UseHold();
        }
        public override void StartedUsing(VRCPlayerApi playerApi)
        {
            main.SetOwner(playerApi);
            main.BlockInteractions(playerApi);
        }  
        public override void LeaveControls()
        {
            main.UnblockInteractions();
        }
        public override void SpaceBar() {
            main.SetOwner();
            main.gameProcess.currentPiece.HardDrop();
        }
    }
}