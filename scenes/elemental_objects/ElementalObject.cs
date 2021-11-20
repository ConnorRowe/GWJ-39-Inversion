using Godot;

namespace Inversion
{
    public class ElementalObject : Node2D, IInvertable, IHasElementalArea
    {
        [Export]
        private bool invertOnStart = false;
        [Export]
        private NodePath elementalAreaPath;
        [Export]
        protected Element currentElement = Element.Fire;
        protected bool isDisabled = false;
        protected Area2D elementalArea;


        public override void _Ready()
        {
            elementalArea = GetNode<Area2D>(elementalAreaPath);

            if (invertOnStart)
                Invert();
        }

        public virtual void Invert()
        {
            currentElement = currentElement.Invert();
        }

        public Element GetCurrentElement()
        {
            return currentElement;
        }

        public Element GetAreaElement()
        {
            return GetCurrentElement();
        }

        public bool IsInversionDisabled()
        {
            return isDisabled;
        }

        public virtual bool IsDisabled()
        {
            return isDisabled;
        }

        public bool IsSource()
        {
            return true;
        }
        public IHasElementalArea GetSource()
        {
            return null;
        }
    }
}