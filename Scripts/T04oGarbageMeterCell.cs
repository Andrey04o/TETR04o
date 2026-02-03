using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oGarbageMeterCell : UdonSharpBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        public void SetMaterial(Material material) {
            meshRenderer.material = material;
        }
    }
}
