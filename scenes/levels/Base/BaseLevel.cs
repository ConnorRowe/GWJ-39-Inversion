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
        public Rect2 levelBounds = new Rect2(0, 640, 0, 360);
        [Export]
        private PackedScene nextLevel;

        public Player Player { get; set; }
        private bool playerDead = false;
        private bool canRespawn = false;
        private bool paused = false;

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

            var unpauseBtn = GetNode<Button>("UILayer/PausePopup/Control/Unpause");
            var exit = GetNode<Button>("UILayer/PausePopup/Control/Exit");

            unpauseBtn.Connect("pressed", this, nameof(UnPause));
            exit.Connect("pressed", this, nameof(Exit));
            unpauseBtn.Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            exit.Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
        }

        public override void _Input(InputEvent evt)
        {
            if (canRespawn && playerDead && evt.IsActionReleased("g_respawn"))
            {
                RestartLevel();
            }

            if (!paused && evt.IsActionReleased("g_pause"))
            {
                Pause();
            }
        }

        private void PlayerDied()
        {
            playerDead = true;
            Globals.LevelDeathCount++;
            Player.IsFrozen = true;

            // Show dead UI
            var tween = GetNode<Tween>("Tween");
            var bg = GetNode<ColorRect>("UILayer/DeadColorRect");
            var youDied = GetNode<Node2D>("UILayer/YouDied");
            var deathCount = GetNode<Label>("UILayer/DeathCount");

            deathCount.Text = $"You've died {Globals.LevelDeathCount} time{(Globals.LevelDeathCount == 1 ? "" : "s")} on this level so far...";
            bg.Visible = youDied.Visible = deathCount.Visible = true;

            tween.InterpolateProperty(bg, "modulate", Colors.Transparent, new Color(1f, 1f, 1f, .7f), 2.5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(youDied, "modulate", Colors.Transparent, Colors.White, 2.5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(deathCount, "modulate", Colors.Transparent, Colors.White, 2.5f, Tween.TransitionType.Cubic);

            tween.Start();

            GetTree().CreateTimer(.75f).Connect("timeout", this, nameof(EnableRespawn));
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
            Globals.LevelDeathCount = 0;
            Globals.CurrentLevel++;
            if (Globals.CurrentLevel > SaveData.MaxLevel)
                SaveData.SaveMaxLevel(Globals.CurrentLevel);
        }

        private void EnableRespawn()
        {
            canRespawn = true;
        }

        private void Pause()
        {
            GetNode<PopupPanel>("UILayer/PausePopup").PopupCentered();

            paused = GetTree().Paused = true;
        }

        private void UnPause()
        {
            GetNode<PopupPanel>("UILayer/PausePopup").Hide();
            GetNode<Settings>("UILayer/PausePopup/Control/PanelContainer/Settings").SaveSettings();

            paused = GetTree().Paused = false;
        }

        private void Exit()
        {
            paused = GetTree().Paused = false;
            GetNode<Settings>("UILayer/PausePopup/Control/PanelContainer/Settings").SaveSettings();

            GetTree().ChangeSceneTo(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn"));
        }
    }
}