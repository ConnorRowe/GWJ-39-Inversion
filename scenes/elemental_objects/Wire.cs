using Godot;

namespace Inversion
{
    [Tool]
    public class Wire : Node2D, IWirePowered
    {
        private static readonly DynamicFont smallFont = GD.Load<DynamicFont>("res://font/small_dynamicfont.tres");

        private Position2D startPos;
        private Position2D endPos;
        [Export]
        public bool IsPowered { get { return isPowered; } set { isPowered = value; Update(); } }
        private bool isPowered = false;
        [Export]
        private NodePath connectedPoweredNodePath;
        private IWirePowered connectedPoweredNode;

        private Vector2[] lastUpdatePositions = new Vector2[2] { Vector2.Zero, Vector2.Zero };

        public override void _Ready()
        {
            startPos = GetNode<Position2D>("StartPos");
            endPos = GetNode<Position2D>("EndPos");

            if (connectedPoweredNodePath != null && !connectedPoweredNodePath.IsEmpty() && GetNode(connectedPoweredNodePath) is IWirePowered wirePowered)
            {
                connectedPoweredNode = wirePowered;
            }
        }

        public override void _Process(float delta)
        {
            if (lastUpdatePositions[0] != startPos.Position || lastUpdatePositions[1] != endPos.Position)
                Update();
        }

        public override void _Draw()
        {
            DrawLine(startPos.Position, endPos.Position, isPowered ? Element.Lightning.GetColour() : Element.Earth.GetColour(), 4, true);

            lastUpdatePositions[0] = startPos.Position;
            lastUpdatePositions[1] = endPos.Position;

            if (Engine.EditorHint)
                DrawString(smallFont, startPos.Position.LinearInterpolate(endPos.Position, .5f), Name);
        }

        public void WirePower()
        {
            isPowered = true;
            Update();

            GD.Print($"{Name} powered");

            if (connectedPoweredNode != null)
            {
                connectedPoweredNode.WirePower();
            }
        }

        public void WireUnPower()
        {
            isPowered = false;
            Update();

            GD.Print($"{Name} unpowered");

            if (connectedPoweredNode != null)
            {
                connectedPoweredNode.WireUnPower();
            }
        }
    }
}