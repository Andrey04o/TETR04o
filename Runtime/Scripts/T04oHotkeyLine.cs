using UnityEngine;
using UdonSharp;
using TMPro;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oHotkeyLine : UdonSharpBehaviour
    {
        public keyType key;
        public TextMeshProUGUI textMeshControl;
        public T04oChangeHotkeys[] changers;
        public T04oHotkeys hotkeys;

    }
}