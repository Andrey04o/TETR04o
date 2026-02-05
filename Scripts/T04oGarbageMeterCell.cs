using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class T04oGarbageMeterCell : UdonSharpBehaviour
    {
        //[SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] MeshRenderer[] meshRenderers;
        [SerializeField] Material materialRenderer;
        public void SetMaterial(Material material) {
            //meshRenderer.material = material;
            materialRenderer = material;
            SetMaterialMeshRenderers(materialRenderer);
        }
        void SetMaterialMeshRenderers(Material material) {
            //materialRenderer = new Material(materialRenderer);
            foreach (MeshRenderer mr in meshRenderers) {
                mr.material = material;
            }
        }
    }
}
