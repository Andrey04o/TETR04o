using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oControls : UdonSharpBehaviour
    {
        public bool isInterface = false;

        public virtual void Up() {
        }
        public virtual void Down() {
        }
        public virtual void Left() {
        }
        public virtual void Right() {
        }
        public virtual void RotateLeft() {
        }
        public virtual void RotateRight() {
        }
        public virtual void SpaceBar() {
        }
        public virtual void Respawn() {
        }
        public virtual void StartedUsing(VRCPlayerApi playerApi) {
        }
        public virtual void LeaveControls() {
        }

        
    }
}
