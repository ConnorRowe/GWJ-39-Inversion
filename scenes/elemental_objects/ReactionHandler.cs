using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public class ReactionHandler : Node2D
    {
        [Signal]
        public delegate void ElementStarted(Element element);
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
        protected NodePath collisionShapePath;
        [Export]
        protected bool trackMetallics = false;
        [Export]
        protected Godot.Collections.Array<NodePath> exludedNodes;

        public bool IsActive { get; set; } = true;
        public bool FlipH { get; set; } = false;

        private Node2D collisionShape;
        private Shape2D shape;
        private Physics2DShapeQueryParameters shapeQueryParams;
        private Dictionary<Element, bool> elementOverlaps = new Dictionary<Element, bool>(emptyElementOverlaps);
        public List<IMetallic> NearbyMetallics = new List<IMetallic>();


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
            var xform = new Transform2D(GlobalRotation, GlobalPosition + collisionShape.Position);
            xform.Scale = new Vector2(FlipH ? -1f : -1f, 1f);
            shapeQueryParams.CollideWithAreas = true;
            shapeQueryParams.CollisionLayer = 4;

            if (exludedNodes != null)
            {
                var exclude = new Godot.Collections.Array();
                foreach (var path in exludedNodes)
                {
                    if (Owner.GetNode(path) is Node node)
                    {
                        exclude.Add(node);
                    }
                }

                shapeQueryParams.Exclude = exclude;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            var spaceState = GetWorld2d().DirectSpaceState;
            var xform = new Transform2D(GlobalRotation, GlobalPosition + collisionShape.Position);
            xform.Scale = new Vector2(FlipH ? -1f : -1f, 1f);
            shapeQueryParams.Transform = xform;
            var results = spaceState.IntersectShape(shapeQueryParams);

            var newElemOverlaps = new Dictionary<Element, bool>(emptyElementOverlaps);
            NearbyMetallics = new List<IMetallic>();

            foreach (Godot.Collections.Dictionary hit in results)
            {
                Node hitNode = ((Node)hit["collider"]);

                if (hitNode.Owner != Owner && hitNode.Owner is IHasElementalArea hasElementalArea)
                {
                    if (!hasElementalArea.IsDisabled())
                        newElemOverlaps[hasElementalArea.GetAreaElement()] = true;

                    if (trackMetallics && hasElementalArea is IMetallic metallic)
                        NearbyMetallics.Add(metallic);
                }
            }

            foreach (var elem in newElemOverlaps.Keys)
            {
                if (newElemOverlaps[elem] != elementOverlaps[elem])
                {
                    if (newElemOverlaps[elem])
                        EmitSignal(nameof(ElementStarted), elem);

                    else
                        EmitSignal(nameof(ElementEnded), elem);

                    elementOverlaps[elem] = newElemOverlaps[elem];
                }
            }
        }
    }
}