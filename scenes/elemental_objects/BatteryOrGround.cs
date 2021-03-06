using Godot;

namespace Inversion
{
    public class BatteryOrGround : ElementalObject
    {
        private static Texture batteryTex = GD.Load<Texture>("res://textures/battery.png");
        private static Texture groundTex = GD.Load<Texture>("res://textures/stalegtite.png");

        private Sprite sprite;
        private Particles2D sparks;
        private PowerSourceHandler powerSourceHandler;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");
            sparks = GetNode<Particles2D>("Sparks");
            powerSourceHandler = GetNode<PowerSourceHandler>("PowerSourceHandler");
            powerSourceHandler.IsPowered = currentElement == Element.Lightning;

            base._Ready();
        }

        public override void Invert()
        {
            base.Invert();

            sparks.Emitting = currentElement == Element.Lightning;
            powerSourceHandler.IsPowered = currentElement == Element.Lightning;

            if (currentElement == Element.Lightning)
                sprite.Texture = batteryTex;
            else
                sprite.Texture = groundTex;


        }

        public override bool IsDisabled()
        {
            return currentElement != Element.Lightning;
        }
    }
}