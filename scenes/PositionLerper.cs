using Godot;

namespace Inversion
{
    public class PositionLerper : Node
    {
        [Export]
        public float Speed { get; set; } = 1f;
        [Export]
        public bool IsActive { get; set; } = true;
        [Export]
        public float PercentOffset { get; set; } = 0f;

        private Vector2 globalPosA;
        private Vector2 globalPosB;
        private float count = 0;

        public override void _Ready()
        {
            globalPosA = GetNode<Position2D>("GlobalPointA").GlobalPosition;
            globalPosB = GetNode<Position2D>("GlobalPointB").GlobalPosition;
        }

        public override void _Process(float delta)
        {
            if (IsActive && GetParent() is Node2D node2D)
            {
                count += delta * Speed;
                if (count > 1f)
                    count--;

                float weight = (Mathf.Sin((count + PercentOffset) * Mathf.Tau) + 1f) * .5f;
                node2D.GlobalPosition = new Vector2(Mathf.Lerp(globalPosA.x, globalPosB.x, weight), Mathf.Lerp(globalPosA.y, globalPosB.y, weight));
            }
        }
    }
}