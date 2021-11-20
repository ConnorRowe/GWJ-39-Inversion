using Godot;

namespace Inversion
{
    public class CrateMetal : RigidBody2D, IHasElementalArea
    {
        private bool isCharged = false;
        private Particles2D sparks;
        private Vector2 startPosition;
        private float maxY = 999999;
        private ReactionHandler reactionHandler;

        public override void _Ready()
        {
            sparks = GetNode<Particles2D>("Sparks");
            reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));
            startPosition = GlobalPosition;
            maxY = ((BaseLevel)GetTree().CurrentScene).levelBounds.Position.y;
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

            if (reactionHandler.LightningSource == null || (reactionHandler.LightningSource != null && reactionHandler.LightningSource.IsDisabled()))
            {
                isCharged = false;
                sparks.Emitting = false;
            }
        }

        private void ElementStarted(Element element, Node source)
        {
            if (element == Element.Lightning && source != null)
            {
                reactionHandler.LightningSource = (IHasElementalArea)source;

                isCharged = true;
                sparks.Emitting = true;

                GlobalNodes.Singleton.MakeBzztSound();
            }
        }

        private void ElementEnded(Element element)
        {
            if (element == Element.Lightning)
            {
                isCharged = false;
                sparks.Emitting = false;
            }
        }

        public Element GetAreaElement()
        {
            return Element.Lightning;
        }

        public bool IsDisabled()
        {
            return !isCharged;
        }

        public bool IsSource()
        {
            return false;
        }

        public IHasElementalArea GetSource()
        {
            return reactionHandler.LightningSource;
        }
    }
}