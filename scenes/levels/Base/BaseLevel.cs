using Godot;

namespace Inversion
{
    public class BaseLevel : Node2D
    {
        [Export]
        public string LevelName { get; set; } = "";
        [Export]
        private NodePath playerStartPosition;
        [Export]
        private Rect2 levelBounds = new Rect2(0, 640, 0, 360);
        [Export]
        private PackedScene nextLevel;

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
            var camera = Player.GetNode<Camera2D>("Camera2D");
            camera.LimitLeft = (int)levelBounds.Position.x;
            camera.LimitRight = (int)levelBounds.Position.y;
            camera.LimitTop = (int)levelBounds.Size.x;
            camera.LimitBottom = (int)levelBounds.Size.y;
        }

        private void PlayerDied()
        {
            RestartLevel();
        }

        private void RestartLevel()
        {
            // Player.Position = GetNode<Node2D>(playerStartPosition).Position;

            GetTree().ReloadCurrentScene();
            QueueFree();
        }

        public void GotoNextLevel()
        {
            GetTree().ChangeSceneTo(nextLevel);
            QueueFree();
        }
    }
}