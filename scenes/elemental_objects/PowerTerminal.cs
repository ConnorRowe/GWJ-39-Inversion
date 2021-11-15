using Godot;

namespace Inversion
{
    public class PowerTerminal : Node2D, IWirePowered, IHasElementalArea
    {
        [Export]
        private NodePath linkedWirePath;
        private Sprite sprite;
        private bool hasElementalPower = false;
        private bool hasWirePower = false;
        private Wire linkedWire;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");

            var reactionHandler = GetNode<ReactionHandler>("ReactionHandler");
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));

            if (linkedWirePath != null && !linkedWirePath.IsEmpty() && GetNode(linkedWirePath) is Wire wire)
            {
                linkedWire = wire;
            }

            PowerChanged();
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

        private void ElementStarted(Element element)
        {
            if (element == Element.Lightning)
            {
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
                sprite.SelfModulate = Element.Lightning.GetColour();
                GD.Print($"{Name} powered");

                if (IsInstanceValid(linkedWire))
                {
                    linkedWire.WirePower();
                }
            }
            else
            {
                sprite.SelfModulate = Element.Earth.GetColour();
                GD.Print($"{Name} unpowered");

                if (IsInstanceValid(linkedWire))
                {
                    linkedWire.WireUnPower();
                }
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
    }
}