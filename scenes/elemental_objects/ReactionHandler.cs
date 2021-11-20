using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class ReactionHandler : Node2D
    {
        [Signal]
        public delegate void ElementStarted(Element element, Node source);
        [Signal]
        public delegate void ElementEnded(Element element);

        private static readonly Dictionary<Element, bool> emptyElementOverlaps = new Dictionary<Element, bool>()
        {
            {Element.Fire, false},
            {Element.Water, false},
            {Element.Earth, false},
            {Element.Lightning, false},
        };

        [Export]
        private NodePath collisionShapePath;
        [Export]
        private bool NeedsSource = false;

        public bool IsActive { get; set; } = true;
        public bool FlipH { get; set; } = false;

        private Node2D collisionShape;
        private Shape2D shape;
        private Physics2DShapeQueryParameters shapeQueryParams;
        private Dictionary<Element, bool> elementOverlaps = new Dictionary<Element, bool>(emptyElementOverlaps);
        public IHasElementalArea LightningSource { get; set; } = null;


        public override void _Ready()
        {
            collisionShape = GetNode<Node2D>(collisionShapePath);

            if (collisionShape is CollisionShape2D collisionShape2D)
            {
                shape = collisionShape2D.Shape;
            }
            else if (collisionShape is CollisionPolygon2D collisionPolygon2D)
            {
                collisionPolygon2D.BuildMode = CollisionPolygon2D.BuildModeEnum.Segments;
                ConcavePolygonShape2D poly = new ConcavePolygonShape2D();
                poly.Segments = collisionPolygon2D.Polygon;

                shape = poly;
            }

            shapeQueryParams = new Physics2DShapeQueryParameters();
            shapeQueryParams.SetShape(shape);
            var xform = new Transform2D(0, GlobalPosition + collisionShape.Position);
            xform.Scale = new Vector2(FlipH ? -1f : -1f, 1f);
            shapeQueryParams.CollideWithAreas = true;
            shapeQueryParams.CollisionLayer = 4;
        }

        public override void _PhysicsProcess(float delta)
        {
            var spaceState = GetWorld2d().DirectSpaceState;
            shapeQueryParams.Transform = new Transform2D(0, GlobalPosition + collisionShape.Position);
            var results = spaceState.IntersectShape(shapeQueryParams);

            var newElemOverlaps = new Dictionary<Element, bool>(emptyElementOverlaps);

            foreach (Godot.Collections.Dictionary hit in results)
            {
                Node hitNode = ((Node)hit["collider"]);

                if (hitNode.Owner != Owner && hitNode.Owner is IHasElementalArea hasElementalArea && !hasElementalArea.IsDisabled())
                {
                    newElemOverlaps[hasElementalArea.GetAreaElement()] = true;

                    if (hasElementalArea.GetAreaElement() == Element.Lightning)
                    {
                        if (hasElementalArea.IsSource())
                            LightningSource = hasElementalArea;
                        else if (hasElementalArea.GetSource() != null && !hasElementalArea.GetSource().IsDisabled())
                            LightningSource = hasElementalArea.GetSource();
                    }
                }
            }

            if (NeedsSource && (LightningSource == null || (LightningSource != null && LightningSource.IsDisabled())))
            {
                newElemOverlaps[Element.Lightning] = false;
            }

            foreach (var elem in newElemOverlaps.Keys)
            {
                if (newElemOverlaps[elem] != elementOverlaps[elem])
                {
                    if (newElemOverlaps[elem])
                    {
                        if (elem == Element.Lightning && LightningSource is Node node)
                            EmitSignal(nameof(ElementStarted), elem, node);
                        else
                            EmitSignal(nameof(ElementStarted), elem, null);
                    }
                    else
                        EmitSignal(nameof(ElementEnded), elem);

                    elementOverlaps[elem] = newElemOverlaps[elem];
                }
            }
        }

        private void ResetElementOverlaps()
        {
            foreach (var elem in elementOverlaps.Keys)
            {
                elementOverlaps[elem] = false;
            }
        }
    }
}