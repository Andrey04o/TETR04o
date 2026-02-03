using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oIMultiplayer : UdonSharpBehaviour
    {
        public T04oMain main;
        public GameObject inteface;
        public TextMeshPro textMeshReady;
        public TextMeshPro textMeshReadySet;
        public TextMeshPro textMeshLeave;
        public TextMeshPro textMeshCountPlayers;
        public TextMeshPro textMeshCountPlayersCount;
        public TextMeshPro textMeshStartingIn;
        public TextMeshPro textMeshStartingInCount;
        [UdonSynced] public byte indexMenu = 0;
        [UdonSynced] public byte indexReady = 0;
        [UdonSynced] public byte indexPlayers = 0;
        [UdonSynced] byte menuHide = 0;
        public Color colorIn;
        public Color colorRegular;
        public Color colorChanging;
        [UdonSynced] public bool isPlayerJoined = false;

        void Start() {
            ChangeColorText(indexMenu);
        }
        public void ShowReady() {
            if (indexReady == 0) {
                textMeshReadySet.text = "No";
                textMeshReadySet.color = Color.red;
            }
            if (indexReady == 1) {
                textMeshReadySet.text = "Yes";
                textMeshReadySet.color = Color.green;
            }
        }
        void ToggleReady() {
            if (indexReady == 0) indexReady = 1;
            else indexReady = 0;
            ShowReady();
            SendReady();
            //RequestSerialization();
        }
        void SetReady(byte value) {
            indexReady = value;
            SendReady();
            ShowReady();
        }
        void SendReady() {
            if (indexReady == 0) {
                main.multiplayer.RemoveReadyRequest();
            }
            if (indexReady == 1) {
                main.multiplayer.AddReadyRequest();
            }
        }
        public void SetPlayerCountRequest(byte value) { // why I added this?
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(SetPlayerCount), value);
        }
        [NetworkCallable] public void SetPlayerCount(byte value) {
            textMeshCountPlayersCount.text = value + "";
        }
        public void SetPlayerCountReady(byte value) {
            //textMeshCountPlayersCount.text = value + "";
        }
        public void ShowStartingTimer(bool value) {
            textMeshStartingIn.gameObject.SetActive(value);
        }
        public void SetTimer(string time) {
            textMeshStartingInCount.text = time;
        }

        public void Up() {
            if (indexMenu == 0) return;
            indexMenu--;
            ChangeColorText(indexMenu);
            RequestSerialization();
        }

        public void Down() {
            if (indexMenu == 1) return;
            indexMenu++;
            ChangeColorText(indexMenu);
            RequestSerialization();
        }
        public void Confirm() {
            if (indexMenu == 0) {
                ToggleReady();
            }
            if (indexMenu == 1) {
                main.LeaveMultiplayer();
            }
            RequestSerialization();
        }


        public void ChangeColorText(int i) {
            textMeshReady.color = colorRegular;
            textMeshLeave.color = colorRegular;
            switch (i)
            {
                case 0:
                    textMeshReady.color = colorIn;
                    break;
                case 1:
                    textMeshLeave.color = colorIn;
                    break;
                default:
                    break;
            }
        }

        public void HideMenu() {
            menuHide = 1;
            SetReady(0);
            inteface.SetActive(false);
        }
        public void LeaveFromGame() {
            isPlayerJoined = false;
            main.multiplayer.RemovePlayerRequest(main.gameProcess.id);
            RequestSerialization();
        }
        public void JoinGame() {
            isPlayerJoined = true;
            main.multiplayer.AddPlayerRequest(main.gameProcess.id);
            RequestSerialization();
        }

        public void ShowMenu() {
            menuHide = 0;
            inteface.SetActive(true);
            RequestSerialization();
        }
        public void ShowVisibility() {
            if (menuHide == 1)
                inteface.SetActive(false);
            if (menuHide == 0)
                inteface.SetActive(true);
        }

        public void NetworkStuff() {
            ChangeColorText(indexMenu);
            ShowReady();
            ShowVisibility();
        }
        public void StartGameRequest() {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(StartGame));
        }
        [NetworkCallable] public void StartGame() {
            main.StartTheGame();
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            NetworkStuff();
            /*
            if (Networking.IsOwner(main.multiplayer.gameObject)) {
                if (indexReady == 0) {
                    main.multiplayer.AddReady();
                }
                if (indexReady == 1) {
                    main.multiplayer.RemoveReady();
                }
            }
            */
        }
    }
}
