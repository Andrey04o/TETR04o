using System.Numerics;
using UnityEngine;
using VRC;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oWallKick : UdonSharpBehaviour
    {
        // 0->R
        public Vector2Int[] data0toR = {new Vector2Int(0,0), new Vector2Int(-1,0), new Vector2Int(-1,1), new Vector2Int(0,-2), new Vector2Int(-1,-2)};
        
        // R->0
        public Vector2Int[] dataRto0 = {new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,2), new Vector2Int(1,2)};
        
        // R->2
        public Vector2Int[] dataRto2 = {new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,2), new Vector2Int(1,2)};
        
        // 2->R
        public Vector2Int[] data2toR = {new Vector2Int(0,0), new Vector2Int(-1,0), new Vector2Int(-1,1), new Vector2Int(0,-2), new Vector2Int(-1,-2)};
        
        // 2->L
        public Vector2Int[] data2toL = {new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(0,-2), new Vector2Int(1,-2)};
        
        // L->2
        public Vector2Int[] dataLto2 = {new Vector2Int(0,0), new Vector2Int(-1,0), new Vector2Int(-1,-1), new Vector2Int(0,2), new Vector2Int(-1,2)};
        
        // L->0
        public Vector2Int[] dataLto0 = {new Vector2Int(0,0), new Vector2Int(-1,0), new Vector2Int(-1,-1), new Vector2Int(0,2), new Vector2Int(-1,2)};
        
        // 0->L
        public Vector2Int[] data0toL = {new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(0,-2), new Vector2Int(1,-2)};

        void Start() {
            
        }

        public void InvertY() {
            FixUpY(data0toR);
            FixUpY(dataRto0);
            FixUpY(dataRto2);
            FixUpY(data2toR);
            FixUpY(data2toL);
            FixUpY(dataLto2);
            FixUpY(dataLto0);
            FixUpY(data0toL);
        }
        public void InvertX() {
            FixX(data0toR);
            FixX(dataRto0);
            FixX(dataRto2);
            FixX(data2toR);
            FixX(data2toL);
            FixX(dataLto2);
            FixX(dataLto0);
            FixX(data0toL);
        }

        void FixUpY(Vector2Int[] data) {
            for(int i = 0; i < data.Length; i++) {
                data[i].y = data[i].y * -1;
            }
        }
        void FixX(Vector2Int[] data) {
            for(int i = 0; i < data.Length; i++) {
                data[i].x = data[i].x * -1;
            }
        }
        public bool Move(T04oPiece piece, bool clockwise) {
            Vector2Int[] dataWallKick = null;

            if (piece.rotation == 0) {
                if (clockwise) dataWallKick = data0toR;
                else dataWallKick = data0toL;
            } else if (piece.rotation == 1) {
                if (clockwise) dataWallKick = dataRto2;
                else dataWallKick = dataRto0;
            } else if (piece.rotation == 2) {
                if (clockwise) dataWallKick = data2toL;
                else dataWallKick = data2toR;
            } else if (piece.rotation == 3) {
                if (clockwise) dataWallKick = dataLto0;
                else dataWallKick = dataLto2;
            }

            if (dataWallKick == null) return false;
            foreach (Vector2Int dir in dataWallKick) {
                if (piece.Move(dir)) {
                    return true;
                }
            }
            return false;
        }
        public Vector2Int[] GetDataWallKick(T04oPiece piece, bool clockwise) {
            Vector2Int[] dataWallKick = null;
            
            if (piece.rotation == 0) {
                if (clockwise) dataWallKick = data0toR;
                else dataWallKick = data0toL;
            } else if (piece.rotation == 1) {
                if (clockwise) dataWallKick = dataRto2;
                else dataWallKick = dataRto0;
            } else if (piece.rotation == 2) {
                if (clockwise) dataWallKick = data2toL;
                else dataWallKick = data2toR;
            } else if (piece.rotation == 3) {
                if (clockwise) dataWallKick = dataLto0;
                else dataWallKick = dataLto2;
            }
            return dataWallKick;
        }
    }
}