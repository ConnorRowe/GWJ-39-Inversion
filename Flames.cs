using Godot;

namespace Inversion
{
    public class Flames : Particles2D
    {
        private Node2D owner;

        public override void _Ready()
        {
            base._Ready();

            owner = (Node2D)Owner;
        }

        public override void _Process(float delta)
        {
            Rotation = -owner.Rotation;
        }
    }
}