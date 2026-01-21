using UnityEngine;
using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oLine : UdonSharpBehaviour
    {
        public T04oCell[] cells;
        public T04oGameField gameField;
        [UdonSynced] public byte cellsFilled = 0;
        public int lineIndex;
        [UdonSynced] public byte[] cellsColorsNetwork;
        public int[] cellsColors;
        public void GetLineUp() {
        }
        public void CountFill(T04oCell cell, int color) {
            cellsFilled++;
            cellsColors[cell.position.x] = color;
            EncodeColorsNetwork();
            RequestSerialization();

            if (IsLineFilled()) {
                gameField.AddClearedLine(lineIndex);
                cellsFilled = 0;
            }
        }
        public bool IsLineFilled() {
            if (cellsFilled >= cells.Length) {
                return true;
            }
            return false;
        }
        public void Clear() {
            cellsFilled = 0;
            foreach(T04oCell cell in cells) {
                cell.Clear();
            }
            for (int i = 0; i < cellsColors.Length; i++) {
                cellsColors[i] = 0;
            }
            EncodeColorsNetwork();
            RequestSerialization();
        }

        void SetColorsFromArray() {
            for (int i = 0; i < cells.Length; i++) {
                if (cellsColors[i] == 0){
                    cells[i].Clear();
                } else {
                    cells[i].SetCubeNetwork(cellsColors[i]);
                }
            }
        }

        public void EncodeColorsNetwork() {
            // Pack 10 colors (4 bits each) into 5 bytes
            // cellsColors array contains color values 0-7 for each cell
            cellsColorsNetwork = new byte[5];
            
            for (int i = 0; i < cells.Length; i++) {
                int byteIndex = i / 2;  // 2 colors per byte
                int bitOffset = (i % 2) * 4;  // 0 or 4 bits
                
                byte colorValue = (byte)(cellsColors[i] & 0xF);  // Ensure 4 bits
                cellsColorsNetwork[byteIndex] |= (byte)(colorValue << bitOffset);
            }
        }

        public void DecodeColorsNetwork() {
            // Unpack 5 bytes back into cellsColors array
            if (cellsColorsNetwork == null || cellsColorsNetwork.Length < 5) {
                return;
            }
            
            for (int i = 0; i < cells.Length; i++) {
                int byteIndex = i / 2;  // 2 colors per byte
                int bitOffset = (i % 2) * 4;  // 0 or 4 bits
                
                cellsColors[i] = (cellsColorsNetwork[byteIndex] >> bitOffset) & 0xF;
            }
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            
            DecodeColorsNetwork();
            SetColorsFromArray();
        }

        [NetworkCallable] public void AskOwnerToSendArray() {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SendArrayToClientNetwork));
        }
        [NetworkCallable] public void SendArrayToClientNetwork() {
            RequestSerialization();
        }

        /*
        public void MoveDown() {
            if (cells[0].up == null) {
                Clear();
                return;
            }
            cellsFilled = cells[0].up.line.cellsFilled;
            foreach (T04oCell cell in cells) {
                cell.Paste(cell.up);
            }
            cells[0].up.line.MoveDown();
        }
        */
    }
}