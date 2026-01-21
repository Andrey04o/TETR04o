using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
public class T04oControlsAll : UdonSharpBehaviour
{
    public T04oGStickGrab[] stickGrabs;
    public T04oGButton[] buttons;
    public T04oGStation[] stations;
    public T04oGStickDetectMovement[] stickDetectMovements;
    public T04oGPCControl[] pcControls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartInteractions() {
        DisableInteractions(true);
        DisableInteractions(false);
    }
    public void DisableInteractions(bool value) {
        if (Networking.LocalPlayer.IsUserInVR() || value == true) {
            foreach (T04oGStickGrab stickGrab in stickGrabs) {
                stickGrab.pickup.pickupable = !value;
            }
            foreach (T04oGButton button in buttons) {
                button.DisableInteractive = value;
            }
        }
        if (Networking.LocalPlayer.IsUserInVR() == false || value == true) {
            foreach (T04oGStation station in stations) {
                station.gameObject.SetActive(!value);
            }
        }

    }
    public void SetSpeedRepeatingKeys(int value) {
        float repeatRate = 1 / (1 + value * 0.1f);
        foreach (T04oGStickDetectMovement stickDetectMovement in stickDetectMovements) {
            stickDetectMovement.timeRepeatKey = repeatRate;
        }
        foreach (T04oGPCControl pcControl in pcControls) {
            pcControl.timeRepeatKey = repeatRate;
        }
    }

    public void DisableRepeatingUpKey(bool value) {
        foreach (T04oGStickDetectMovement stickDetectMovement in stickDetectMovements) {
            stickDetectMovement.isDisableRepeatUp = value;
        }
        foreach (T04oGPCControl pcControl in pcControls) {
            pcControl.isDisableRepeatUp = value;
        }
    }
}
