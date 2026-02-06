using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.SDK3.Components;
namespace TETR04o {
    public class T04oGStickGrab : UdonSharpBehaviour
    {
        public T04oGStick stick;
        public VRCPickup pickup;
        public override void OnPickup()
        {
            base.OnPickup();
            stick.ActivateStickDetectionMovement();
            stick.main.controls.StartedUsing(Networking.LocalPlayer);
        }
        public override void OnDrop()
        {
            base.OnDrop();
            stick.DeactivateStickDetectionMovement();
            stick.main.controls.LeaveControls();
        }
    }
}
