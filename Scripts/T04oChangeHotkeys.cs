using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Reflection;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oChangeHotkeys : UdonSharpBehaviour
    {
        public T04oHotkeys hotkeys;
        public keyType key;
        public Button button;
        public TextMeshProUGUI textMeshLetter;
        public KeyCode previousKeyKode;
        public int indexButtonNumber = 0;
        
        void Update() {
            if (Input.anyKeyDown) {
                if (Input.GetKeyDown(KeyCode.Delete)) {
                    hotkeys.SetKey(key, 0, indexButtonNumber);
                    textMeshLetter.text = "";
                } else {
                    hotkeys.SetKey(key, GetKey(), indexButtonNumber);
                    textMeshLetter.text = GetKey().ToString();
                }   
                gameObject.SetActive(false);
            }
        }
        public void ReadHotkey() {
            int index = hotkeys.GetKey(key, indexButtonNumber);
            if (index == 0) {
                textMeshLetter.text = "";
                return;
            }
            textMeshLetter.text = ((KeyCode)index).ToString();
        }
        
        public void EnableCheckKeyboard() {
            gameObject.SetActive(true);
        }
        private KeyCode GetKey() {
            foreach (int index in hotkeys.indexKeyCodes) {
                if (Input.GetKeyDown((KeyCode)index)){
                    return (KeyCode)index;
                }
            }
            return previousKeyKode;
        }
    }
}
