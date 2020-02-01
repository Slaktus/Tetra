using System.Collections.Generic;
using Tetra.CommandSystem;
using Tetra.UIManagement;
using Unity.Mathematics;
using Tetra.Rendering;
using Tetra.Collision;
using Tetra.Caching;
using Tetra.Timing;

namespace Tetra.EntityManagement
{
    public class Session
    {
        public void SetActive(bool active) => this.active = active;

        public void SetCommandQueue(Queue<IQueueable> queue) => commander.SetQueue(queue);
        public void EnqueueCommand<T>(ICommandable actor, T command) where T : struct, IRoutineHandleable => commander.Enqueue(new Commander.QueueableCommand<T>(actor, command));
        public void RecordCommand<T>(ICommandable actor, T command) where T : struct, IRoutineHandleable => commander.Enqueue(recorder.Record(new Commander.QueueableCommand<T>(actor, command)));

        public List<T> Active<T>() where T : Entity => context.Active<T>();
        public void AddRoster<T>(Pool<T> pool) where T : Entity, new() => context.Add(new Roster<T>(pool, this));

        public T Get<T>(int id) where T : Entity => context.Get<T>(id);
        public void RemoveAll<T>() where T : Entity => context.Clear<T>();
        public void Remove<T>(T entity) where T : Entity => context.Remove(entity);
        public T Add<T>(float3 position = default) where T : Entity => context.Add<T>(position);

        public void SetTimeScale(float timeScale) => context.SetTimeScale(timeScale);
        public void SetUnityTimeScale(float timeScale) => UnityEngine.Time.timeScale = timeScale;
        public void SetTimeScale<T>(float timeScale) where T : Entity => context.SetTimeScale<T>(timeScale);

        public T GetCamera<T>() where T : Camera => drawer.GetCamera<T>();
        public void RemoveCamera<T>() where T : Camera => drawer.RemoveCamera<T>();
        public void AddCamera<T>(T camera) where T : Camera => drawer.AddCamera(camera);
        public void AddDrawer<T>(UnityEngine.Mesh mesh, UnityEngine.Material material, Camera camera, int layer, int subMeshIndex, bool castShadows, bool receiveShadows) where T : Entity => drawer.AddEntityRenderer(Active<T>(), mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows);

        public void AddCollisionGroup<T>() => collisionSystem.AddCollisionGroup<T>();
        public void RemoveCollisionGroup<T>() => collisionSystem.RemoveCollisionGroup<T>();
        public float2 Depenetrate(AbstractCollider collider) => collisionSystem.Depenetrate(collider);
        public void AddCollider<T>(AbstractCollider collider) => collisionSystem.AddCollider<T>(collider);
        public void RemoveCollider<T>(AbstractCollider collider) => collisionSystem.RemoveCollider<T>(collider);
        public bool Intersects(AbstractCollider colliderA, AbstractCollider colliderB) => collisionSystem.Intersects(colliderA, colliderB);
        public bool ContainedBy(AbstractCollider colliderA, AbstractCollider colliderB) => collisionSystem.ContainedBy(colliderA, colliderB);
        public float2 Depenetrate(AbstractCollider colliderA, AbstractCollider colliderB) => collisionSystem.Depenetrate(colliderA, colliderB);
        public void AddGroupToCheck<T, Y>(bool contains, bool intersect, bool depenetrate) => collisionSystem.AddGroupToCheck<T, Y>(contains, intersect, depenetrate);


        public void Update(float delta)
        {
            timer.Update(delta);
            collisionSystem.Update();

            if (!playback)
                recorder.AddQueue(time, delta);

            context.Cull();
            context.Update();
            commander.Update();
            drawer.Update(delta);
        }

        public void Clear(bool clearRosters = false)
        {
            context.Clear(clearRosters);
            collisionSystem.Clear();
        }

        public Recorder recorder { get; }
        public bool active { get; private set; }
        public bool playback => recorder.playback;
        public Record record => recorder.record;
        public float delta => timer.delta;
        public float time => timer.time;
        public Beat beat => timer.beat;
        public int id { get; }

        private UI ui { get; }
        private Timer timer { get; } = new Timer();
        private Drawer drawer { get; } = new Drawer();
        private Context context { get; } = new Context();
        private Commander commander { get; } = new Commander();
        private CollisionSystem collisionSystem { get; } = new CollisionSystem();

        public Session(int id, Recorder recorder, UI ui)
        {
            this.id = id;
            this.ui = ui;
            this.recorder = recorder;
            recorder.SetSession(this);
        }
    }
}