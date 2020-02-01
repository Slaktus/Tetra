using System.Collections.Generic;
using Tetra.UIManagement;
using UnityEngine;

namespace Tetra.Rendering
{
    public class ElementRenderer<T> : ElementRenderer where T : Element
    {
        public override void Draw()
        {
            for (int i = 0; elements.Count > i; i++)
                Graphics.DrawMesh(mesh, elements[i].matrix, material, layer, camera, subMeshIndex, elements[i].materialPropertyBlock, castShadows, receiveShadows);
        }

        public void SetEntities(List<T> entities) => this.elements = entities;

        private List<T> elements { get; set; }

        public ElementRenderer(List<T> elements, Mesh mesh, Material material, Camera camera, int layer = 8, int subMeshIndex = 0, bool castShadows = false, bool receiveShadows = false ) 
            : base(mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows) 
            => this.elements = elements;
    }

    public abstract class ElementRenderer : AbstractRenderer
    {
        protected ElementRenderer(Mesh mesh, Material material, Camera camera, int layer = 8, int subMeshIndex = 0, bool castShadows = false, bool receiveShadows = false) 
            : base(mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows) { }
    }
}