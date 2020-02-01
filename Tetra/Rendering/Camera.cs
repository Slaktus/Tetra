using Unity.Mathematics;
using UnityEngine;

namespace Tetra.Rendering
{
    public abstract class Camera
    {
        public abstract void Update(float delta);
        public float3 WorldToViewportPoint(float3 point) => camera.WorldToViewportPoint(point);
        public float3 ScreenToWorldPoint(float2 point) => camera.ScreenToWorldPoint(new float3(point, -transform.position.z));
        public float3 ViewportToWorldPoint(float2 point) => camera.ViewportToWorldPoint(new float3(point, -transform.position.z));

        public float orthographicSize { get => camera.orthographicSize; protected set => camera.orthographicSize = value; }
        public float3 position { get => camera.transform.position; protected set => camera.transform.position = value; }
        public UnityEngine.Camera camera { get; private set; }
        public Transform transform { get; private set; }
        public float aspect => camera.aspect;

        protected Camera(UnityEngine.Camera camera)
        {
            this.camera = camera;
            transform = camera.transform;
        }
    }
}