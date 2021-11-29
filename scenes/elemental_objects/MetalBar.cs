using Godot;

namespace Inversion
{
    // [Tool]
    public class MetalBar : Node2D, IHasElementalArea, IMetallic
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
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }

        public override void _Draw()
        {
            var extents = ((RectangleShape2D)staticCollisionRect.Shape).Extents;
            sprite.RegionRect = new Rect2(Vector2.Zero, extents * 2);
            var sparksMat = (ParticlesMaterial)sparks.ProcessMaterial;
            sparksMat.EmissionShape = ParticlesMaterial.EmissionShapeEnum.Box;
            sparksMat.EmissionBoxExtents = new Vector3(extents.x + 4, extents.y + 4, 0);
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

        public System.Collections.Generic.HashSet<IMetallic> GetNearbyMetallics()
        {
            return reactionHandler.NearbyMetallics;
        }
    }
}