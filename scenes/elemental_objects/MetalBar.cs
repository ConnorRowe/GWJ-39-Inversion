using Godot;

namespace Inversion
{
    [Tool]
    public class MetalBar : Node2D, IHasElementalArea
    {
        private CollisionShape2D baseCollisionRect;
        private CollisionShape2D elemCollisionRect;
        private Particles2D sparks;
        private Sprite sprite;
        private bool isCharged = false;

        public override void _Ready()
        {
            base._Ready();

            elemCollisionRect = GetNode<CollisionShape2D>("LightningArea/CollisionRect");
            sparks = GetNode<Particles2D>("Sparks");
            sprite = GetNode<Sprite>("Sprite");
            GetNode("ReactionHandler").Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            GetNode("ReactionHandler").Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));

            if (!Engine.EditorHint)
            {
                var extents = ((RectangleShape2D)elemCollisionRect.Shape).Extents;
                baseCollisionRect = new CollisionShape2D();
                var rectShape = new RectangleShape2D();
                rectShape.Extents = extents;
                baseCollisionRect.Shape = rectShape;
                GetNode("StaticBody2D").AddChild(baseCollisionRect);
                baseCollisionRect.Position = elemCollisionRect.Position;

                ((RectangleShape2D)elemCollisionRect.Shape).Extents = extents + new Vector2(6f, 3f);
            }
        }

        public override void _Draw()
        {
            var extents = ((RectangleShape2D)baseCollisionRect.Shape).Extents;
            sprite.RegionRect = new Rect2(Vector2.Zero, extents * 2);
            var sparksMat = (ParticlesMaterial)sparks.ProcessMaterial;
            sparksMat.EmissionShape = ParticlesMaterial.EmissionShapeEnum.Box;
            sparksMat.EmissionBoxExtents = new Vector3(extents.x, extents.y, 0);
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

        private void ElementStarted(Element element, IHasElementalArea source)
        {
            if (element == Element.Lightning)
            {
                isCharged = true;
                sparks.Emitting = true;
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
            return null;
        }
    }
}