using Godot;

namespace Inversion
{
    public class WireBranch : Node2D, IWirePowered
    {
        private bool isPowered = false;

        [Export]
        private Godot.Collections.Array<NodePath> connectedPoweredNodePaths = new Godot.Collections.Array<NodePath>();
        private System.Collections.Generic.HashSet<IWirePowered> connectedPoweredNodes = new System.Collections.Generic.HashSet<IWirePowered>();

        public override void _Ready()
        {
            base._Ready();

            var label = GetChild(0);
            RemoveChild(label);
            label.QueueFree();

            foreach (var path in connectedPoweredNodePaths)
            {
                if (!path.IsEmpty() && GetNode<Node2D>(path) is IWirePowered wirePowered)
                {
                    connectedPoweredNodes.Add(wirePowered);
                }
            }

            connectedPoweredNodePaths.Clear();
        }

        public void WirePower()
        {
            isPowered = true;

            GD.Print($"{Name} powered");

            foreach (var wirePowered in connectedPoweredNodes)
            {
                wirePowered.WirePower();
            }
        }

        public void WireUnPower()
        {
            isPowered = false;

            GD.Print($"{Name} unpowered");

            foreach (var wirePowered in connectedPoweredNodes)
            {
                wirePowered.WireUnPower();
            }
        }
    }
}