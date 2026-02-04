using UnityEngine;
using UdonSharp;
using System;
using System.Collections;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oMultiplayer : UdonSharpBehaviour
    {
        public T04oGameProcess[] machines;
        public T04oMultiplayerTimerStart timerStart;
        [UdonSynced] public byte[] players;
        [UdonSynced] public byte count;
        [UdonSynced] public byte[] playersAlive;
        [UdonSynced] public byte countAlive;
        [UdonSynced] public byte indexReady;
        [UdonSynced] public bool isInProcess = false;
        //[UdonSynced] public byte isGameRunning;
        public void AddPlayerRequest(byte playerId) {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(AddPlayer), playerId);
            //SendCustomNetworkEvent((IUdonEventReceiver)this, NetworkEventTarget.Owner, nameof(AddPlayer), playerId);
        }
        [NetworkCallable] public void AddPlayer(byte playerId) {
            if (playerId == byte.MaxValue)
            {
                Debug.LogWarning("Cannot add player with ID 255 (reserved for 'no player')");
                return;
            }
            
            if (players == null)
            {
                Debug.LogWarning("Players array is not initialized");
                return;
            }
            
            // Find first empty slot (ID = 0)
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == playerId) {
                    Debug.LogWarning($"Player already exist {playerId}");
                    return;
                }
                if (players[i] == byte.MaxValue)
                {
                    players[i] = playerId;
                    Debug.Log($"Added player {playerId} to slot {i}");
                    
                    count++;
                    Array.Sort((Array)players, 0, count);
                    ShowCountsPlayer();
                    RequestSerialization();
                    return;
                }
            }
            
            Debug.LogWarning($"No available slots for player {playerId}");
        }
        public void RemovePlayerRequest(byte playerId) {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(RemovePlayer), playerId);
        }
        [NetworkCallable] public void RemovePlayer(byte playerId) {
            if (playerId == byte.MaxValue)
            {
                Debug.LogWarning("Cannot remove player with ID 255 (reserved for 'no player')");
                return;
            }
            
            if (players == null)
            {
                Debug.LogWarning("Players array is not initialized");
                return;
            }
            
            // Find and remove the player
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == byte.MaxValue)
                    break; // No more players to check
                    
                if (players[i] == playerId)
                {
                    players[i] = byte.MaxValue; // Set to "no player"
                    Debug.Log($"Removed player {playerId} from slot {i}");
                    
                    // Sort array to move zeros to the end
                    Array.Sort((Array)players, 0, count);
                    count--;
                    ShowCountsPlayer();
                    RequestSerialization();
                    return;
                }
            }
            
            Debug.LogWarning($"Player {playerId} not found in array");
            return;
        }
        public void AddReadyRequest() {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(AddReady));
        }
        [NetworkCallable] public void AddReady() {
            indexReady++;
            RequestSerialization();
            StartTimerStart();
        }
        public void RemoveReadyRequest() {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(RemoveReady));
        }
        [NetworkCallable] public void RemoveReady() {
            if (indexReady == 0) {
                Debug.LogWarning("indexReady is already 0, can't make it lower");
            } else {
                indexReady--;
                RequestSerialization();
            }
            StartTimerStart();
        }
        void StartTimerStart() {
            if (isInProcess == true) return;
            if (count <= 1) {
                timerStart.StopTimer();
                return;
            }
            if (indexReady < count) {
                timerStart.StopTimer();
                return;
            }
            if (indexReady >= count) {
                timerStart.StartTimer();
            }
        }
        public void EnableTextTimer(bool value) {
            for (int i = 0; i < count; i++) {
                machines[players[i]].main.multiplayerMenu.ShowStartingTimer(value);
            }
        }
        public void SetTextTimer(string text) {
            for (int i = 0; i < count; i++) {
                machines[players[i]].main.multiplayerMenu.SetTimer(text);
            }
        }
        public void StartMultiplayerGame() {
            //isGameRunning = 1;
            CopyArrayIndexesToPlayersAlive();
            countAlive = count;
            for (int i = 0; i < count; i++) {
                machines[players[i]].main.multiplayerMenu.StartGameRequest();
            }
            ShowPlayerCountAlive();
            isInProcess = true;
            RequestSerialization();
        }
        void CopyArrayIndexesToPlayersAlive() {
            for (int i = 0; i < count; i++) {
                playersAlive[i] = players[i];
            }
        }

        public void ShowCountsPlayer() {
            for (int i = 0; i < count; i++) {
                machines[players[i]].main.multiplayerMenu.SetPlayerCount(count);
                machines[players[i]].main.multiplayerMenu.SetPlayerCountReady(count);
            }
        }
        public void PlayerDiedRequest(byte myId) {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(PlayerDied), myId);
        }
        [NetworkCallable] public void PlayerDied(byte myId) {
            if (myId == byte.MaxValue)
            {
                Debug.LogWarning("Cannot remove playerAlive with ID 255 (reserved for 'no player')");
                return;
            }
            
            if (playersAlive == null)
            {
                Debug.LogWarning("PlayersAlive array is not initialized");
                return;
            }
            
            // Find and remove the player
            for (int i = 0; i < playersAlive.Length; i++)
            {
                if (playersAlive[i] == byte.MaxValue)
                    break; // No more players to check
                    
                if (playersAlive[i] == myId)
                {
                    playersAlive[i] = byte.MaxValue; // Set to "no player"
                    Debug.Log($"Removed playerAlive {myId} from slot {i}");
                    
                    // Sort array to move zeros to the end
                    Array.Sort((Array)playersAlive, 0, countAlive);
                    countAlive--;
                    ShowPlayerCountAlive();
                    ProcessLastPlayer();
                    RequestSerialization();
                    return;
                }
            }
        }
        public void ShowPlayerCountAlive() {
            for (int i = 0; i < countAlive; i++) {
                machines[playersAlive[i]].playersAlive.SetValue(countAlive);
            }
        }
        public void ProcessLastPlayer() {
            if (countAlive == 1) {
                machines[playersAlive[0]].GameWinRequest();
                isInProcess = false;
                RequestSerialization();
            }
        }
        public override void OnDeserialization() {
            base.OnDeserialization();
            ShowCountsPlayer();
            StartTimerStart();
            ShowPlayerCountAlive();
        }
        public T04oGameProcess GetRandomPlayerOpponent(byte myId) {
            if (countAlive == 1) return machines[myId];

            int randomNumber = UnityEngine.Random.Range(0, countAlive);
            if (randomNumber == myId) {
                if (randomNumber == 0) randomNumber += 1;
                else if (randomNumber == count - 1) randomNumber -= 1;
                else if (UnityEngine.Random.Range(0, 2) == 0) randomNumber -= 1;
                else randomNumber += 1;
            }
            return machines[playersAlive[randomNumber]];
        }

    }
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oMultiplayer))]
    public class T04oMultiplayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oMultiplayer myTarget = (T04oMultiplayer)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Get all arcade machines and set ids"))
            {
                myTarget.machines = FindObjectsByType<T04oGameProcess>(FindObjectsSortMode.None);
                byte id = 0;
                foreach (T04oGameProcess machine in myTarget.machines) {
                    machine.id = id;
                    id++;
                    machine.main.multiplayer = myTarget;
                    EditorUtility.SetDirty(machine.main);
                    EditorUtility.SetDirty(machine);
                }
                myTarget.players = new byte[myTarget.machines.Length];
                myTarget.playersAlive = myTarget.players;
                for(int i = 0; i < myTarget.players.Length; i++) {
                    myTarget.players[i] = byte.MaxValue;
                }
                EditorUtility.SetDirty(myTarget);
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
    #endif

}
