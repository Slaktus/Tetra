using System.Collections.Generic;
using Tetra.EntityManagement;
using UnityEngine;

namespace Tetra.Rendering
{
    public class EntityRenderer<T> : EntityRenderer where T : Entity
    {
        public override void Draw()
        {
            for (int i = 0; entities.Count > i; i++)
                Graphics.DrawMesh(mesh, entities[i].matrix, material, layer, camera, subMeshIndex, entities[i].materialPropertyBlock, castShadows, receiveShadows);
        }

        public void SetEntities(List<T> entities) => this.entities = entities;

        private List<T> entities { get; set; }

        public EntityRenderer(List<T> entities, Mesh mesh, Material material, Camera camera, int layer = 8, int subMeshIndex = 0, bool castShadows = false, bool receiveShadows = false ) 
            : base(mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows) 
            => this.entities = entities;
    }

    public abstract class EntityRenderer : AbstractRenderer
    {
        protected EntityRenderer(Mesh mesh, Material material, Camera camera, int layer = 8, int subMeshIndex = 0, bool castShadows = false, bool receiveShadows = false) 
            : base(mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows) { }
    }
}