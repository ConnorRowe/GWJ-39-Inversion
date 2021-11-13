using Godot;

namespace Inversion
{
    public class InversionOrb : KinematicBody2D
    {
        private const float maxSpeed = 180f;
        private const float acceleration = 180f;

        public bool Active { get; set; } = true;
        private float speed = 0f;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!Active)
                return;

            var mousePos = GetLocalMousePosition();

            if (Mathf.Abs(mousePos.x) > 1f || Mathf.Abs(mousePos.y) > 1f)
            {
                if (speed < maxSpeed)
                    speed += acceleration * delta;
                else if (speed > maxSpeed)
                    speed = maxSpeed;

                var collision = MoveAndCollide((mousePos.Normalized()) * speed * delta, false);
            }
            else
            {
                speed -= (speed * 12 * delta);
            }
        }
    }
}