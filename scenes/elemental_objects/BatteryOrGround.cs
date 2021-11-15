using Godot;

namespace Inversion
{
    public class BatteryOrGround : ElementalObject
    {
        private Sprite sprite;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");

            base._Ready();
        }

        public override void Invert()
        {
            base.Invert();

            sprite.SelfModulate = currentElement.GetColour();
        }
    }
}