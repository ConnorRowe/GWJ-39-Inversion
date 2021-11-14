using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class InversionOrb : KinematicBody2D
    {
        private const float MaxSpeed = 180f;
        private const float Acceleration = 180f;
        private static readonly Color baseColour = new Color("e64539");
        private static readonly Color overlapColour = new Color("ff8933");

        public bool Active { get; set; } = true;
        private float speed = 0f;
        private List<IInvertable> overlappedInvertables = new List<IInvertable>();
        private Sprite baseSprite;
        private Sprite glow;
        private Color glowColour = new Color(1, 1, 1, .3f);
        private Color targetColour = baseColour;
        private float count = 0f;

        public override void _Ready()
        {
            Area2D inversionArea = GetNode<Area2D>("InversionArea");
            inversionArea.Connect("area_entered", this, nameof(InversionAreaEntered));
            inversionArea.Connect("area_exited", this, nameof(InversionAreaExited));
            baseSprite = GetNode<Sprite>("Sprite");
            glow = GetNode<Sprite>("Sprite/Glow");
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

            baseSprite.Modulate = baseSprite.Modulate.LinearInterpolate(targetColour, delta * 4f);
            glowColour.a = .25f + (Mathf.Sin(count * Mathf.Pi) * .2f);
            glow.Modulate = glowColour;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!Active)
                return;

            var mousePos = GetLocalMousePosition();

            if (Mathf.Abs(mousePos.x) > 1f || Mathf.Abs(mousePos.y) > 1f)
            {
                if (speed < MaxSpeed)
                    speed += Acceleration * delta;
                else if (speed > MaxSpeed)
                    speed = MaxSpeed;

                float mouseDist = mousePos.Length();
                if (mouseDist < 8f)
                {
                    speed *= (mouseDist / 8f);
                }

                var collision = MoveAndCollide((mousePos.Normalized()) * speed * delta, false);
            }
            else
            {
                speed -= (speed * 12 * delta);
            }
        }

        private void InversionAreaEntered(Area2D area)
        {
            if (area.Owner is IInvertable invertable)
            {
                overlappedInvertables.Add(invertable);

                GD.Print($"invert overlap -> {invertable}");
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
            foreach (var invertable in overlappedInvertables)
            {
                if (!invertable.IsDisabled())
                {
                    invertable.Invert();
                    GD.Print($"inverting -> {invertable}");
                }
            }

            overlappedInvertables.Clear();
        }
    }
}