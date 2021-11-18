using Godot;

namespace Inversion
{
    [Tool]
    public class Lava : Node2D, IHasElementalArea
    {
        private static readonly Color lavaColour = new Color("ff8933");
        private CollisionShape2D collisionRect;
        private Particles2D flames;

        public override void _Ready()
        {
            collisionRect = GetNode<CollisionShape2D>("Area2D/CollisionRect");
            flames = GetNode<Particles2D>("Flames");

            if (!Engine.EditorHint)
            {
                var extents = ((RectangleShape2D)collisionRect.Shape).Extents;
                var staticCollider = new CollisionShape2D();
                var rectShape = new RectangleShape2D();
                rectShape.Extents = new Vector2(extents.x, extents.y * .8f);
                staticCollider.Shape = rectShape;
                GetNode("StaticBody2D").AddChild(staticCollider);
                staticCollider.Position = collisionRect.Position;
            }
        }

        public override void _Draw()
        {
            var extents = ((RectangleShape2D)collisionRect.Shape).Extents;
            DrawRect(new Rect2(collisionRect.Position - extents, extents * 2), lavaColour);
            flames.Position = collisionRect.Position + new Vector2(0, -extents.y);
            ((ParticlesMaterial)flames.ProcessMaterial).EmissionBoxExtents = new Vector3(extents.x, 1, 0);
            flames.Amount = (int)extents.x;
        }

		public Element GetAreaElement()
        {
            return Element.Fire;
        }

        public bool IsDisabled()
        {
            return false;
        }
    }
}