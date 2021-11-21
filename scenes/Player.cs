using Godot;

namespace Inversion
{
    public class Player : KinematicBody2D, IDamageable
    {
        private enum OrbState
        {
            Hidden,
            Active,
        }

        [Signal]
        public delegate void PlayerDied();

        private static readonly AudioStreamSample deathSample = GD.Load<AudioStreamSample>("res://sound/player_death_sound.wav");

        private const float JumpForce = 300f;
        private const float GravityAccel = 98f * 8;
        private const float TerminalVel = 245f;
        private const float MaxSpeed = 1000f;
        private const float Acceleration = 600f;
        private const float MovementDamping = 8f;

        public InversionOrb InversionOrb { get; set; } = null;
        public bool IsFrozen { get; set; } = false;

        private float inputDir = 0f;
        private Vector2 velocity = Vector2.Zero;
        private Vector2 externalVelocity = Vector2.Zero;
        private float gravity = 0;
        private bool canJump = false;
        private ReactionHandler reactionHandler;
        private AnimatedSprite charSprite;
        private float targetCharAngle = 0f;
        private Label debugLabel;
        private Sprite lightBase;
        private Sprite lightGlow;
        private Tween tween;
        private OrbState orbState = OrbState.Hidden;
        private float count = 0f;
        private Color glowColour = new Color(1, 1, 1, .3f);
        private float lightPulseSpeed = .25f;
        private bool canInvert = true;
        private Area2D hitBox;
        private Particles2D feetDust;
        private AudioStreamPlayer footstepPlayer;
        private AudioStreamPlayer slidePlayer;
        private AudioStreamPlayer jumpPlayer;
        private AnimatedSprite jumpEffect;
        private bool inAirLastFrame = false;
        private bool dead = false;
        private float maxY = 999999;

        public override void _Ready()
        {
            reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementAreaEntered));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementAreaExited));
            charSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            lightBase = GetNode<Sprite>("AnimatedSprite/Light");
            lightGlow = GetNode<Sprite>("AnimatedSprite/Light/Glow");
            tween = GetNode<Tween>("Tween");
            hitBox = GetNode<Area2D>("HitBox");
            feetDust = GetNode<Particles2D>("FeetDust");
            footstepPlayer = GetNode<AudioStreamPlayer>("FootstepPlayer");
            slidePlayer = GetNode<AudioStreamPlayer>("SlidePlayer");
            jumpPlayer = GetNode<AudioStreamPlayer>("JumpPlayer");
            jumpEffect = GetNode<AnimatedSprite>("JumpEffect");

            charSprite.Connect("frame_changed", this, nameof(FrameChanged));

            debugLabel = GetNode<Label>("debuglabel");

            var playerMat = (ShaderMaterial)charSprite.Material;
            playerMat.SetShaderParam("override_percent", 0f);

            maxY = ((BaseLevel)GetTree().CurrentScene).levelBounds.Position.y;
        }

        public override void _Input(InputEvent evt)
        {
            if (InversionOrb == null || IsFrozen)
                return;

            if (evt.IsActionPressed("g_spawn_orb"))
            {
                // show & activate orb
                SpawnOrbPressed();
            }
            else if (evt.IsActionReleased("g_spawn_orb"))
            {
                // hide & deactivate orb
                SpawnOrbReleased();
            }
            else if (evt.IsActionPressed("g_restart_level"))
            {
                KillPlayer();
            }
        }

        public override void _Process(float delta)
        {
            if (dead)
                return;

            count += delta * lightPulseSpeed;
            if (count > 1f)
                count--;

            glowColour.a = .25f + (Mathf.Sin(count * Mathf.Pi) * .2f);
            lightGlow.Modulate = glowColour;

            bool shouldEndSlide = false;

            slidePlayer.VolumeDb = Mathf.Min(-4 + (((velocity + externalVelocity).LengthSquared() / 5200f) * 10f), 0f);

            if ((velocity + externalVelocity).LengthSquared() > 50f)
            {
                if (inputDir == 0f)
                {
                    if (charSprite.Animation != "slide")
                        StartSlide();

                    charSprite.Animation = "slide";
                }
                else
                {
                    if (charSprite.Animation == "slide" || slidePlayer.Playing)
                        shouldEndSlide = true;

                    charSprite.Animation = "run";
                    charSprite.SpeedScale = velocity.LengthSquared() * .0001f;
                }
            }
            else
            {
                if (charSprite.Animation == "slide")
                    shouldEndSlide = true;

                charSprite.Animation = "idle";
            }

            if (shouldEndSlide)
                EndSlide();

            feetDust.Emitting = charSprite.Animation != "idle" && IsOnFloor();

            if (inputDir > 0f)
                charSprite.FlipH = false;
            else if (inputDir < 0f)
                charSprite.FlipH = true;

            reactionHandler.FlipH = charSprite.FlipH;
            hitBox.Scale = new Vector2(charSprite.FlipH ? -1f : 1f, 1f);

            charSprite.Rotation = Mathf.Lerp(charSprite.Rotation, targetCharAngle, delta * 16f);
            lightGlow.Rotation = -charSprite.Rotation;

            //Adjust light position to match animation
            int flipHOffset = charSprite.FlipH ? -1 : 1;

            switch (charSprite.Animation)
            {
                case "idle":
                    lightBase.Position = new Vector2(2 * flipHOffset, -16);
                    lightPulseSpeed = .2f;
                    break;
                case "slide":
                    lightBase.Position = new Vector2(3 * flipHOffset, -15);
                    lightPulseSpeed = .5f;
                    break;
                case "run":
                    lightPulseSpeed = .8f;
                    switch (charSprite.Frame)
                    {
                        case 1:
                        case 5:
                            lightBase.Position = new Vector2(2 * flipHOffset, -15);
                            break;
                        case 2:
                        case 6:
                            lightBase.Position = new Vector2(2 * flipHOffset, -17);
                            break;
                        default:
                            lightBase.Position = new Vector2(2 * flipHOffset, -16);
                            break;
                    }
                    break;
            }

            if (GlobalPosition.y > maxY) // Offscreen
            {
                KillPlayer();
            }
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
            }

            if (Input.IsActionJustPressed("g_jump") && canJump)
            {
                // Jump
                gravity = -JumpForce;
                canJump = false;

                charSprite.Rotation = 0f;
                targetCharAngle = 0f;

                jumpPlayer.Play();
                MakeJumpEffect();
            }

            velocity -= (velocity * MovementDamping * delta);

            velocity += (new Vector2(inputDir, 0)) * Acceleration * delta;

            externalVelocity -= (externalVelocity * MovementDamping * delta);

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
            if (!IsFrozen)
                MoveAndSlideWithSnap(velocity + externalVelocity + new Vector2(0, gravity), Vector2.Down, Vector2.Up, infiniteInertia: false);

            // Push objects
            if (inputDir != 0f)
            {
                int slideCount = GetSlideCount();
                for (int i = 0; i < slideCount; ++i)
                {
                    var collision = GetSlideCollision(i);
                    if (collision.Collider is RigidBody2D rigidBody)
                    {
                        rigidBody.ApplyImpulse(rigidBody.GlobalPosition - collision.Position, new Vector2(inputDir, 0) * (8f / rigidBody.Mass));
                    }
                }
            }
            debugLabel.Text = $"";

            if (IsOnFloor() && inAirLastFrame)
                footstepPlayer.Play();

            inAirLastFrame = !IsOnFloor();
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
            if (dead)
                return;

            dead = true;

            jumpPlayer.Stream = deathSample;
            jumpPlayer.Play();
            slidePlayer.Stop();
            charSprite.Animation = "idle";

            var playerMat = charSprite.Material;

            tween.InterpolateProperty(lightBase, "modulate", lightBase.Modulate, Colors.Transparent, .5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(lightGlow, "modulate", lightGlow.Modulate, Colors.Transparent, .5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(playerMat, "shader_param/override_percent", 0f, 1f, .5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(playerMat, "shader_param/override_colour", new Color("ad2f45"), new Color("ff8933"), .3f, Tween.TransitionType.Cubic, delay: .25f);
            tween.InterpolateProperty(playerMat, "shader_param/override_colour", new Color("ff8933"), new Color("21181b"), .1f, Tween.TransitionType.Cubic, delay: .55f);
            tween.InterpolateProperty(playerMat, "shader_param/override_colour", new Color("21181b"), new Color("0021181b"), .3f, Tween.TransitionType.Cubic, delay: 1.55f);

            tween.Start();

            EmitSignal(nameof(PlayerDied));
        }

        private void ElementAreaEntered(Element element, Node source)
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

        private void ActivateOrb(bool activate = true)
        {
            InversionOrb.Active = activate;
            InversionOrb.Visible = activate;
            InversionOrb.Start();
        }

        private void SpawnOrbPressed()
        {
            if (!canInvert || orbState == OrbState.Active)
                return;

            InversionOrb.GlobalPosition = lightBase.GlobalPosition;
            canInvert = false;
            orbState = OrbState.Active;
            ActivateOrb();
            tween.Stop(InversionOrb.BaseSprite, "scale");
            tween.Stop(lightBase, "modulate");
            tween.InterpolateProperty(InversionOrb.BaseSprite, "scale", Vector2.Zero, Vector2.One, 1f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.InterpolateProperty(lightBase, "modulate", lightBase.Modulate, Colors.Transparent, .5f, Tween.TransitionType.Quad);
            tween.Start();

            InversionOrb.KinematicCollisionShape.Disabled = false;
        }

        private void SpawnOrbReleased()
        {
            InversionOrb.End();

            if (orbState != OrbState.Active)
                return;

            orbState = OrbState.Hidden;
            InversionOrb.TryInvert();

            tween.Stop(InversionOrb.BaseSprite, "scale");
            tween.Stop(lightBase, "modulate");
            tween.InterpolateProperty(InversionOrb.BaseSprite, "scale", InversionOrb.Scale, Vector2.Zero, .5f, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.InterpolateProperty(lightBase, "modulate", lightBase.Modulate, Colors.White, 1f, Tween.TransitionType.Quad, delay: .5f);
            tween.Start();

            InversionOrb.KinematicCollisionShape.Disabled = true;

            GetTree().CreateTimer(1.5f).Connect("timeout", this, nameof(ResetCanInvert));
            GetTree().CreateTimer(.5f).Connect("timeout", this, nameof(ActivateOrb), new Godot.Collections.Array(false));
        }

        private void ResetCanInvert()
        {
            canInvert = true;
        }

        public void TakeDamage()
        {
            KillPlayer();
        }

        private void FrameChanged()
        {
            if ((charSprite.Frame == 1 || charSprite.Frame == 4) && IsOnFloor())
            {
                footstepPlayer.Play();
            }
        }

        private void StartSlide()
        {
            slidePlayer.Play();
        }

        private void EndSlide()
        {
            slidePlayer.Stop();
        }

        private void MakeJumpEffect()
        {
            var newJumpEffect = (AnimatedSprite)jumpEffect.Duplicate();
            GetTree().CurrentScene.AddChild(newJumpEffect);
            newJumpEffect.GlobalPosition = jumpEffect.GlobalPosition;
            newJumpEffect.Visible = true;
            newJumpEffect.Play("default");

            GetTree().CreateTimer(0.83334f).Connect("timeout", newJumpEffect, "queue_free");
        }
    }
}