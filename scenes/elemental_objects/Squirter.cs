using Godot;

namespace Inversion
{
    public class Squirter : ElementalObject
    {
        private Particles2D fire;
        private Particles2D water;
        private AnimatedSprite fireSprite;

        public override void _Ready()
        {
            fire = GetNode<Particles2D>("FireStuff/Particles2D");
            water = GetNode<Particles2D>("WaterStuff/Particles2D");
            fireSprite = GetNode<AnimatedSprite>("FireStuff/FireSprite");

            base._Ready();
        }

        public override void Invert()
        {
            base.Invert();

            fire.Emitting = !fire.Emitting;
            water.Emitting = !water.Emitting;
            fireSprite.Visible = fire.Emitting;

        }
    }
}