using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oMain : UdonSharpBehaviour
    {
        public T04oGameProcess gameProcess;
        public T04oIMainMenu mainMenu;
        public T04oGControls controlsGame;
        public T04oIControls controlsInterface;
        public T04oControlsAll controlsAll;
        public T04oScoring scoring;
        public T04oHotkeys hotkeys;
        [HideInInspector] public T04oControls controls;
        [UdonSynced] byte isGameStarted = 0;
        [UdonSynced] byte isUsing = 0;
        public void ChangeToGame() {
            mainMenu.gameObject.SetActive(false);
            gameProcess.gameProcessInterface.SetActive(true);
            controls = controlsGame;
            controlsAll.SetSpeedRepeatingKeys(mainMenu.indexSpeedKeys);
            gameProcess.ChangeLevelSpeed(mainMenu.indexLevelSpeed);
            controlsAll.DisableRepeatingUpKey(true);
        }
        public void ChangeToInterface() {
            gameProcess.gameProcessInterface.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            controls = controlsInterface;
            controlsAll.DisableRepeatingUpKey(false);
        }
        public T04oControls GetControls() {
            return controls;
        }
        public void BlockInteractions(VRCPlayerApi playerApi) {
            isUsing = 1;
            if (playerApi == Networking.LocalPlayer) {
                return;
            }
            controlsAll.DisableInteractions(true);
            RequestSerialization();
        }
        public void UnblockInteractions() {
            isUsing = 0;
            controlsAll.DisableInteractions(false);
            RequestSerialization();
        }
        public void CheckInteractions() {
            if (isUsing == 1) {
                controlsAll.DisableInteractions(true);
            } else {
                controlsAll.DisableInteractions(false);
            }
        }
        public void StartTheGame() {
            isGameStarted = 1;
            ChangeToGame();
            gameProcess.StartGame();
            RequestSerialization();
        }
        public void GameOver() {
            isGameStarted = 0;
            ChangeToInterface();
            RequestSerialization();
        }
        void Start()
        {
            controlsAll.StartInteractions();
            ChangeToInterface();
        }
        public void SetOwner(VRCPlayerApi player = null) {
            if (gameProcess.isGameRunning) {
                gameProcess.StartUpdate();
            }
            if (player == null) player = Networking.LocalPlayer;
            if (Networking.IsOwner(player, gameObject)) {
                return;
            }
            Networking.SetOwner(player, gameObject);
            Networking.SetOwner(player, gameProcess.gameObject);
            Networking.SetOwner(player, gameProcess.gameField.gameObject);
            Networking.SetOwner(player, mainMenu.gameObject);
            Networking.SetOwner(player, scoring.gameObject);
            foreach (T04oLine line in gameProcess.gameField.lines) {
                Networking.SetOwner(player, line.gameObject);
            }
            foreach (T04oPiece piece in gameProcess.pieces) {
                if (piece == null) continue;
                Networking.SetOwner(player, piece.gameObject);
            }
            RequestSerialization();
        }

        public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
        {
            if (Networking.LocalPlayer != newOwner) return;
            if (Networking.IsOwner(newOwner, gameProcess.gameObject) == false) {
                SetOwner(newOwner);
            }
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            if (isGameStarted == 1) {
                ChangeToGame();
                gameProcess.TurnItOn();
            }
            if (isGameStarted == 0) {
                ChangeToInterface();
            }
            CheckInteractions();
            gameProcess.StopUpdate();
        }
    }
}