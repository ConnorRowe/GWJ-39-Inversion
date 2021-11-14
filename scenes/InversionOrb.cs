using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class InversionOrb : KinematicBody2D
    {
        private const float maxSpeed = 180f;
        private const float acceleration = 180f;

        public bool Active { get; set; } = true;
        private float speed = 0f;
        private List<IInvertable> overlappedInvertables = new List<IInvertable>();

        public override void _Ready()
        {
            base._Ready();

            Area2D inversionArea = GetNode<Area2D>("InversionArea");
            inversionArea.Connect("area_entered", this, nameof(InversionAreaEntered));
            inversionArea.Connect("area_exited", this, nameof(InversionAreaExited));
        }

        public override void _Input(InputEvent evt)
        {
            if (evt is InputEventMouseButton emb && !emb.Pressed && emb.ButtonIndex == 1)
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
            if (area is IInvertable invertable)
            {
                overlappedInvertables.Remove(invertable);

                GD.Print($"invert exit -> {invertable}");
            }
        }
    }
}