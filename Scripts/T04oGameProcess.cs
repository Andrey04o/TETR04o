using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.SDK3.UdonNetworkCalling;
using VRC;
using UnityEngine.UIElements;
using VRC.Udon.Common.Interfaces;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oGameProcess : UdonSharpBehaviour
    {
        public byte id = 0;
        public T04oGameProcessUpdate gameProcessUpdate;
        public GameObject gameProcessInterface;
        public T04oGameField gameField;
        public T04oGameField[] gameFieldNextPieces;
        public T04oGameField gameFieldHold;
        public T04oPiece[] pieces;
        public T04oPiece currentPiece;
        public T04oGarbageMeter garbageMeter;
        public T04oGarbageSender garbageSender;
        public T04oMain main;
        public T04oScoring scoring;
        public T04oPlayersAlive playersAlive;
        private T04oPiece nextPiece;
        public float speedFall = 1.2f;
        public float lockDelay = 0.5f;
        public byte lockDelayTimesReset = 15;
        [HideInInspector] public float tickSpeed = 1f;
        [HideInInspector] public float timer = 0f;
        [HideInInspector] public float timerPlace;
        private bool isGameOver;
        private int score;
        private int linesCleared;
        [HideInInspector] public bool isFloor = false;
        [HideInInspector] public bool isGameRunning = false;
        private Vector2Int positionSpawnPiece = new Vector2Int(3, 0);
        public int linesForLevelUp = 5;
        [UdonSynced] public byte[] nextPieceNetwork;
        [UdonSynced] byte currentPieceNetwork;
        [UdonSynced] byte currentPieceNetworkHold;
        [UdonSynced] byte indexLevelSpeed = 1;
        [UdonSynced] byte linesClearedToLevelUp = 1;
        [UdonSynced] byte countHold = 1;
        [UdonSynced] byte indexCombo = 0;
        [UdonSynced] byte indexCountLockDelayReset = 0;
        public void StartUpdate() {
            gameProcessUpdate.gameObject.SetActive(true);
        }
        public void StopUpdate() {
            gameProcessUpdate.gameObject.SetActive(false);
        }
        public void TurnItOn() {
            //InitIndexes();
            isGameRunning = true;
            gameObject.SetActive(true);
            playersAlive.Show(main.multiplayerMenu.isPlayerJoined);
            ChangeLevelSpeed(indexLevelSpeed);
        }

        public void InitIndexes() {
            for (int i = 0; i < pieces.Length; i++) {
                pieces[i].index = i + 1;
                #if !COMPILER_UDONSHARP && UNITY_EDITOR
                pieces[i].MarkDirty();
                #endif
            }
            nextPieceNetwork = new byte[gameFieldNextPieces.Length];
        }
        void MakeGameClear() {
            gameField.Clear();
            linesClearedToLevelUp = 0;
            currentPieceNetworkHold = 0;
            ShowVisuallyHold();
        }
        public void StartGame() {
            //InitIndexes();
            StartUpdate();
            MakeGameClear();
            garbageMeter.ClearGarbage();
            garbageSender.ClearGarbage();
            isGameRunning = true;
            //gameObject.SetActive(true);
            gameProcessInterface.SetActive(true);
            playersAlive.Show(main.multiplayerMenu.isPlayerJoined);
            FillNextPieces();
            //ChooseNextPiece();
            SpawnNewPiece();
            ChangeLevelSpeed(indexLevelSpeed);
            scoring.SetScore(0);
            SendCustomEventDelayedSeconds(nameof(UpdatePiece), 1f);
        }
        public void ChangeLevelSpeed(int value) {
            if (value > 100) {
                value = 100;
            }
            indexLevelSpeed = (byte)value;
            tickSpeed = 1/(1 + (indexLevelSpeed * 1f));
            Debug.Log("tickSpeed is = " + tickSpeed + " Level = " + indexLevelSpeed);
        }

        void SetSpeedFall() {
            if (speedFall != 0f)
                tickSpeed = 1/speedFall;
            else
                tickSpeed = 100000000;
        }

        public void GameOver() {
            if (main.multiplayerMenu.isPlayerJoined) {
                main.multiplayerMenu.SetVictorPlace(main.multiplayer.countAlive);
                main.multiplayer.PlayerDiedRequest(id);
                main.multiplayerMenu.SetReady(0);
            }
            isGameRunning = false;
            gameProcessUpdate.gameObject.SetActive(false);
            gameProcessInterface.SetActive(false);
            main.GameOver();
            //gameObject.SetActive(false);
        }
        public void GameWinRequest() {
            NetworkCalling.SendCustomNetworkEvent((IUdonEventReceiver)this, NetworkEventTarget.Owner, nameof(GameWin));
        }
        [NetworkCallable] public void GameWin() {
            main.multiplayerMenu.SetVictorPlace(1);
            main.multiplayerMenu.SetReady(0);
            isGameRunning = false;
            gameProcessUpdate.gameObject.SetActive(false);
            gameProcessInterface.SetActive(false);
            main.GameOver();
        }

        void FillNextPieces() {
            for (int i = 0; i < nextPieceNetwork.Length; i++) {
                nextPieceNetwork[i] = ChooseRandomPiece();
            }
        }
        public void UseHold() {
            byte replacer;
            if (countHold == 0) return;
            countHold--;
            if (currentPieceNetworkHold == 0) {
                currentPieceNetworkHold = currentPieceNetwork;
                replacer = nextPieceNetwork[0];
                ChooseNextPiece();
            } else {
                replacer = currentPieceNetworkHold;
                currentPieceNetworkHold = currentPieceNetwork;
            }
            ShowVisuallyHold();
            SpawnNewPiece(replacer);
            //RequestSerialization();
        }
        void ChooseNextPiece() {
            for (int i = 0; i < nextPieceNetwork.Length - 1; i++) {
                nextPieceNetwork[i] = nextPieceNetwork[i + 1];
            }
            nextPieceNetwork[nextPieceNetwork.Length - 1] = ChooseRandomPiece();

            ShowVisuallyNextPieces();
        }

        public byte ChooseRandomPiece() {
            return (byte)pieces[Random.Range(1,8)].index;
        }
        void ShowVisuallyNextPieces() {
            for (int i = 0; i < gameFieldNextPieces.Length; i++) {
                gameFieldNextPieces[i].ClearNoSync();
                if (nextPieceNetwork[i] == 0) continue;
                gameFieldNextPieces[i].DrawCells(pieces[nextPieceNetwork[i]]);
            }
        }
        void ShowVisuallyHold() {
            gameFieldHold.ClearNoSync();
            if (currentPieceNetworkHold == 0) {
                gameFieldHold.ClearNoSync();
            } else {
                gameFieldHold.DrawCells(pieces[currentPieceNetworkHold]);
            }
        }
        public void ResetLockDelay() {
            if (isFloor == false) return;
            if (indexCountLockDelayReset >= lockDelayTimesReset) return;
            isFloor = !currentPiece.IsCanMove(Vector2Int.down);
            indexCountLockDelayReset++;
            ResetTimer();
            RequestSerialization();
        }
        public void ResetLockDelayCount() {
            indexCountLockDelayReset = 0;
        }
        public void SpawnNewPiece(byte piece) {
            isFloor = false;
            ResetLockDelayCount();
            ResetTimer();
            currentPiece.ClearCellGhost();
            currentPiece.Clear();
            currentPieceNetwork = piece;
            currentPiece = pieces[currentPieceNetwork];
            gameField.SpawnPiece(positionSpawnPiece, currentPiece);
            if (currentPiece.IsCanMove(Vector2Int.zero) == false) {
                GameOver();
            }
            RequestSerialization();
        }
        public void SpawnNewPiece()
        {
            ProcessGarbagePush();

            isFloor = false;
            ResetLockDelayCount();
            ResetTimer();
            currentPieceNetwork = nextPieceNetwork[0];
            currentPiece = pieces[currentPieceNetwork];
            gameField.SpawnPiece(positionSpawnPiece, currentPiece);
            ChooseNextPiece();
            countHold = 1;

            if (currentPiece.IsCanMove(Vector2Int.zero) == false) {
                GameOver();
            }
            RequestSerialization();
        }
        public void ProcessGarbagePush() {
            if (indexCombo == 0) {
                garbageMeter.PushToField();
            }
        }
        void ShowNewPieceNetwork() {
            if (currentPiece != null)
                currentPiece.ClearSafe();
            currentPiece = pieces[currentPieceNetwork];
            gameField.SpawnPieceNetwork(currentPiece);
        }
        void SpawnNewPieceNetwork() {
            currentPiece = pieces[currentPieceNetwork];
            gameField.SpawnPiece(Vector2Int.zero, currentPiece);
        }
        public void ResetTimer() {
            timerPlace = 0;
            if (isFloor) timer = 0;
        }

        public void MovePieceDownGravity() {
            if (currentPiece.Move(Vector2Int.down) == false) {
                isFloor = true;
            }
        }

        public void AddClearedLine(int value) {
            scoring.AddPoints(value, indexLevelSpeed);
            linesCleared += value;
            linesClearedToLevelUp += (byte)value;
            while (linesClearedToLevelUp >= linesForLevelUp) {
                linesClearedToLevelUp -= (byte)linesForLevelUp;
                indexLevelSpeed++;
                ChangeLevelSpeed(indexLevelSpeed);
            }
            RequestSerialization();
        }

        public void CheckLinesCleared() { // I think, it's can be removed
            if(gameField.countLinesCleared > 0) {
                gameField.ProcessClearedLines();
            }
        }
        public void AddCombo() {
            if (indexCombo < 255) {
                indexCombo++;
            }
            
        }
        public void ResetCombo() {
            indexCombo = 0;
        }

        [NetworkCallable] public void UpdatePiece() {
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            ShowVisuallyNextPieces();
            ShowVisuallyHold();
            //SpawnNewPieceNetwork();
            ShowNewPieceNetwork();
            ChangeLevelSpeed(indexLevelSpeed);
        }

    }

    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oGameProcess))]
    public class T04oGameProcessEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oGameProcess myTarget = (T04oGameProcess)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Init indexes of pieces"))
            {
                myTarget.InitIndexes();
                myTarget.MarkDirty();
            }
        }
    }
    #endif
}