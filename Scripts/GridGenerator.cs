using UnityEngine;
using System.Collections.Generic;
using VRC;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TETR04o {
public class GridGenerator : MonoBehaviour
{
    [SerializeField] private T04oCell cellPrefab;
    [SerializeField] private T04oLine linePrefab;
    [SerializeField] private T04oGameField fieldPrefab;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float padding = 0.1f;
    
    private Transform gridContainer;
    #if UNITY_EDITOR
    public void GenerateGrid()
    {
        // Clear existing grid if any
        if (gridContainer != null)
        {
            DestroyImmediate(gridContainer.gameObject);
        }

        // Create container for grid cells
        T04oGameField gameField = PrefabUtility.InstantiatePrefab(fieldPrefab, this.transform) as T04oGameField;

        gameField.transform.SetParent(transform);
        gameField.transform.localPosition = Vector3.zero;

        Vector3 cellPosition = new Vector3(0, 0, 0);
        List<T04oCell> cells = new List<T04oCell>();
        List<T04oLine> lines = new List<T04oLine>();
        // Generate grid cells
        for (int y = 0; y < gridHeight; y++)
        {
            T04oLine line = PrefabUtility.InstantiatePrefab(linePrefab, gameField.transform) as T04oLine;
            float yPos = -y * (cellSize + padding);
            cellPosition = new Vector3(0, yPos, 0);
            line.transform.localPosition = cellPosition;
            line.name = $"Line_{y}";
            for (int x = 0; x < gridWidth; x++)
            {
                // Calculate position with padding
                float xPos = x * (cellSize + padding);

                cellPosition = new Vector3(xPos, 0, 0);

                // Instantiate prefab using PrefabUtility
                T04oCell cell = PrefabUtility.InstantiatePrefab(cellPrefab, line.transform) as T04oCell;
                
                if (cell != null)
                {
                    cell.transform.localPosition = cellPosition;
                    cell.name = $"Cell_{x}_{y}";
                    cell.position = new Vector2Int(x,y);
                    cell.line = line;
                    cells.Add(cell);
                }
            }
            line.cells = cells.ToArray();
            line.cellsColors = new int[line.cells.Length];
            line.gameField = gameField;
            line.MarkDirty();
            lines.Add(line);
            cells.Clear();
        }
        gameField.lines = lines.ToArray();
        gameField.MarkDirty();
        lines.Clear();
    }
    #endif

    public void ClearGrid()
    {
        if (gridContainer != null)
        {
            DestroyImmediate(gridContainer.gameObject);
            gridContainer = null;
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGenerator = (GridGenerator)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Grid", GUILayout.Height(40)))
        {
            gridGenerator.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid", GUILayout.Height(40)))
        {
            gridGenerator.ClearGrid();
        }
    }
}
#endif
}