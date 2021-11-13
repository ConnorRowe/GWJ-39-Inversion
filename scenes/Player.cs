using Godot;

namespace Inversion
{
    public class Player : KinematicBody2D
    {
        private const float jumpForce = 300f;
        private const float gravityAccel = 98f * 8;
        private const float terminalVel = 980f;

        private float inputDir = 0f;
        private float acceleration = 450f;
        private float maxSpeed = 650f;
        private float movementDampening = 7f;
        private Vector2 velocity = Vector2.Zero;
        private Vector2 externalVelocity = Vector2.Zero;
        private float gravity = 0;
        private bool canJump = false;

        public override void _Ready()
        {
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!canJump && IsOnFloor())
                canJump = true;

            if (canJump && !IsOnFloor())
                StartedFalling();

            if (IsOnFloor())
            {
                inputDir = 0;

                if (Input.IsActionPressed("g_move_left"))
                    inputDir--;

                if (Input.IsActionPressed("g_move_right"))
                    inputDir++;
            }
            else if (Input.IsActionPressed("g_move_left") || Input.IsActionPressed("g_move_right"))
            {
                float newDir = 0;

                if (Input.IsActionPressed("g_move_left"))
                    newDir--;

                if (Input.IsActionPressed("g_move_right"))
                    newDir++;


                inputDir = Mathf.Lerp(inputDir, newDir, delta * 4f);
            }

            if (Input.IsActionJustPressed("g_jump") && canJump)
            {
                // Jump
                gravity = -jumpForce;
                canJump = false;
            }

            velocity -= (velocity * movementDampening * delta);

            velocity += (new Vector2(inputDir, 0)) * acceleration * delta;

            externalVelocity -= (externalVelocity * 2f * delta);

            if (velocity.Length() > maxSpeed)
                velocity = velocity.Normalized() * maxSpeed;

            // If you hit the ceiling when jumping, start falling
            if (gravity < 0 && IsOnCeiling())
                gravity = 0;

            if (!IsOnFloor())
            {
                if (gravity < terminalVel)
                    gravity += gravityAccel * delta;
                else if (gravity > terminalVel)
                    gravity = terminalVel;
            }
            else if (gravity > 0)
                gravity = 0;

            // Apply velocities
            MoveAndSlideWithSnap(velocity + externalVelocity + new Vector2(0, gravity), Vector2.Down, Vector2.Up);
        }

        public void ApplyExternalImpulse(Vector2 impulse)
        {
            externalVelocity += impulse;
        }

        private void StartedFalling()
        {
            GetTree().CreateTimer(0.15f).Connect("timeout", this, nameof(DisableJumpFromFall));
        }

        private void DisableJumpFromFall()
        {
            if (!IsOnFloor())
            {
                canJump = false;
            }
        }
    }
}