using Godot;

namespace Inversion
{
    public class PowerTerminal : Node2D, IWirePowered, IHasElementalArea
    {
        [Export]
        private NodePath connectedPoweredNodePath;
        private Sprite sprite;
        private bool hasElementalPower = false;
        private bool hasWirePower = false;
        private IWirePowered connectedPoweredNode;
        private Particles2D sparks;
        private ReactionHandler reactionHandler;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");

            reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));
            sparks = GetNode<Particles2D>("Sparks");

            if (connectedPoweredNodePath != null && !connectedPoweredNodePath.IsEmpty() && GetNode(connectedPoweredNodePath) is IWirePowered wirePowered)
            {
                connectedPoweredNode = wirePowered;
            }

            CallDeferred(nameof(PowerChanged));
        }

        public void WirePower()
        {
            if (hasWirePower)
                return;

            hasWirePower = true;

            PowerChanged();
        }

        public void WireUnPower()
        {
            hasWirePower = false;

            PowerChanged();
        }

        private void ElementStarted(Element element, Node source)
        {
            if (element == Element.Lightning && source != null)
            {
                GD.Print($"{source.Name}");

                reactionHandler.LightningSource = (IHasElementalArea)source;

                hasElementalPower = true;

                PowerChanged();
            }
        }

        private void ElementEnded(Element element)
        {
            if (element == Element.Lightning)
            {
                hasElementalPower = false;

                PowerChanged();
            }
        }

        private void PowerChanged()
        {
            if (hasWirePower || hasElementalPower)
            {
                GD.Print($"{Name} powered");

                if (connectedPoweredNode != null)
                {
                    connectedPoweredNode.WirePower();
                }

                sparks.Emitting = true;

                GlobalNodes.Singleton.MakeBzztSound();
            }
            else
            {
                GD.Print($"{Name} unpowered");

                if (connectedPoweredNode != null)
                {
                    connectedPoweredNode.WireUnPower();
                }

                sparks.Emitting = false;
            }
        }

        public Element GetAreaElement()
        {
            return Element.Lightning;
        }

        public bool IsDisabled()
        {
            return !(hasWirePower || hasElementalPower);
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