using Godot;

namespace Inversion
{
    public class CrateMetal : RigidBody2D, IHasElementalArea, IMetallic
    {
        private bool isCharged = false;
        private Particles2D sparks;
        private Vector2 startPosition;
        private float maxY = 999999;
        private ReactionHandler reactionHandler;
        private BaseLevel level;

        public override void _Ready()
        {
            sparks = GetNode<Particles2D>("Sparks");
            reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            startPosition = GlobalPosition;
            maxY = ((BaseLevel)GetTree().CurrentScene).levelBounds.Position.y;
            level = (BaseLevel)GetTree().CurrentScene;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (GlobalPosition.y > maxY) // Offscreen
            {
                LinearVelocity = Vector2.Zero;
                AngularVelocity = 0f;
                GlobalPosition = startPosition;
            }
        }

        public void Power()
        {
            if (isCharged)
                return;

            isCharged = true;
            sparks.Emitting = true;

            GlobalNodes.Singleton.MakeBzztSound();
        }

        public void UnPower()
        {
            isCharged = false;
            sparks.Emitting = false;
        }

        public System.Collections.Generic.List<IMetallic> GetNearbyMetallics()
        {
            return reactionHandler.NearbyMetallics;
        }

        public Element GetAreaElement()
        {
            return Element.Lightning;
        }

        public bool IsDisabled()
        {
            return !isCharged;
        }
    }
}