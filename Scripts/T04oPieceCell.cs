using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
public class T04oPieceCell : UdonSharpBehaviour
{
    public MeshRenderer meshRenderer;
    public T04oCell cellLocation;
    public T04oCell cellCheck;
    public T04oCell cellGhost;
    public T04oCell cellGhostCheck;
    //public T04oCell cellCheckRotated;
    public Vector2Int position;
    public int colorPiece = 0;
    [HideInInspector] public Vector2Int positionRotated;
    [HideInInspector] public Vector2Int movementRotation = Vector2Int.zero;
    [HideInInspector] public Vector2Int positionRotatedCheck;

    public void Set(T04oCell cell) {
        cellLocation = cell;
        cell.ShowCube(meshRenderer.material);
    }
    public void SetGhost(T04oCell cell) {
        cellGhost = cell;
        cell.DrawGhost();
    }
    public void Draw(T04oCell cell) {
        cell.Draw(meshRenderer.material);
    }

    public void Place() {
        cellLocation.SetCube(colorPiece);
    }
    public void Clear() {
        cellLocation.Clear();
    }
}
