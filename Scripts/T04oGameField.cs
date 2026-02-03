using System;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
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
            if (countLinesCleared == 0) {
                gameProcess.ResetCombo();
                return;
            }
            gameProcess.AddCombo();
            gameProcess.garbageSender.SendGarbage((ClearType)countLinesCleared);
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

        public void AddGarbageLines(int garbageCount, int emptyPosition) {
            if (garbageCount <= 0 || garbageCount >= lines.Length) return;

            // Check if the top lines (that will be pushed up) are clear
            // If any of them have blocks, trigger game over
            for (int i = 0; i < garbageCount; i++) {
                T04oLine topLine = GetLineAtVisualPosition(i);
                if (topLine != null && topLine.cellsFilled > 0) {
                    gameProcess.GameOver();
                    return;
                }
            }

            // Build new index mapping - reverse of ProcessClearedLines
            byte[] newLineIndexes = new byte[lines.Length];
            
            // First, move existing lines up by garbageCount positions
            for (int visualPos = 0; visualPos < lines.Length - garbageCount; visualPos++) {
                byte lineArrayIdx = lineIndexes[visualPos + garbageCount];
                newLineIndexes[visualPos] = lineArrayIdx;
                lines[lineArrayIdx].lineIndex = visualPos;
                
                float yPos = -visualPos * (cellSize + padding);
                lines[lineArrayIdx].transform.localPosition = new Vector3(0, yPos, 0);
            }

            // Then, place the bottom lines (which become garbage) at the bottom positions
            for (int i = 0; i < garbageCount; i++) {
                int bottomVisualPos = lines.Length - garbageCount + i;
                byte lineArrayIdx = lineIndexes[i];
                newLineIndexes[bottomVisualPos] = lineArrayIdx;
                lines[lineArrayIdx].lineIndex = bottomVisualPos;
                
                float yPos = -bottomVisualPos * (cellSize + padding);
                lines[lineArrayIdx].transform.localPosition = new Vector3(0, yPos, 0);
                
                // Fill the garbage line with random blocks (leaving one gap for strategy)
                FillGarbageLine(lines[lineArrayIdx], emptyPosition);
            }

            // Update the main index array
            System.Array.Copy(newLineIndexes, lineIndexes, lines.Length);

            CalcEmptyLinesHash();
            RequestSerialization();
        }

        private void FillGarbageLine(T04oLine line, int emptyPosition) {
            // Clear the line first
            line.Clear();
            
            // Choose a random position to leave empty (0-9)
            //int emptyPosition = UnityEngine.Random.Range(0, line.cells.Length);
            
            // Fill all positions except the empty one with random colors (1-7)
            for (int i = 0; i < line.cells.Length; i++) {
                if (i != emptyPosition) {
                    //int randomColor = UnityEngine.Random.Range(1, 8); // Colors 1-7
                    int randomColor = 9; // Garbage color 
                    line.cellsColors[i] = randomColor;
                    line.cells[i].SetCubeNetwork(randomColor);
                    line.cellsFilled++;
                }
            }
            
            // Update network data
            line.EncodeColorsNetwork();
            line.RequestSerialization();
        }

        // Public method to add garbage lines (can be called from multiplayer or other systems)
        public void ReceiveGarbageAttack(int lineCount) {
            if (!gameProcess.isGameRunning) return;
            int emptyPosition = UnityEngine.Random.Range(0, lines[0].cells.Length);
            AddGarbageLines(lineCount, emptyPosition);
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
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oGameField))]
    public class T04oGameFieldEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oGameField myTarget = (T04oGameField)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Spawn 1 garbage line"))
            {
                myTarget.ReceiveGarbageAttack(1);
            }

            if (GUILayout.Button("Spawn 4 garbage lines"))
            {
                myTarget.ReceiveGarbageAttack(4);
            }
        }
    }
    #endif
}