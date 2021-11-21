using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class InversionOrb : KinematicBody2D
    {
        private static readonly AudioStreamSample invertFailSample = GD.Load<AudioStreamSample>("res://sound/inversion_fail.wav");
        private static readonly AudioStreamSample invertSuccSample = GD.Load<AudioStreamSample>("res://sound/inversion_success.wav");
        private const float MaxSpeed = 180f;
        private const float Acceleration = 180f;
        private static readonly Color baseColour = new Color("e64539");
        private static readonly Color overlapColour = new Color("ff8933");

        public bool Active { get; set; } = false;
        public Sprite BaseSprite { get; set; }
        public CollisionShape2D KinematicCollisionShape { get; set; }
        private float speed = 0f;
        private List<IInvertable> overlappedInvertables = new List<IInvertable>();
        private Sprite glow;
        private Color glowColour = new Color(1, 1, 1, .3f);
        private Color targetColour = baseColour;
        private float count = 0f;
        private AudioStreamPlayer tonePlayer;
        private AudioStreamPlayer overlapPlayer;
        private AudioStreamPlayer endPlayer;

        public override void _Ready()
        {
            Area2D inversionArea = GetNode<Area2D>("InversionArea");
            inversionArea.Connect("area_entered", this, nameof(InversionAreaEntered));
            inversionArea.Connect("area_exited", this, nameof(InversionAreaExited));
            BaseSprite = GetNode<Sprite>("Sprite");
            glow = GetNode<Sprite>("Sprite/Glow");
            tonePlayer = GetNode<AudioStreamPlayer>("TonePlayer");
            overlapPlayer = GetNode<AudioStreamPlayer>("OverlapPlayer");
            endPlayer = GetNode<AudioStreamPlayer>("EndPlayer");
            KinematicCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        public override void _Process(float delta)
        {
            count += delta * .3f;
            if (count > 1f)
                count--;

            if (overlappedInvertables.Count > 0)
                targetColour = overlapColour;
            else
                targetColour = baseColour;

            BaseSprite.Modulate = BaseSprite.Modulate.LinearInterpolate(targetColour, delta * 4f);
            glowColour.a = .25f + (Mathf.Sin(count * Mathf.Pi) * .2f);
            glow.Modulate = glowColour;

            if (!Visible && tonePlayer.Playing)
                tonePlayer.Stop();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!Active)
                return;

            var mousePos = GetLocalMousePosition();

            float moveDelta = 0f;
            Vector2 lastPos = Position;

            if (Mathf.Abs(mousePos.x) > 1f || Mathf.Abs(mousePos.y) > 1f)
            {
                if (speed < MaxSpeed)
                    speed += Acceleration * delta;
                else if (speed > MaxSpeed)
                    speed = MaxSpeed;

                float mouseDist = mousePos.Length();
                if (mouseDist < 24f)
                {
                    speed *= .85f;
                }

                var collision = MoveAndCollide((mousePos.Normalized()) * speed * delta, infiniteInertia: false);
                moveDelta = (lastPos - Position).Length();
            }
            else
            {
                speed -= (speed * 12 * delta);
            }

            var audioScale = moveDelta / 3f;
            tonePlayer.PitchScale = .8f + (audioScale * .6f);
            tonePlayer.VolumeDb = Mathf.Min(-20f + (audioScale * 10f), 0f);
        }

        private void InversionAreaEntered(Area2D area)
        {
            if (area.Owner is IInvertable invertable)
            {
                overlappedInvertables.Add(invertable);

                GD.Print($"invert overlap -> {invertable}");

                overlapPlayer.Play(0);
            }
        }

        private void InversionAreaExited(Area2D area)
        {
            if (area.Owner is IInvertable invertable)
            {
                overlappedInvertables.Remove(invertable);

                GD.Print($"invert exit -> {invertable}");
            }
        }

        public void TryInvert()
        {
            bool success = false;

            foreach (var invertable in overlappedInvertables)
            {
                if (!invertable.IsInversionDisabled())
                {
                    invertable.Invert();
                    GD.Print($"inverting -> {invertable}");
                    success = true;
                }
            }

            ((AudioStreamRandomPitch)endPlayer.Stream).AudioStream = success ? invertSuccSample : invertFailSample;
            endPlayer.Play(0f);

            overlappedInvertables.Clear();
        }

        public void Start()
        {
            tonePlayer.Play();
        }

        public void End()
        {
            GD.Print("End inversion orb.");
            speed = 0f;
            tonePlayer.Stop();
        }
    }
}