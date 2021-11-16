using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class Fan : LigntningPowered
    {
        [Export]
        public Vector2 PushVector { get; set; } = Vector2.Right;
        [Export]
        public float PushPower { get; set; } = 10f;

        private HashSet<PhysicsBody2D> overlappedBodies = new HashSet<PhysicsBody2D>();

        private Sprite[] windLines;

        public override void _Ready()
        {
            var pushArea = GetNode("PushArea");
            windLines = new Sprite[3] { GetNode<Sprite>("Wind1"), GetNode<Sprite>("Wind2"), GetNode<Sprite>("Wind3") };

            pushArea.Connect("body_entered", this, nameof(BodyEntered));
            pushArea.Connect("body_exited", this, nameof(BodyExited));

            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!isLightningPowered && !alwaysLightningPowered)
                return;

            foreach (var body in overlappedBodies)
            {
                if (body is RigidBody2D rigidBody)
                {
                    rigidBody.ApplyCentralImpulse(PushVector * PushPower);
                }
                else if (body is Player player)
                {
                    player.ApplyExternalImpulse(PushVector * PushPower);
                }
            }
        }

        private void BodyEntered(Node body)
        {
            if (body is PhysicsBody2D physicsBody)
            {
                overlappedBodies.Add(physicsBody);

                GD.Print($"Added {body} to fan.");
            }
        }

        private void BodyExited(Node body)
        {
            if (body is PhysicsBody2D physicsBody)
            {
                var b = overlappedBodies.Remove(physicsBody);
                if (!b)
                    GD.PrintErr("Tried to remove non-cached body from fan!");
                else
                    GD.Print($"Removed {body} from fan.");
            }
        }

        protected override void LightningPower()
        {
            base.LightningPower();

            foreach (var line in windLines)
            {
                line.Visible = true;
            }
        }

        protected override void LightningUnPower()
        {
            base.LightningUnPower();

            foreach (var line in windLines)
            {
                line.Visible = false;
            }
        }
    }
}