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
        public T04oMultiplayer multiplayer;
        public T04oIMultiplayer multiplayerMenu;
        public T04oResizerButton resizerButton;
        [HideInInspector] public T04oControls controls;
        [UdonSynced] [HideInInspector] public byte isGameStarted = 0;
        [UdonSynced] byte isUsing = 0;
        [UdonSynced] byte playerID = 0;
        public void ChangeToGame() {
            mainMenu.gameObject.SetActive(false);
            gameProcess.gameProcessInterface.SetActive(true);
            controls = controlsGame;
            multiplayerMenu.HideMenu();
            controlsAll.SetSpeedRepeatingKeys(mainMenu.indexSpeedKeys);
            gameProcess.ChangeLevelSpeed(mainMenu.indexLevelSpeed);
            controlsAll.DisableRepeatingUpKey(true);
        }
        public void ChangeToInterface() {
            gameProcess.gameProcessInterface.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            multiplayerMenu.HideMenu();
            controls = controlsInterface;
            controlsAll.DisableRepeatingUpKey(false);
        }
        public void ChangeToInterfaceMultiplayer() {
            gameProcess.gameProcessInterface.SetActive(false);
            mainMenu.gameObject.SetActive(false);
            multiplayerMenu.ShowMenu();
            controls = controlsInterface;
            controlsAll.DisableRepeatingUpKey(false);
        }
        public T04oControls GetControls() {
            return controls;
        }
        public void BlockInteractions(VRCPlayerApi playerApi) {
            isUsing = 1;
            resizerButton.resizerTimer.StopTimer();
            RequestSerialization();
        }
        public void UnblockInteractions() {
            isUsing = 0;
            controlsAll.DisableInteractions(false);
            resizerButton.DisableInteraction(false);
            resizerButton.StartTheTimerIfSizeDifferent();
            RequestSerialization();
        }
        public void CheckInteractions() {
            if (isUsing == 1) {
                controlsAll.DisableInteractions(true);
                resizerButton.DisableInteraction(true);
            } else {
                controlsAll.DisableInteractions(false);
                resizerButton.DisableInteraction(false);
            }
        }
        public void StartTheGame() {
            isGameStarted = 1;
            ChangeToGame();
            gameProcess.StartGame();
            RequestSerialization();
        }
        public void JoinMultiplayer() {
            isGameStarted = 2;
            multiplayerMenu.JoinGame();
            ChangeToInterfaceMultiplayer();
            RequestSerialization();
        }
        public void LeaveMultiplayer() {
            isGameStarted = 0;
            multiplayerMenu.LeaveFromGame();
            ChangeToInterface();
            RequestSerialization();
        }
        public void GameOver() {
            if (multiplayerMenu.isPlayerJoined) {
                isGameStarted = 2;
                ChangeToInterfaceMultiplayer();
            } else {
                isGameStarted = 0;
                ChangeToInterface();
            }
            RequestSerialization();
        }
        void Start()
        {
            controlsAll.StartInteractions();
            ChangeToInterface();
        }
        public void SetOwnerIfNot(VRCPlayerApi player = null) {
            if (player == null) player = Networking.LocalPlayer;
            if (playerID != (byte)(player.playerId % 255)) {
                SetOwner(player);
            }
        }
        public void SetOwner(VRCPlayerApi player = null) {
            if (gameProcess.isGameRunning) {
                gameProcess.StartUpdate();
            }
            if (player == null) player = Networking.LocalPlayer;

            playerID = (byte)(player.playerId % 255);
            if (Networking.IsOwner(player, gameObject)) {
                return;
            }
            Networking.SetOwner(player, gameObject);
            Networking.SetOwner(player, gameProcess.gameObject);
            Networking.SetOwner(player, gameProcess.gameField.gameObject);
            Networking.SetOwner(player, gameProcess.garbageMeter.gameObject);
            Networking.SetOwner(player, gameProcess.garbageSender.gameObject);
            Networking.SetOwner(player, mainMenu.gameObject);
            Networking.SetOwner(player, multiplayerMenu.gameObject);
            Networking.SetOwner(player, scoring.gameObject);
            Networking.SetOwner(player, resizerButton.gameObject);
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
            if (playerID != (newOwner.playerId % 255)) {
                UnblockInteractions();
                if (Networking.LocalPlayer == newOwner) {
                    SetOwner(Networking.LocalPlayer);
                }
            }
            if (Networking.LocalPlayer != newOwner) {
                resizerButton.resizerTimer.StopTimer();
                return;
            }
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (Networking.IsOwner(player, this.gameObject)) {
                UnblockInteractions();
            }
            base.OnPlayerRespawn(player);
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            if (isGameStarted == 2) {
                ChangeToInterfaceMultiplayer();
            }
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