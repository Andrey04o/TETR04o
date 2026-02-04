using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oPlayersAlive : UdonSharpBehaviour
    {
        public TextMeshPro textMeshPlayers;
        public TextMeshPro textMeshPlayersCount;
        public void Show(bool value) {
            textMeshPlayers.gameObject.SetActive(value);
        }
        public void SetValue(byte value) {
            textMeshPlayersCount.text = value + "";
        }
    }
}
