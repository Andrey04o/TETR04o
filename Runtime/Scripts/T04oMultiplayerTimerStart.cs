using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oMultiplayerTimerStart : UdonSharpBehaviour
    {
        public T04oMultiplayer multiplayer;
        float timer = 44f;
        public float timeStart = 5f;
        public void StartTimer() {
            ResetTimer();
            gameObject.SetActive(true);
            multiplayer.EnableTextTimer(true);
        }
        public void StopTimer() {
            ResetTimer();
            multiplayer.EnableTextTimer(false);
            gameObject.SetActive(false);
        }
        public void ResetTimer() {
            timer = timeStart;
        }
        public void StartTheGame() {
            if (Networking.IsOwner(gameObject)) {
                multiplayer.StartMultiplayerGame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            timer -= Time.deltaTime;
            multiplayer.SetTextTimer((int)timer + "");
            if (timer <= 0) {
                StartTheGame();
                StopTimer();
            }
        }
    }
}