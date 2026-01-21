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
            main.mainMenu.Up();
        }
        override public void Down() {
            main.mainMenu.Down();
        }
        override public void Left() {
            main.mainMenu.Left();
        }
        override public void Right() {
            main.mainMenu.Right();
        }
        override public void RotateLeft() {
            main.mainMenu.Confirm();
        }
        override public void RotateRight() {
            main.mainMenu.Confirm();
        }
        override public void Respawn() {
            main.mainMenu.Confirm();
        }
        public override void SpaceBar()
        {
            main.mainMenu.Confirm();
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