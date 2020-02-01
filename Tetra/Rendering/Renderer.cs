using UnityEngine;

namespace Tetra.Rendering
{
    public abstract class AbstractRenderer
    {
        public abstract void Draw();

        public void SetMesh(Mesh mesh) => this.mesh = mesh;
        public void SetLayer(int layer) => this.layer = layer;
        public void SetCamera(Camera camera) => this.camera = camera.camera;
        public void SetMaterial(Material material) => this.material = material;
        public void SetCastShadows(bool castShadows) => this.castShadows = castShadows;
        public void SetSubMeshIndex(int subMeshIndex) => this.subMeshIndex = subMeshIndex;
        public void SetReceiveShadows(bool receiveShadows) => this.receiveShadows = receiveShadows;

        protected int layer { get; private set; }
        protected Mesh mesh { get; private set; }
        protected int subMeshIndex { get; private set; }
        protected bool castShadows { get; private set; }
        protected Material material { get; private set; }
        protected bool receiveShadows { get; private set; }
        protected UnityEngine.Camera camera { get; private set; }

        protected AbstractRenderer( Mesh mesh, Material material, Camera camera, int layer = 8, int subMeshIndex = 0, bool castShadows = false, bool receiveShadows = false)
        {
            this.mesh = mesh;
            this.layer = layer;
            this.material = material;
            this.camera = camera.camera;
            this.castShadows = castShadows;
            this.subMeshIndex = subMeshIndex;
            this.receiveShadows = receiveShadows;
        }
    }
}