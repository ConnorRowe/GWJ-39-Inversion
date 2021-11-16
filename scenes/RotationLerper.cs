using Godot;

namespace Inversion
{
    public class RotationLerper : Node
    {
        [Export]
        public float Speed { get; set; } = 1f;
        [Export]
        public bool IsActive { get; set; } = true;
        [Export]
        public float AngleSpread { get; set; } = 45f;
        [Export]
        public float BaseAngle { get; set; } = 0f;

        private float count = 0;

        public override void _Process(float delta)
        {
            if (IsActive && GetParent() is Node2D node2D)
            {
                count += delta * Speed;
                if (count > 1f)
                    count--;

                node2D.RotationDegrees = Mathf.Lerp(BaseAngle - (AngleSpread * .5f), BaseAngle + (AngleSpread * .5f), ((Mathf.Sin(count * Mathf.Tau) + 1f) * .5f));
            }
        }
    }
}