using float3 = Unity.Mathematics.float3;
using System.Collections.Generic;
using Tetra.CommandSystem;
using Tetra.Rendering;
using Tetra.Caching;
using Tetra.Timing;
using Tetra.Input;
using System;

namespace Tetra.UIManagement
{
    public class UI
    {
        public T Get<T>() where T : UIScreen => screens[typeof(T)] as T;
        public void Add<T>(T screen) where T : UIScreen => screens.Add(typeof(T), screen);

        public void Clear() => context.Clear();
        public T Get<T>(int id) where T : Element => context.Get<T>(id);
        public List<T> Active<T>() where T : Element => context.Active<T>();
        public void Remove<T>(T element) where T : Element => context.Remove(element);
        public T Add<T>(float3 position = default) where T : Element => context.Add<T>(position);
        public void AddRoster<T>(Pool<T> pool) where T : Element, new() => context.Add(new Roster<T>(pool));

        public T GetCamera<T>() where T : Camera => drawer.GetCamera<T>();
        public void RemoveCamera<T>() where T : Camera => drawer.RemoveCamera<T>();
        public void AddCamera<T>(T camera) where T : Camera => drawer.AddCamera(this.camera = camera);
        public void AddDrawer<T>(UnityEngine.Mesh mesh, UnityEngine.Material material, Camera camera, int layer, int subMeshIndex, bool castShadows, bool receiveShadows) where T : Element
            => drawer.AddElementRenderer(Active<T>(), mesh, material, camera, layer, subMeshIndex, castShadows, receiveShadows);

        public void EnqueueCommand<T>(ICommandable actor, T command) where T : struct, IRoutineHandleable => commander.Enqueue(new Commander.QueueableCommand<T>(actor, command));

        public void Update(float delta)
        {
            timer.Update(delta);

            foreach (UIScreen screen in screens.Values)
                if(screen.active)
                    screen.Update();

            context.Cull();
            context.Update();
            commander.Update();
            drawer.Update(delta);
        }

        public bool Showing<T>() where T : UIScreen
        {
            Type type = typeof(T);

            foreach (UIScreen screen in screens.Values)
                if (screen.active && type == screen.GetType())
                    return true;

            return false;
        }

        public T Show<T>() where T : UIScreen
        {
            T screen = screens[typeof(T)] as T;
            screen.SetUI(this);
            screen.Show();
            return screen;
        }

        public void Hide<T>() where T : UIScreen
        {
            T screen = screens[typeof(T)] as T;
            screen.Hide();
            screen.SetUI(null);
        }

        public void HideAll()
        {
            foreach (UIScreen screen in screens.Values)
                if(screen.active)
                {
                    screen.Hide();
                    screen.SetUI(null);
                }
        }
        
        public Game game { get; }
        public Camera camera { get; private set; }
        public RewiredInput input { get; } = new RewiredInput();
        public float delta => timer.delta;
        public float time => timer.time;
        public Beat beat => timer.beat;

        private Timer timer { get; } = new Timer();
        private Drawer drawer { get; } = new Drawer();
        private Context context { get; } = new Context();
        private Commander commander { get; } = new Commander();
        private Dictionary<Type, UIScreen> screens { get; } = new Dictionary<Type, UIScreen>();

        public UI(Game game)
        {
            this.game = game;
            input.SetPlayer(Input.Input.GetUnassignedPlayer());
        }
    }
}