using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    public class T04oIControls : T04oControls
    {
        public T04oMain main;
        override public void Up() {
            if (main.isGameStarted == 0)
            main.mainMenu.Up();
            else if (main.isGameStarted == 2)
            main.multiplayerMenu.Up();
        }
        override public void Down() {
            if (main.isGameStarted == 0)
            main.mainMenu.Down();
            else if (main.isGameStarted == 2)
            main.multiplayerMenu.Down();
        }
        override public void Left() {
            if (main.isGameStarted == 0)
            main.mainMenu.Left();
        }
        override public void Right() {
            if (main.isGameStarted == 0)
            main.mainMenu.Right();
        }
        override public void RotateLeft() {
            if (main.isGameStarted == 0)
            main.mainMenu.Confirm();
            else if (main.isGameStarted == 2)
            main.multiplayerMenu.Confirm();
        }
        override public void RotateRight() {
            if (main.isGameStarted == 0)
            main.mainMenu.Confirm();
            else if (main.isGameStarted == 2)
            main.multiplayerMenu.Confirm();
        }
        override public void Respawn() {
            if (main.isGameStarted == 0)
            main.mainMenu.Confirm();
            else if (main.isGameStarted == 2)
            main.multiplayerMenu.Confirm();
        }
        public override void SpaceBar()
        {
            if (main.isGameStarted == 0)
            main.mainMenu.Confirm();
            else if (main.isGameStarted == 2)
            main.multiplayerMenu.Confirm();
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
        
        public void StartGame() {
            main.SetOwner();
            if (isInterface) {
                main.StartTheGame();
            }
            isInterface = false;

        }
    }
}