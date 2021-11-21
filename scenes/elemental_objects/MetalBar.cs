using Godot;

namespace Inversion
{
    // [Tool]
    public class MetalBar : Node2D, IHasElementalArea
    {
        private CollisionShape2D staticCollisionRect;
        private CollisionShape2D elemCollisionRect;
        private Particles2D sparks;
        private Sprite sprite;
        private bool isCharged = false;
        private ReactionHandler reactionHandler;

        public override void _Ready()
        {
            base._Ready();

            staticCollisionRect = GetNode<CollisionShape2D>("StaticBody2D/CollisionRect");
            elemCollisionRect = GetNode<CollisionShape2D>("LightningArea/CollisionRect");
            sparks = GetNode<Particles2D>("Sparks");
            sprite = GetNode<Sprite>("Sprite");
            reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (reactionHandler.LightningSource == null || (reactionHandler.LightningSource != null && reactionHandler.LightningSource.IsDisabled()))
            {
                isCharged = false;
                sparks.Emitting = false;
            }
        }

        public override void _Draw()
        {
            var extents = ((RectangleShape2D)staticCollisionRect.Shape).Extents;
            sprite.RegionRect = new Rect2(Vector2.Zero, extents * 2);
            var sparksMat = (ParticlesMaterial)sparks.ProcessMaterial;
            sparksMat.EmissionShape = ParticlesMaterial.EmissionShapeEnum.Box;
            sparksMat.EmissionBoxExtents = new Vector3(extents.x*1.5f, extents.y*1.5f, 0);
            sparks.Amount = Mathf.FloorToInt((extents.x * extents.y) / 30f);
        }

        public Element GetAreaElement()
        {
            return Element.Lightning;
        }

        public bool IsDisabled()
        {
            return !isCharged;
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