using Godot;

namespace Inversion
{
    public class Rotator : Node
    {
        [Export]
        public float RotationSpeed { get; set; } = 1f;

        [Export]
        public bool IsActive { get; set; } = true;

        public override void _Process(float delta)
        {
            if (IsActive && GetParent() is Node2D node2D)
                node2D.Rotation += Mathf.Tau * delta * RotationSpeed;
        }
    }
}