using Godot;

namespace Inversion
{
    public class Squirter : Node2D, IInvertable, IHasElementalArea
    {
        private Element currentElement = Element.Fire;
        private bool isDisabled = false;

        private Particles2D fire;
        private Particles2D water;

        private Area2D sprayArea;

        public override void _Ready()
        {
            fire = GetNode<Particles2D>("FireStuff/Particles2D");
            water = GetNode<Particles2D>("WaterStuff/Particles2D");

            sprayArea = GetNode<Area2D>("SprayArea");
        }

        public void Invert()
        {
            currentElement = currentElement.Invert();

            fire.Emitting = !fire.Emitting;
            water.Emitting = !water.Emitting;
        }

        public Element GetCurrentElement()
        {
            return currentElement;
        }

        public bool IsDisabled()
        {
            return isDisabled;
        }

        public Element GetAreaElement()
        {
            return currentElement;
        }
    }
}