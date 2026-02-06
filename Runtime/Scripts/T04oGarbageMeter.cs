using UnityEngine;
using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oGarbageMeter : UdonSharpBehaviour
    {
        public Material materialGarbageMeter;
        public Material materialGarbageEmpty;
        public T04oGarbageMeterCell[] cells;
        public T04oGameField gameField;
        public int indexMaxPushToField = 4;
        [UdonSynced] public byte indexGarbage;
        int indexGarbageInt = 0;
        public void ClearGarbage() {
            indexGarbage = 0;
            ShowGarbageCount();
            RequestSerialization();
        }
        public void ReceiveGarbageAttackRequest(byte count) {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(ReceiveGarbageAttack), count);
        }
        [NetworkCallable] public void ReceiveGarbageAttack(byte count) {
            indexGarbageInt = indexGarbage;
            indexGarbageInt += count;
            indexGarbageInt %= 255;
            indexGarbage = (byte)(indexGarbageInt);
            ShowGarbageCount();
            RequestSerialization();
        }
        public byte DecreaseGarbageAttack(byte count) {
            indexGarbageInt = indexGarbage;
            
            // Calculate how much can be decreased
            int decreaseAmount = Mathf.Min(indexGarbageInt, count);
            
            // Decrease the garbage meter
            indexGarbageInt -= decreaseAmount;
            indexGarbage = (byte)indexGarbageInt;
            
            // Calculate remaining count that couldn't be decreased
            byte remainingCount = (byte)(count - decreaseAmount);
            
            // Update visual representation
            ShowGarbageCount();
            RequestSerialization();
            
            return remainingCount;
        }
        public void PushToField() {
            indexGarbageInt = indexGarbage;
            
            // Calculate how many lines to push (maximum 4)
            int linesToPush = Mathf.Min(indexGarbageInt, indexMaxPushToField);
            
            if (linesToPush > 0) {
                // Push garbage lines to the field
                gameField.ReceiveGarbageAttack(linesToPush);
                
                // Reduce the garbage meter by the amount pushed
                indexGarbageInt -= linesToPush;
                indexGarbage = (byte)indexGarbageInt;
                
                // Update visual representation
                ShowGarbageCount();
                RequestSerialization();
            }
        }
        void ShowGarbageCount() {
            Clear();
            int index = Mathf.Clamp(indexGarbage, 0, cells.Length);
            for (int i = 0; i < index; i++) {
                cells[i].SetMaterial(materialGarbageMeter);
            }
        }
        void Clear() {
            foreach (T04oGarbageMeterCell cell in cells) {
                cell.SetMaterial(materialGarbageEmpty);
            }
        }
        public override void OnDeserialization()
        {
            base.OnDeserialization();
            ShowGarbageCount();
        }
    }
}
