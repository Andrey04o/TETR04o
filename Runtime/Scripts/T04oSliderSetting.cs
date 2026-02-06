using UnityEngine;
using UnityEngine.UI;
using UdonSharp;
using TMPro;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oSliderSetting : UdonSharpBehaviour
    {
        public T04oHandling handling;
        public HandlingType type;
        public Slider slider;
        public TextMeshProUGUI textMeshValue;

        public void SetValueWithoutNotify(float value) {
            slider.SetValueWithoutNotify(value);
            textMeshValue.text = value + "";
        }
        public void SetValue() {
            handling.SetValue(type, slider.value);
            textMeshValue.text = slider.value + "";
        }
    }
}
