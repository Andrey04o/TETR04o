using UnityEngine;
using UdonSharp;
using TMPro;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oIMainMenu : UdonSharpBehaviour
    {
        public T04oMain main;
        public TextMeshPro textMeshStartGame;
        public TextMeshPro textMeshMultiplayer;
        public TextMeshPro textMeshLevelSpeed;
        public TextMeshPro textMeshLevelSpeedNumber;
        public TextMeshPro textMeshRepeatingSpeedKeys;
        public TextMeshPro textMeshRepeatingSpeedKeysNumber;
        [UdonSynced] public byte indexMenu = 0;
        [UdonSynced] public byte indexLevelSpeed = 1;
        [UdonSynced] public byte indexSpeedKeys = 1;
        [UdonSynced] byte menuHide = 0;
        public Color colorIn;
        public Color colorRegular;
        public Color colorChanging;

        void Start() {
            ChangeColorText(indexMenu);
        }

        public void Left() {
            if (indexMenu == 2) {
                if (indexLevelSpeed == 1) return;
                indexLevelSpeed--;
                SetNumberLevelSpeed();
            }
            if (indexMenu == 3) {
                if (indexSpeedKeys == 0) return;
                indexSpeedKeys--;
                SetNumberSpeedKeys();
            }
            RequestSerialization();
        }

        public void Right() {
            if (indexMenu == 2) {
                if (indexLevelSpeed == 20) return;
                indexLevelSpeed++;
                SetNumberLevelSpeed();
            }
            if (indexMenu == 3) {
                if (indexSpeedKeys == 100) return;
                indexSpeedKeys++;
                SetNumberSpeedKeys();
            }
            RequestSerialization();
        }
        public void SetNumberLevelSpeed() {
            textMeshLevelSpeedNumber.text = indexLevelSpeed + "";
        }
        public void SetNumberSpeedKeys() {
            textMeshRepeatingSpeedKeysNumber.text = indexSpeedKeys + "";
        }

        public void Up() {
            if (indexMenu == 0) return;
            indexMenu--;
            ChangeColorText(indexMenu);
            RequestSerialization();
        }

        public void Down() {
            if (indexMenu == 2) return;
            indexMenu++;
            ChangeColorText(indexMenu);
            RequestSerialization();
        }
        public void Confirm() {
            if (indexMenu == 0) {
                main.StartTheGame();
            }
            if (indexMenu == 1) {
                main.JoinMultiplayer();
            }
            RequestSerialization();
        }

        public void ChangeColorText(int i) {
            textMeshStartGame.color = colorRegular;
            textMeshMultiplayer.color = colorRegular;
            textMeshLevelSpeed.color = colorRegular;
            textMeshLevelSpeedNumber.color = colorRegular;
            textMeshRepeatingSpeedKeys.color = colorRegular;
                    textMeshRepeatingSpeedKeysNumber.color = colorRegular;
            switch (i)
            {
                case 0:
                    textMeshStartGame.color = colorIn;
                    break;
                case 1:
                    textMeshMultiplayer.color = colorIn;
                    break;
                case 2:
                    textMeshLevelSpeed.color = colorIn;
                    textMeshLevelSpeedNumber.color = colorChanging;
                    break;
                case 3:
                    textMeshRepeatingSpeedKeys.color = colorIn;
                    textMeshRepeatingSpeedKeysNumber.color = colorChanging;
                    break;
                default:
                    break;
            }
        }

        public void HideMenu() {
            menuHide = 1;
            gameObject.SetActive(false);
        }

        public void ShowMenu() {
            menuHide = 0;
            gameObject.SetActive(true);
        }

        public void NetworkStuff() {
            ChangeColorText(indexMenu);
            SetNumberLevelSpeed();
            SetNumberSpeedKeys();
            if (menuHide == 1)
                gameObject.SetActive(false);
            if (menuHide == 0)
                gameObject.SetActive(true);
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            NetworkStuff();
        }
    }
}