using System.Collections.Generic;
using Tetra.CommandSystem;
using Unity.Mathematics;
using Tetra.Rendering;
using Tetra.Timing;
using Tetra.Input;

namespace Tetra.UIManagement
{
    public abstract class UIScreen
    {
        public abstract void Show();
        public abstract void Hide();
        public abstract void Update();

        public void SetUI(UI ui) => this.ui = ui;
        public void EnqueueCommand<T>(ICommandable actor, T command) where T : struct, IRoutineHandleable => ui.EnqueueCommand(actor, command);

        public List<T> Active<T>() where T : Element => ui.Active<T>();

        public void Remove<T>(T element) where T : Element
        {
            element.Deactivate();
            element.SetUIScreen(null);
            ui.Remove(element);
        }

        public T Add<T>(float3 position = default) where T : Element
        {
            T element = ui.Add<T>(position);
            element.SetUIScreen(this);
            element.Activate();
            return element;
        }

        public int id { get; }
        public Beat beat => ui.beat;
        public float time => ui.time;
        public float delta => ui.delta;
        public bool active => ui != null;
        public Camera camera => ui.camera;
        public UI ui { get; private set; }
        public RewiredInput input => ui.input;
        public float2 mousePosition => camera.ScreenToWorldPoint(input.mousePosition).xy;

        public UIScreen(int id) => this.id = id;
    }
}