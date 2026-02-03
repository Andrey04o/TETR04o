using UnityEngine;
using UdonSharp;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;
using YamlDotNet.Core.Tokens;

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
        [UdonSynced] public byte indexReady;
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
            if (count < 1) {
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
            for (int i = 0; i < count; i++) {
                machines[players[i]].main.multiplayerMenu.StartGameRequest();
            }
            //RequestSerialization();
        }

        public void ShowCountsPlayer() {
            for (int i = 0; i < count; i++) {
                machines[players[i]].main.multiplayerMenu.SetPlayerCount(count);
                machines[players[i]].main.multiplayerMenu.SetPlayerCountReady(count);
            }
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            ShowCountsPlayer();
            StartTimerStart();
            
        }
        public T04oGameProcess GetRandomPlayerOpponent(byte myId) {
            int randomNumber = UnityEngine.Random.Range(0, count);
            if (randomNumber == myId) {
                if (randomNumber == 0) randomNumber += 1;
                else if (randomNumber == count - 1) randomNumber -= 1;
                else if (UnityEngine.Random.Range(0, 2) == 0) randomNumber -= 1;
                else randomNumber += 1;
            }
            return machines[players[randomNumber]];
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
