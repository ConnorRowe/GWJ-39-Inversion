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
            Player.InversionOrb = GetNode<InversionOrb>("InversionOrb");

            Player.Connect(nameof(Player.PlayerDied), this, nameof(PlayerDied));
        }

        private void PlayerDied()
        {
            RestartLevel();
        }

        private void RestartLevel()
        {
            Player.Position = GetNode<Node2D>(playerStartPosition).Position;
        }
    }
}