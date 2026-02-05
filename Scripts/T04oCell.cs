using Unity.Mathematics;
using UnityEngine;
using UdonSharp;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oCell : UdonSharpBehaviour
    {
        public MeshRenderer[] meshRenderers;
        public Material materialRenderer;
        public Material materialWhite;
        public bool isFilled;
        public bool isUnderPiece;
        public Vector2Int position;
        public T04oLine line;
        private T04oGameField gameField;
        int indexColor = 0;

        private void Start() {
            gameField = line.gameField;
            //SetMaterialMeshRenderers();
        }

        public void SetMaterialMeshRenderers(Material material) {
            //materialRenderer = new Material(materialRenderer);
            foreach (MeshRenderer mr in meshRenderers) {
                mr.material = material;
            }
        }

        public void SetCube(int color)
        {
            isFilled = true;
            isUnderPiece = false;
            line.CountFill(this, color);
            Draw(color);
        }
        public void SetCubeNetwork(int color) {
            isFilled = true;
            isUnderPiece = false;
            Draw(color);
        }
        public void ShowCube(Material material) {
            isUnderPiece = true;
            Draw(material);
        }
        public void Draw(Material material) {
            materialRenderer = material;
            SetMaterialMeshRenderers(material);
        }
        public void Draw(int color) {
            materialRenderer = line.gameField.colors.materials[color];
            SetMaterialMeshRenderers(materialRenderer);
        }
        public void DrawGhost() {
            if (isUnderPiece) return;
            if (isFilled) return;
            Draw(8);
        }
        public void ClearGhost() {
            if (isUnderPiece) return;
            if (isFilled) return;
            Draw(0);
        }
        public void ClearSafe() {
            if (isFilled == true) return;
            isUnderPiece = true;
            materialRenderer = materialWhite;
            SetMaterialMeshRenderers(materialRenderer);
        }
        public void Clear()
        {
            isUnderPiece = false;
            isFilled = false;
            materialRenderer = materialWhite;
            SetMaterialMeshRenderers(materialRenderer);
        }

        public T04oCell GetNeighbour(Vector2Int dir) {
            if (gameField == null) gameField = line.gameField;
            if (dir == Vector2Int.zero) {
                return this;
            }
            if (dir == Vector2Int.down) {
                return GetDown();
            }
            if (dir == Vector2Int.left) {
                return GetLeft();
            }
            if (dir == Vector2Int.right) {
                return GetRight();
            }
            if (dir == Vector2Int.up) {
                return GetUp();
            }
            return null;
        }
        public bool GetNeighbour(Vector2Int dir, out T04oCell cellOut) {
            if (gameField == null) gameField = line.gameField;
            if (dir == Vector2Int.zero) {
                cellOut = this;
                if (cellOut == null) return false;
                return true;
            }
            if (dir == Vector2Int.down) {
                cellOut = GetDown();
                if (cellOut == null) return false;
                return true;
            }
            if (dir == Vector2Int.left) {
                cellOut = GetLeft();
                if (cellOut == null) return false;
                return true;
            }
            if (dir == Vector2Int.right) {
                cellOut = GetRight();
                if (cellOut == null) return false;
                return true;
            }
            if (dir == Vector2Int.up) {
                cellOut = GetUp();
                if (cellOut == null) return false;
                return true;
            }
            cellOut = null;
            return false;
        }

        public T04oCell GetLeft() {
            int cellIndex = System.Array.IndexOf(line.cells, this);
            if (cellIndex > 0) {
                return line.cells[cellIndex - 1];
            }
            return null;
        }

        public T04oCell GetRight() {
            int cellIndex = System.Array.IndexOf(line.cells, this);
            if (cellIndex < line.cells.Length - 1) {
                return line.cells[cellIndex + 1];
            }
            return null;
        }

        public T04oCell GetUp() {
            if (gameField == null) gameField = line.gameField;
            
            int currentLineVisualPos = line.lineIndex;
            if (currentLineVisualPos <= 0) return null;
            
            T04oLine lineAbove = gameField.GetLineAtVisualPosition(currentLineVisualPos - 1);
            if (lineAbove == null) return null;
            
            int cellIndex = System.Array.IndexOf(line.cells, this);
            if (cellIndex >= 0 && cellIndex < lineAbove.cells.Length) {
                return lineAbove.cells[cellIndex];
            }
            return null;
        }

        public T04oCell GetDown() {
            if (gameField == null) gameField = line.gameField;
            
            int currentLineVisualPos = line.lineIndex;
            if (currentLineVisualPos >= gameField.lines.Length - 1) return null;
            
            T04oLine lineBelow = gameField.GetLineAtVisualPosition(currentLineVisualPos + 1);
            if (lineBelow == null) return null;
            
            int cellIndex = System.Array.IndexOf(line.cells, this);
            if (cellIndex >= 0 && cellIndex < lineBelow.cells.Length) {
                return lineBelow.cells[cellIndex];
            }
            return null;
        }

        public T04oCell GetNeighbourByOffset(Vector2Int offset) {
            Vector2Int myMovement = Vector2Int.zero;
            T04oCell currentNeighbour = this;
            int dir = 0;
            while (myMovement.x != offset.x) {
                dir = Mathf.Clamp(myMovement.x + offset.x, -1, 1);
                myMovement.x += dir;
                currentNeighbour = currentNeighbour.GetNeighbour(new Vector2Int(dir, 0));
                if (currentNeighbour == null) return null;
            }
            while (myMovement.y != offset.y) {
                dir = Mathf.Clamp(myMovement.y + offset.y, -1, 1);
                myMovement.y += dir;
                currentNeighbour = currentNeighbour.GetNeighbour(new Vector2Int(0, -dir));
                if (currentNeighbour == null) return null;
            }
            return currentNeighbour;
        }

        public void Paste(T04oCell cell) {
            isFilled = cell.isFilled;
            isUnderPiece = cell.isUnderPiece;
            Draw(cell.materialRenderer);
        }
    }
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oCell))]
    public class T04oCellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oCell myTarget = (T04oCell)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("SetMaterialMeshRenderers"))
            {
                //myTarget.SetMaterialMeshRenderers();
                //myTarget.materialRenderer = new Material(myTarget.materialWhite);
                foreach (MeshRenderer mr in myTarget.meshRenderers) {
                    
                    mr.material = myTarget.materialRenderer;
                    EditorUtility.SetDirty(mr);
                }
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
    #endif
}