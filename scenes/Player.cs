using Godot;

namespace Inversion
{
    public class Player : KinematicBody2D
    {
        [Signal]
        public delegate void PlayerDied();

        private const float JumpForce = 300f;
        private const float GravityAccel = 98f * 8;
        private const float TerminalVel = 980f;
        private const float MaxSpeed = 900f;
        private const float Acceleration = 600f;
        private const float MovementDampening = 6f;

        public InversionOrb InversionOrb { get; set; } = null;

        private float inputDir = 0f;
        private Vector2 velocity = Vector2.Zero;
        private Vector2 externalVelocity = Vector2.Zero;
        private float gravity = 0;
        private bool canJump = false;
        private ReactionHandler reactionHandler;
        private AnimatedSprite charSprite;
        private float targetCharAngle = 0f;

        private Label debugLabel;

        public override void _Ready()
        {
            reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementAreaEntered));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementAreaExited));
            charSprite = GetNode<AnimatedSprite>("AnimatedSprite");

            debugLabel = GetNode<Label>("debuglabel");
        }

        public override void _Input(InputEvent evt)
        {
            if (InversionOrb == null)
                return;

            if (evt.IsActionPressed("g_spawn_orb"))
            {
                // show & activate orb
                InversionOrb.Visible = true;
                InversionOrb.SetProcessInternal(true);
                InversionOrb.Position = Position + new Vector2(0, -16f);
            }
            else if (evt.IsActionReleased("g_spawn_orb"))
            {
                // hide & deactivate orb
                InversionOrb.Visible = false;
                InversionOrb.SetProcessInternal(false);
            }
        }

        public override void _Process(float delta)
        {
            if ((velocity + externalVelocity).LengthSquared() > 50f)
            {
                if (inputDir == 0f)
                    charSprite.Animation = "slide";
                else
                    charSprite.Animation = "run";
            }
            else
            {
                charSprite.Animation = "idle";
            }

            if (inputDir > 0f)
                charSprite.FlipH = false;
            else if (inputDir < 0f)
                charSprite.FlipH = true;

            charSprite.Rotation = Mathf.Lerp(charSprite.Rotation, targetCharAngle, delta * 16f);
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

                // Sprite rotation stuff
                var floorNormal = GetFloorNormal();
                targetCharAngle = Mathf.Atan2(floorNormal.y, floorNormal.x) + (Mathf.Pi / 2f);
                debugLabel.Text = $"normal={GetFloorNormal()}, angle={GetFloorAngle()}";
            }
            else if (Input.IsActionPressed("g_move_left") || Input.IsActionPressed("g_move_right"))
            {
                float newDir = 0;

                if (Input.IsActionPressed("g_move_left"))
                    newDir--;

                if (Input.IsActionPressed("g_move_right"))
                    newDir++;


                inputDir = Mathf.Lerp(inputDir, newDir, delta * 4f);

                charSprite.Rotation = 0f;
                debugLabel.Text = "";
            }

            if (Input.IsActionJustPressed("g_jump") && canJump)
            {
                // Jump
                gravity = -JumpForce;
                canJump = false;

                charSprite.Rotation = 0f;
                targetCharAngle = 0f;
            }

            velocity -= (velocity * MovementDampening * delta);

            velocity += (new Vector2(inputDir, 0)) * Acceleration * delta;

            externalVelocity -= (externalVelocity * 2f * delta);

            if (velocity.Length() > MaxSpeed)
                velocity = velocity.Normalized() * MaxSpeed;

            // If you hit the ceiling when jumping, start falling
            if (gravity < 0 && IsOnCeiling())
                gravity = 0;

            if (!IsOnFloor())
            {
                if (gravity < TerminalVel)
                    gravity += GravityAccel * delta;
                else if (gravity > TerminalVel)
                    gravity = TerminalVel;
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

        public void KillPlayer()
        {
            EmitSignal(nameof(PlayerDied));
        }

        private void ElementAreaEntered(Element element)
        {
            GD.Print($"Element Area Entered -> {element.ToString()}");

            if (element is Element.Fire)
            {
                KillPlayer();
            }
        }

        private void ElementAreaExited(Element element)
        {
            GD.Print($"Element Area Exited -> {element.ToString()}");
        }
    }
}