using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oResizerTimer : UdonSharpBehaviour
    {
        public T04oResizerButton resizerButton;
        public float timerWait = 10f;
        public float timerCurrent = 0f;

        // Update is called once per frame
        void Update()
        {
            if (resizerButton.main.isUsing == 0) {
                timerCurrent += Time.deltaTime;
                if (timerCurrent >= timerWait) {
                    resizerButton.SetOriginalSize();
                    StopTimer();
                }
            }
            
        }

        public void ResetTimer() {
            timerCurrent = 0f;
        }
        public void StartTimer() {
            ResetTimer();
            gameObject.SetActive(true);
        }
        public void StopTimer() {
            ResetTimer();
            gameObject.SetActive(false);
        }
    }
}
