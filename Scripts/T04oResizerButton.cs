using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oResizerButton : UdonSharpBehaviour
    {
        public T04oMain main;
        public T04oResizerTimer resizerTimer;
        public Vector3 vectorSizeOriginal;
        [UdonSynced] public Vector3 vectorCurrentSize;
        public GameObject resizerObject;
        public float dividerNumber = 51.6666f;
        public float maxPlayerSize = 2.0f;
        void Start() {
            vectorSizeOriginal = resizerObject.transform.localScale;
            vectorCurrentSize = vectorSizeOriginal;
            //SetOriginalSize();
        }
        public override void Interact()
        {
            base.Interact();
            main.SetOwner(Networking.LocalPlayer);
            float size = Mathf.Clamp(Networking.LocalPlayer.GetAvatarEyeHeightAsMeters(), 0, maxPlayerSize) / dividerNumber;
            vectorCurrentSize = new Vector3(size, size, size);
            SetSize(vectorCurrentSize);
            resizerTimer.StartTimer();
            RequestSerialization();
        }

        public void SetSize(Vector3 size) {
            if (vectorCurrentSize == Vector3.zero) return;
            resizerObject.transform.localScale = size;
        }
        public void SetOriginalSize() {
            vectorCurrentSize = vectorSizeOriginal;
            resizerObject.transform.localScale = vectorSizeOriginal;
            RequestSerialization();
        }


        public override void OnDeserialization()
        {
            base.OnDeserialization();
            SetSize(vectorCurrentSize);
        }
        public void DisableInteraction(bool value) {
            DisableInteractive = value;
            if (value == false) {
                StartTheTimerIfSizeDifferent();
            }
        }

        public void StartTheTimerIfSizeDifferent() {
            if (vectorSizeOriginal != vectorCurrentSize) {
                resizerTimer.StartTimer();
            }
        }
        
    }
}