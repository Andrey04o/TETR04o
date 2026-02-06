using UnityEngine;
using VRC;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oPiece : UdonSharpBehaviour
    {
        public T04oPieceLine[] pieceLines;
        public Vector2 pointRotation;
        public Vector2Int position = Vector2Int.zero;
        private T04oGameField gameField;
        public T04oWallKick wallKick;
        public int rotation = 0;
        public int colorPiece = 0;
        public int index = 0;
        public T04oPiece pieceGhost;
        [UdonSynced] byte syncedXpos = 0;
        [UdonSynced] byte syncedYpos = 0;
        [UdonSynced] byte syncedRotation = 0;
        [UdonSynced] byte isUsing = 0;
        public void Spawn(Vector2Int position, T04oGameField gameField, bool showGhost = true) {
            this.gameField = gameField;
            this.position = position;
            int y = 0;
            int x = 0;
            rotation = 0;
            foreach(T04oPieceLine pieceline in pieceLines) {
                x = 0;
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        x++;
                        continue;
                    }
                    pieceCell.Set(gameField.GetLineAtVisualPosition(position.y + y).cells[position.x + x]);
                    pieceCell.positionRotated = pieceCell.position;
                    pieceCell.positionRotatedCheck = pieceCell.positionRotated;
                    x++;
                }
                y++;
            }
            isUsing = 1;
            SyncVariables();
            if (showGhost) {
                HardDropGhost();
            }
        }
        public void SetGamefield(T04oGameField gameField) {
            this.gameField = gameField;
        }
        void SyncVariables() {
            //syncedXpos = (byte)position.x;
            //syncedYpos = (byte)position.y;
            syncedRotation = (byte)rotation;
            SyncPositionCells();
            RequestSerialization();
            //FakeSerialization();
        }
        [UdonSynced] byte[] syncPosCelsX = new byte[4];
        [UdonSynced] byte[] syncPosCelsY = new byte[4];
        [UdonSynced] sbyte[] syncPosRotCelsX = new sbyte[4];
        [UdonSynced] sbyte[] syncPosRotCelsY = new sbyte[4];
        void FakeSerialization() {
            ShowPositionCellsNetwork();
        }
        void SyncPositionCells() {
            int i = 0;
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        continue;
                    }
                    syncPosCelsX[i] = (byte)pieceCell.cellLocation.position.x;
                    syncPosCelsY[i] = (byte)pieceCell.cellLocation.position.y;
                    syncPosRotCelsX[i] = (sbyte)pieceCell.positionRotated.x;
                    syncPosRotCelsY[i] = (sbyte)pieceCell.positionRotated.y;
                    i++;
                }
            }
        }
        void ShowPositionCellsNetwork() {
            if (gameField == null) return;
            int i = 0;
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        continue;
                    }
                    pieceCell.Set(gameField.lines[syncPosCelsY[i]].cells[syncPosCelsX[i]]);
                    pieceCell.positionRotated.x = syncPosRotCelsX[i];
                    pieceCell.positionRotated.y = syncPosRotCelsY[i];
                    i++;
                }
            }
        }
        void ShowPieceNetwork() {
            if (isUsing == 0) return;
            ClearCellSafe();
            position.x = syncedXpos;
            position.y = syncedYpos;
            rotation = syncedRotation;
            
            ShowPositionCellsNetwork();
        }
        void SetPiecePositionNetwork() {
            // Apply rotation to piece cells
            RotatePiece();
            
            // Redraw the piece at the new position and rotation
            int y = 0;
            int x = 0;
            foreach(T04oPieceLine pieceline in pieceLines) {
                x = 0;
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        x++;
                        continue;
                    }
                    T04oCell targetCell = gameField.GetLineAtVisualPosition(position.y + pieceCell.positionRotated.y).cells[position.x + pieceCell.positionRotated.x];
                    pieceCell.Set(targetCell);
                    pieceCell.positionRotatedCheck = pieceCell.positionRotated;
                    x++;
                }
                y++;
            }
        }
        public void Draw(Vector2Int position, T04oGameField gameField) {
            //this.gameField = gameField;
            int y = 0;
            int x = 0;
            foreach(T04oPieceLine pieceline in pieceLines) {
                x = 0;
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        x++;
                        continue;
                    }
                    pieceCell.Draw(gameField.GetLineAtVisualPosition(position.y + y).cells[position.x + x]);
                    x++;
                }
                y++;
            }
        }
        public bool Move(Vector2Int direction, bool showGhost = true)
        {
            if (IsCanMove(direction) == false) return false;
            //position += direction;
            position.x += direction.x;
            position.y += -direction.y;
            ChangePosition();
            SyncVariables();
            if (showGhost) {
                HardDropGhost();
            }
            return true;
        }
        public bool MoveDontSync(Vector2Int direction, bool showGhost = true) {
            if (IsCanMove(direction) == false) return false;
            //position += direction;
            position.x += direction.x;
            position.y += -direction.y;
            ChangePosition();
            if (showGhost) {
                HardDropGhost();
            }
            return true;
        }
        public void SoftDrop() {
            if (gameField.gameProcess.main.handling.sdf == 0) {
                SonicDrop();
                return;
            }
            if (Move(Vector2Int.down) == false) return;
            gameField.gameProcess.scoring.AddSoftDrop();
        }
        public void SonicDrop() {
            while (MoveDontSync(Vector2Int.down, false)) {
                gameField.gameProcess.scoring.AddSoftDropDontSync();
            }
            gameField.gameProcess.scoring.SetScore();
            SyncVariables();
        }
        public bool MoveDontSyncGhost(Vector2Int direction)
        {
            if (IsCanMoveGhost(direction) == false) return false;
            //position += direction;
            //position.x += direction.x;
            //position.y += -direction.y;
            ChangePositionGhost();
            return true;
        }


        public bool Rotate(bool clockwise = true, bool showGhost = true)
        {
            if (IsCanRotate(clockwise) == false) {
                SetRotationBack();
                return false;
            }
            ChangePosition();
            if (clockwise == true) {
                rotation++;
                rotation%=4;
            } else {
                rotation--;
                if (rotation < 0) rotation = 3;
            }
            //Debug.Log("rotation " + rotation);
            SyncVariables();
            if (showGhost) {
                HardDropGhost();
            }
            return true;
        }
        void SetRotationBack() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        continue;
                    }
                    pieceCell.positionRotatedCheck = pieceCell.positionRotated;
                }
            }
        }
        private void ClearCellSafe() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    if (pieceCell.cellLocation == null) return;
                    if (pieceCell.cellLocation.isFilled) return;
                    pieceCell.cellLocation.Clear();
                }
            }
        }
        private void ClearCell() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.cellLocation.Clear();
                }
            }
        }
        public void ClearCellGhost() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    if (pieceCell.cellGhost == null) continue;
                    pieceCell.cellGhost.ClearGhost();
                }
            }
        }
        private void ChangePosition() {
            ClearCell();
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.Set(pieceCell.cellCheck);
                    pieceCell.positionRotated = pieceCell.positionRotatedCheck;
                }
            }
        }
        private void ChangePositionGhost() {
            ClearCellGhost();
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.SetGhost(pieceCell.cellGhostCheck);
                    //pieceCell.positionRotated = pieceCell.positionRotatedCheck;
                }
            }
        }
        public bool IsCanMove(Vector2Int direction) {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.cellCheck = pieceCell.cellLocation.GetNeighbour(direction);
                    if (pieceCell.cellCheck == null) {
                        return false;
                    }
                    if (pieceCell.cellCheck.isFilled == true) {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool IsCanMoveGhost(Vector2Int direction) {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.cellGhostCheck = pieceCell.cellGhost.GetNeighbour(direction);
                    if (pieceCell.cellGhostCheck == null) {
                        return false;
                    }
                    if (pieceCell.cellGhostCheck.isFilled == true) {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool CheckWallKick(Vector2Int direction) {
            
            Debug.Log(direction);
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    //Debug.Log(pieceCell.cellCheckRotated);
                    pieceCell.cellCheck = pieceCell.cellLocation.GetNeighbourByOffset(pieceCell.movementRotation + direction);
                    if (pieceCell.cellCheck == null) {
                        return false;
                    }
                    if (pieceCell.cellCheck.isFilled == true) {
                        return false;
                    }
                }
            }
            return true;
        }

        private void GetCellCheckRotated(bool clockwise) {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    
                    // Calculate offset from rotation point using fractional coordinates
                    Vector2 cellOffsetFloat = pieceCell.positionRotated - pointRotation;
                    
                    // Rotate the offset
                    Vector2 rotatedOffsetFloat = RotateOffsetFloat(cellOffsetFloat, clockwise);
                    
                    // Calculate the new position with fractional rotation point
                    Vector2 newPositionFloat = rotatedOffsetFloat + pointRotation;
                    Vector2Int newPosition = Vector2Int.RoundToInt(newPositionFloat);
                    
                    // Calculate the movement needed
                    pieceCell.movementRotation = newPosition - pieceCell.positionRotated;
                    
                    //pieceCell.cellCheckRotated = pieceCell.cellLocation.GetNeighbourByOffset(movement);

                    /*
                    if (pieceCell.cellCheck == null || pieceCell.cellCheck.isFilled == true) {
                        return false;
                    }
                    */
                    
                    pieceCell.positionRotatedCheck = newPosition;
                }
            }
        }
        private bool IsCanRotate(bool clockwise)
        {
            GetCellCheckRotated(clockwise);
            int counter = 0;
            foreach (Vector2Int dir in wallKick.GetDataWallKick(this, clockwise)) {
                counter++;
                Debug.Log(counter);
                if (CheckWallKick(dir)) {
                    //Debug.Log("wallkick " + counter);
                    //position += dir;
                    position.x += dir.x;
                    position.y += -dir.y;
                    if (counter > 1) {
                        PerfomWallKick();
                    }        
                    return true;
                }
            }
            
            //Debug.Log("cant wallkick");
            return false;
        }
        public void PerfomWallKick() {
            gameField.gameProcess.main.controlsHandling.SetDcd();
        }
        public void HardDrop() {
            int countLines = 0;
            while (Move(Vector2Int.down, false)) {
                countLines++;
            }
            gameField.gameProcess.scoring.AddHardDrop(countLines);
            Place();
            gameField.gameProcess.SpawnNewPiece();
        }
        public void HardDropGhost() {
            ClearCellGhost();
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) {
                        continue;
                    }
                    pieceCell.cellGhost = pieceCell.cellLocation;
                }
            }
            while (MoveDontSyncGhost(Vector2Int.down)) {

            }
        }
        private Vector2Int RotateOffset(Vector2Int offset, bool clockwise)
        {
            if (clockwise) {
                return new Vector2Int(-offset.y, offset.x);
            } else {
                return new Vector2Int(offset.y, -offset.x);
            }
        }

        private Vector2 RotateOffsetFloat(Vector2 offset, bool clockwise)
        {
            if (clockwise) {
                return new Vector2(-offset.y, offset.x);
            } else {
                return new Vector2(offset.y, -offset.x);
            }
        }

        private void RotatePiece() {
            // Apply rotation based on the rotation variable (0-3)
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    
                    // Calculate offset from rotation point using fractional coordinates
                    Vector2 cellOffsetFloat = pieceCell.position - pointRotation;
                    
                    // Apply rotation based on rotation variable
                    for (int i = 0; i < rotation; i++) {
                        cellOffsetFloat = RotateOffsetFloat(cellOffsetFloat, true);
                    }
                    
                    // Calculate the new position with fractional rotation point
                    Vector2 newPositionFloat = cellOffsetFloat + pointRotation;
                    pieceCell.positionRotated = Vector2Int.RoundToInt(newPositionFloat);
                }
            }
        }
        public void Place() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.Place();
                }
            }
            //gameField.ClearLinesIfPossible();
            gameField.ProcessClearedLines();
            isUsing = 0;
            gameField.gameProcess.main.controlsHandling.SetDcd();
            SyncVariables();
        }
        public void Draw() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    //pieceCell.Draw();
                }
            }
            SyncVariables();
        }
        public void Clear() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    pieceCell.cellLocation.Clear();
                }
            }
        }
        public void ClearSafe() {
            ClearCellGhost();
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell pieceCell in pieceline.pieceCells) {
                    if (pieceCell == null) continue;
                    if (pieceCell.cellLocation == null) continue;
                    pieceCell.cellLocation.ClearSafe();
                }
            }
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            ShowPieceNetwork();
        }

        #if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void SetCellOffset()
        {
            int cellY = 0;
            int cellX = 0;
            
            foreach(T04oPieceLine pieceline in pieceLines) {
                cellX = 0;
                foreach(T04oPieceCell cell in pieceline.pieceCells) {
                    if (cell == null) {
                        cellX++;
                        continue;
                    }
                    cell.position = new Vector2Int(cellX, cellY);
                    cellX++;
                    cell.MarkDirty();
                }
                cellY++;
            }
        }
        public void SetPieceColorsToCells() {
            foreach(T04oPieceLine pieceline in pieceLines) {
                foreach(T04oPieceCell cell in pieceline.pieceCells) {
                    if (cell == null) {
                        continue;
                    }
                    cell.colorPiece = colorPiece;
                    cell.MarkDirty();
                }
            }
        }
        #endif
    }
}