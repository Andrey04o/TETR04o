using System;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
namespace TETR04o {
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oGameField : UdonSharpBehaviour
    {
        public T04oLine[] lines;
        [HideInInspector] public T04oPiece pieceCurrent;
        private readonly T04oLine[] linesCleared = new T04oLine[20];
        public int countLinesCleared = 0;
        public T04oGameProcess gameProcess;
        private readonly int[] clearedLineIndices = new int[20];
        [UdonSynced] private byte[] lineIndexes;
        public T04oColors colors;
        public int gridHeight = 20;
        public float padding = 0.1f;
        public float cellSize = 1f;
        private void Start() {
            // Initialize line index mapping: lineIndexes[visual_position] = actual_line_array_index
            lineIndexes = new byte[lines.Length];
            for (byte i = 0; i < lines.Length; i++) {
                lineIndexes[i] = i;
                lines[i].lineIndex = i;
            }
        }

        public void SpawnPiece(Vector2Int position, T04oPiece piece) {
            piece.Spawn(position, this);
            pieceCurrent = piece;
        }
        public void SpawnPieceNetwork(T04oPiece piece) {
            piece.SetGamefield(this);
            pieceCurrent = piece;
        }
        public void DrawCells(T04oPiece piece) {
            piece.Draw(Vector2Int.zero, this);
            pieceCurrent = piece;
        }
        public void MovePiece(Vector2Int dir) {
            pieceCurrent.Move(dir);
        }
        public void ClearPiece() {
            pieceCurrent.Clear();
        }
        public void Clear() {
            foreach(T04oLine line in lines) {
                line.Clear();
            }
        }
        public void ClearNoSync() {
            foreach(T04oLine line in lines) {
                foreach(T04oCell cell in line.cells) {
                    cell.Clear();
                }
            }
        }

        public void AddClearedLine(T04oLine line) {
            linesCleared[countLinesCleared] = line;
            countLinesCleared++;
        }
        public void AddClearedLine(int lineIndex) {
            clearedLineIndices[countLinesCleared] = lineIndex;
            countLinesCleared++;
        }

        public T04oLine GetLineAtVisualPosition(int visualPosition) {
            if (visualPosition < 0 || visualPosition >= lines.Length) return null;
            return lines[lineIndexes[visualPosition]];
        }

        public void ProcessClearedLines() {
            if (countLinesCleared == 0) return;
            gameProcess.AddClearedLine(countLinesCleared);
            
            // Sort cleared line indices in ascending order
            Array.Sort((Array)clearedLineIndices, 0, countLinesCleared);

            // Build new index mapping all at once
            byte[] newLineIndexes = new byte[lines.Length];
            int writePos = countLinesCleared; // Start writing non-cleared lines after the cleared ones
            int clearedIdx = 0;

            // First, place cleared lines at the top (positions 0 to countLinesCleared-1)
            for (int i = 0; i < countLinesCleared; i++) {
                byte clearedLineArrayIdx = lineIndexes[clearedLineIndices[i]];
                newLineIndexes[i] = clearedLineArrayIdx;
                lines[clearedLineArrayIdx].lineIndex = i;
                
                float yPos = -i * (cellSize + padding);
                lines[clearedLineArrayIdx].transform.localPosition = new Vector3(0, yPos, 0);
                lines[clearedLineArrayIdx].Clear();
            }

            // Then, copy non-cleared lines below them
            clearedIdx = 0;
            for (int readPos = 0; readPos < lines.Length; readPos++) {
                if (clearedIdx < countLinesCleared && readPos == clearedLineIndices[clearedIdx]) {
                    // This position has a cleared line, skip it
                    clearedIdx++;
                } else {
                    // Copy this line to a position below the cleared lines
                    byte lineArrayIdx = lineIndexes[readPos];
                    newLineIndexes[writePos] = lineArrayIdx;
                    lines[lineArrayIdx].lineIndex = writePos;
                    
                    float yPos = -writePos * (cellSize + padding);
                    lines[lineArrayIdx].transform.localPosition = new Vector3(0, yPos, 0);
                    
                    writePos++;
                }
            }

            // Update the main index array
            System.Array.Copy(newLineIndexes, lineIndexes, lines.Length);

            // Reset cleared lines tracking
            for (int i = 0; i < countLinesCleared; i++) {
                clearedLineIndices[i] = 0;
            }
            countLinesCleared = 0;

            CalcEmptyLinesHash();
            RequestSerialization();
        }

        byte emptyLinesHashLocal;
        [UdonSynced] byte emptyLineHashNetwork;
        void CalcEmptyLinesHash() {
            emptyLinesHashLocal = 0;
            foreach (T04oLine line in lines) {
                if (line.cellsFilled > 0) {
                    emptyLinesHashLocal += 1;
                }
            }
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject)) {
                emptyLineHashNetwork = emptyLinesHashLocal;
            }
        }
        void CheckEmptyLinesNetwork() {
            if (emptyLineHashNetwork != emptyLinesHashLocal) {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(AskOwnerToSendArray));
            }
        }
        [NetworkCallable] public void AskOwnerToSendArray() {
            Debug.Log("for some reason lines not updated for one player, let's update cells again");
            foreach (T04oLine line in lines) {
                line.SendArrayToClientNetwork();
            }
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            CheckEmptyLinesNetwork();
            PlaceLines();
        }

        public void PlaceLines() {
        for (int visualPosition = 0; visualPosition < lines.Length; visualPosition++) {
            byte lineArrayIndex = lineIndexes[visualPosition];
            T04oLine line = lines[lineArrayIndex];
            
            float yPos = -visualPosition * (cellSize + padding);
            line.transform.localPosition = new Vector3(0, yPos, 0);
            line.lineIndex = visualPosition;
        }
    }
        /*
        public void ClearLinesIfPossible() {
            for (int i = 0; i < countLinesCleared; i++) {
                linesCleared[i].MoveDown();
                linesCleared[i] = null;
            }
            countLinesCleared = 0;
        }
        */


        




    }
}