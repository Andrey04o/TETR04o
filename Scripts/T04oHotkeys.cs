using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using System;
using VRC;
using VRC.SDKBase;
using VRC.SDK3.Persistence;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif
namespace TETR04o {
    public enum keyType {
            none,
            left,
            right,
            up,
            down,
            confirm,
            moveLeft,
            moveRight,
            rotateLeft,
            rotateRight,
            softDrop,
            hardDrop,
            hold,
            exit
        }
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oHotkeys : UdonSharpBehaviour
    {
        public string nameForSavePersistance = "T04o";
        public int[] indexKeyCodes;
        public int[] indexLefts = new int[3];
        public int[] indexRights = new int[3];
        public int[] indexUps = new int[3];
        public int[] indexDowns = new int[3];
        public int[] indexConfirm = new int[3];
        public int[] indexRotateLefts = new int[3];
        public int[] indexRotateRights = new int[3];
        public int[] indexSoftDrops = new int[3];
        public int[] indexHardDrops = new int[3];
        public int[] indexMoveLeft = new int[3];
        public int[] indexMoveRight = new int[3];
        public int[] indexHolds = new int[3];
        public int[] indexExits = new int[3];
        public T04oHotkeyLine[] hotkeyLines;
        static public bool CheckButtonDown(int[] indexes) {
            foreach (int index in indexes) {
                if (Input.GetKeyDown((KeyCode)index)) {
                    return true;
                }
            }
            return false;
        }
        static public bool CheckButtonUp(int[] indexes) {
            foreach (int index in indexes) {
                if (index == 0) continue;
                if (Input.GetKeyUp((KeyCode)index)) {
                    return true;
                }
            }
            return false;
        }
        public void SetKey(keyType button, KeyCode key, int indexNumber = 0) {
            switch (button)
            {
                case keyType.none:
                return;
                case keyType.left:
                indexLefts[indexNumber] = (int)key;
                return;
                case keyType.right:
                indexRights[indexNumber] = (int)key;
                return;
                case keyType.up:
                indexUps[indexNumber] = (int)key;
                return;
                case keyType.down:
                indexDowns[indexNumber] = (int)key;
                return;
                case keyType.confirm:
                indexConfirm[indexNumber] = (int)key;
                return;
                case keyType.moveLeft:
                indexMoveLeft[indexNumber] = (int)key;
                return;
                case keyType.moveRight:
                indexMoveRight[indexNumber] = (int)key;
                return;
                case keyType.rotateLeft:
                indexRotateLefts[indexNumber] = (int)key;
                return;
                case keyType.rotateRight:
                indexRotateRights[indexNumber] = (int)key;
                return;
                case keyType.softDrop:
                indexSoftDrops[indexNumber] = (int)key;
                return;
                case keyType.hardDrop:
                indexHardDrops[indexNumber] = (int)key;
                return;
                case keyType.hold:
                indexHolds[indexNumber] = (int)key;
                return;
                case keyType.exit:
                indexExits[indexNumber] = (int)key;
                return;
                default:
                return;
            }
        }
        public int GetKey(keyType button, int indexNumber = 0) {
            switch (button)
            {
                case keyType.none:
                return 0;
                case keyType.left:
                return indexLefts[indexNumber];
                case keyType.right:
                return indexRights[indexNumber];
                case keyType.up:
                return indexUps[indexNumber];
                case keyType.down:
                return indexDowns[indexNumber];
                case keyType.confirm:
                return indexConfirm[indexNumber];
                case keyType.moveLeft:
                return indexMoveLeft[indexNumber];
                case keyType.moveRight:
                return indexMoveRight[indexNumber];
                case keyType.rotateLeft:
                return indexRotateLefts[indexNumber];
                case keyType.rotateRight:
                return indexRotateRights[indexNumber];
                case keyType.softDrop:
                return indexSoftDrops[indexNumber];
                case keyType.hardDrop:
                return indexHardDrops[indexNumber];
                case keyType.hold:
                return indexHolds[indexNumber];
                case keyType.exit:
                return indexExits[indexNumber];
                default:
                return 0;
            }
        }
        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            base.OnPlayerRestored(player);
            if (Networking.LocalPlayer != player) {
                return;
            }
            if (PlayerData.HasKey(player, nameForSavePersistance + keyType.left.ToString())) {
                LoadControls(player);
            }
            ShowKeys();
        }
        // Encode 3 KeyCode indexes into a byte array
        // Each KeyCode uses 2 bytes (ushort), total 6 bytes
        public byte[] EncodeControls(int index0, int index1, int index2) {
            byte[] encoded = new byte[6];
            // Store each index as 2 bytes (little-endian)
            encoded[0] = (byte)(index0 & 0xFF);
            encoded[1] = (byte)((index0 >> 8) & 0xFF);
            encoded[2] = (byte)(index1 & 0xFF);
            encoded[3] = (byte)((index1 >> 8) & 0xFF);
            encoded[4] = (byte)(index2 & 0xFF);
            encoded[5] = (byte)((index2 >> 8) & 0xFF);
            return encoded;
        }

        // Decode a byte array back into 3 KeyCode indexes
        public void DecodeControls(byte[] encoded, out int index0, out int index1, out int index2) {
            index0 = encoded[0] | (encoded[1] << 8);
            index1 = encoded[2] | (encoded[3] << 8);
            index2 = encoded[4] | (encoded[5] << 8);
        }

        // Save a control action with 3 key indexes
        public void SaveControl(VRCPlayerApi playerApi, keyType key, int index0, int index1, int index2) {
            byte[] encoded = EncodeControls(index0, index1, index2);
            PlayerData.SetBytes(nameForSavePersistance+key.ToString(), encoded);
        }

        // Load a control action and decode the 3 key indexes
        public void LoadControl(VRCPlayerApi playerApi, keyType key) {
            byte[] encoded = PlayerData.GetBytes(playerApi, key.ToString());
            if (encoded != null && encoded.Length == 6) {
                DecodeControls(encoded, out int index0, out int index1, out int index2);
                SetKey(key, (KeyCode)index0, 0);
                SetKey(key, (KeyCode)index1, 1);
                SetKey(key, (KeyCode)index2, 2);
            }
        }

        void ReadKey(VRCPlayerApi playerApi, keyType key) {
            SetKey(key, (KeyCode)PlayerData.GetUShort(playerApi, key.ToString()), 0);
        }
        public void LoadControls(VRCPlayerApi playerApi) {
            LoadControl(playerApi, keyType.left);
            LoadControl(playerApi, keyType.right);
            LoadControl(playerApi, keyType.up);
            LoadControl(playerApi, keyType.down);
            LoadControl(playerApi, keyType.confirm);
            LoadControl(playerApi, keyType.moveLeft);
            LoadControl(playerApi, keyType.moveRight);
            LoadControl(playerApi, keyType.rotateLeft);
            LoadControl(playerApi, keyType.rotateRight);
            LoadControl(playerApi, keyType.softDrop);
            LoadControl(playerApi, keyType.hardDrop);
            LoadControl(playerApi, keyType.hold);
            LoadControl(playerApi, keyType.exit);
        }
        public void ShowKeys() {
            foreach (T04oHotkeyLine hotkeyLine in hotkeyLines) {
                foreach (T04oChangeHotkeys changer in hotkeyLine.changers) {
                    changer.ReadHotkey();
                }
            }
            
        }
        public void SetDefault() {
            SetKey(keyType.left, KeyCode.A, 0);
            SetKey(keyType.left, KeyCode.LeftArrow, 1);
            SetKey(keyType.right, KeyCode.D, 0);
            SetKey(keyType.right, KeyCode.RightArrow, 1);
            SetKey(keyType.up, KeyCode.W, 0);
            SetKey(keyType.up, KeyCode.UpArrow, 1);
            SetKey(keyType.down, KeyCode.S, 0);
            SetKey(keyType.down, KeyCode.DownArrow, 1);
            SetKey(keyType.confirm, KeyCode.Space, 0);
            SetKey(keyType.confirm, KeyCode.Return, 1);
            SetKey(keyType.moveLeft, KeyCode.A, 0);
            SetKey(keyType.moveLeft, KeyCode.LeftArrow, 1);
            SetKey(keyType.moveRight, KeyCode.D, 0);
            SetKey(keyType.moveRight, KeyCode.RightArrow, 1);
            SetKey(keyType.hardDrop, KeyCode.W, 0);
            SetKey(keyType.hardDrop, KeyCode.UpArrow, 1);
            SetKey(keyType.hardDrop, KeyCode.Space, 2);
            SetKey(keyType.softDrop, KeyCode.S, 0);
            SetKey(keyType.softDrop, KeyCode.DownArrow, 1);
            SetKey(keyType.rotateLeft, KeyCode.Q, 0);
            SetKey(keyType.rotateRight, KeyCode.E, 0);
            SetKey(keyType.hold, KeyCode.F, 0);
            SetKey(keyType.exit, KeyCode.Mouse1, 0);
            ShowKeys();
        }

        // Save all controls to player persistence
        public void SaveControls() {
            VRCPlayerApi playerApi = Networking.LocalPlayer;
            SaveControl(playerApi, keyType.left, GetKey(keyType.left, 0), GetKey(keyType.left, 1), GetKey(keyType.left, 2));
            SaveControl(playerApi, keyType.right, GetKey(keyType.right, 0), GetKey(keyType.right, 1), GetKey(keyType.right, 2));
            SaveControl(playerApi, keyType.up, GetKey(keyType.up, 0), GetKey(keyType.up, 1), GetKey(keyType.up, 2));
            SaveControl(playerApi, keyType.down, GetKey(keyType.down, 0), GetKey(keyType.down, 1), GetKey(keyType.down, 2));
            SaveControl(playerApi, keyType.confirm, GetKey(keyType.confirm, 0), GetKey(keyType.confirm, 1), GetKey(keyType.confirm, 2));
            SaveControl(playerApi, keyType.moveLeft, GetKey(keyType.moveLeft, 0), GetKey(keyType.moveLeft, 1), GetKey(keyType.moveLeft, 2));
            SaveControl(playerApi, keyType.moveRight, GetKey(keyType.moveRight, 0), GetKey(keyType.moveRight, 1), GetKey(keyType.moveRight, 2));
            SaveControl(playerApi, keyType.rotateLeft, GetKey(keyType.rotateLeft, 0), GetKey(keyType.rotateLeft, 1), GetKey(keyType.rotateLeft, 2));
            SaveControl(playerApi, keyType.rotateRight, GetKey(keyType.rotateRight, 0), GetKey(keyType.rotateRight, 1), GetKey(keyType.rotateRight, 2));
            SaveControl(playerApi, keyType.softDrop, GetKey(keyType.softDrop, 0), GetKey(keyType.softDrop, 1), GetKey(keyType.softDrop, 2));
            SaveControl(playerApi, keyType.hardDrop, GetKey(keyType.hardDrop, 0), GetKey(keyType.hardDrop, 1), GetKey(keyType.hardDrop, 2));
            SaveControl(playerApi, keyType.hold, GetKey(keyType.hold, 0), GetKey(keyType.hold, 1), GetKey(keyType.hold, 2));
            SaveControl(playerApi, keyType.exit, GetKey(keyType.exit, 0), GetKey(keyType.exit, 1), GetKey(keyType.exit, 2));
        }
    }

    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(T04oHotkeys))]
    public class T04oHotKeysEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            T04oHotkeys myTarget = (T04oHotkeys)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Get all keys"))
            {
                myTarget.indexKeyCodes = (int[])Enum.GetValues(typeof(KeyCode));
                myTarget.MarkDirty();
            }
            if (GUILayout.Button("Set default keys"))
            {
                myTarget.SetDefault();
                myTarget.MarkDirty();
            }
        }
    }
    #endif
}
