using Godot;

namespace Inversion
{
    public class BaseLevel : Node2D
    {
        [Export]
        private NodePath playerStartPosition;

        public Player Player { get; set; }

        public override void _Ready()
        {
            // Spawn player
            var startPos = GetNode<Node2D>(playerStartPosition);
            Player = GD.Load<PackedScene>("res://scenes/Player.tscn").Instance<Player>();
            AddChild(Player);
            Player.Position = startPos.Position;
        }


    }
}