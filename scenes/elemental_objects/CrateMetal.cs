using Godot;

namespace Inversion
{
    public class CrateMetal : RigidBody2D, IHasElementalArea
    {
        private bool isCharged = false;
        private Particles2D sparks;

        public override void _Ready()
        {
            sparks = GetNode<Particles2D>("Sparks");
            GetNode("ReactionHandler").Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            GetNode("ReactionHandler").Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));
        }

        private void ElementStarted(Element element)
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

        public Element GetAreaElement()
        {
            return Element.Lightning;
        }

        public bool IsDisabled()
        {
            return !isCharged;
        }
    }
}