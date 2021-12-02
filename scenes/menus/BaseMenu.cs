using Godot;

namespace Inversion
{
    public class BaseMenu : Node2D
    {
        private ColorRect bg;
        private Transition transition;

        public override void _Ready()
        {
            base._Ready();

            transition = GetNode<Transition>("Transition");
            bg = GetNode<ColorRect>("BG");
        }

        protected async void TransitionToScene(PackedScene scene)
        {
            var timer = GetTree().CreateTimer(1.25f);

            transition.StartTransition();

            await ToSignal(timer, "timeout");

            GetTree().ChangeSceneTo(scene);
            QueueFree();
        }
    }
}