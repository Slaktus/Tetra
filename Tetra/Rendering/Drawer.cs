using System.Collections.Generic;
using Tetra.EntityManagement;
using Tetra.UIManagement;
using System;

namespace Tetra.Rendering
{
    public class Drawer
    {
        public T GetCamera<T>() where T : Camera => cameras[typeof(T)] as T;
        public void AddCamera<T>(T camera) where T : Camera => cameras.Add(typeof(T), camera);

        public void AddEntityRenderer<T>(List<T> entities, UnityEngine.Mesh mesh, UnityEngine.Material material, Camera camera, int layer, int subMeshIndex, bool castShadows, bool receiveShadows) where T : Entity
            => renderers.Add(typeof(T), new EntityRenderer<T>(entities, mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows));

        public void AddElementRenderer<T>(List<T> elements, UnityEngine.Mesh mesh, UnityEngine.Material material, Camera camera, int layer, int subMeshIndex, bool castShadows, bool receiveShadows) where T : Element
            => renderers.Add(typeof(T), new ElementRenderer<T>(elements, mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows));

        public void RemoveCamera<T>()
        {
            Type type = typeof(T);

            if (cameras.ContainsKey(type))
                cameras.Remove(type);
            else
                renderers.Remove(type);
        }

        public void Update(float delta)
        {
            foreach (Camera camera in cameras.Values)
                camera.Update(delta);

            foreach (AbstractRenderer renderer in renderers.Values)
                renderer.Draw();
        }

        private Dictionary<Type, AbstractRenderer> renderers { get; } = new Dictionary<Type, AbstractRenderer>();
        private Dictionary<Type, Camera> cameras { get; } = new Dictionary<Type, Camera>();
    }
}