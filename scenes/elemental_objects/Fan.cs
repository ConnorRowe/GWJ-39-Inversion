using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class Fan : LigntningPowered
    {
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

            var pushVector = Vector2.Right.Rotated(Rotation);

            foreach (var body in overlappedBodies)
            {
                if (body is RigidBody2D rigidBody)
                {
                    rigidBody.ApplyCentralImpulse(pushVector * PushPower);
                }
                else if (body is Player player)
                {
                    player.ApplyExternalImpulse(pushVector * PushPower);
                }
            }
        }

        private void BodyEntered(Node body)
        {
            if (body is PhysicsBody2D physicsBody)
            {
                overlappedBodies.Add(physicsBody);
            }
        }

        private void BodyExited(Node body)
        {
            if (body is PhysicsBody2D physicsBody)
            {
                var b = overlappedBodies.Remove(physicsBody);
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