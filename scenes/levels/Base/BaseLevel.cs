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
        public VirtualJoystick MoveStick { get; set; }
        public VirtualJoystick OrbStick { get; set; }
        private bool playerDead = false;
        private bool canRespawn = false;
        private bool paused = false;
        private Tween tween;
        private Transition transition;

        public Label Debug { get; set; }

        public override void _Ready()
        {
            tween = GetNode<Tween>("Tween");

            Debug = GetNode<Label>("UILayer/debug");

            var unpauseBtn = GetNode<Button>("UILayer/PausePopup/Control/Unpause");
            var exit = GetNode<Button>("UILayer/PausePopup/Control/Exit");

            unpauseBtn.Connect("pressed", this, nameof(UnPause));
            exit.Connect("pressed", this, nameof(Exit));
            unpauseBtn.Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            exit.Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));

            var levelNameParent = GetNode<Node2D>("UILayer/LabelNameParent");
            var levelNameLabel = GetNode<Label>("UILayer/LabelNameParent/LevelName");

            levelNameLabel.Text = LevelName;
            levelNameParent.Visible = true;
            CallDeferred(nameof(UpdateLevelNamePos));

            tween.InterpolateProperty(levelNameParent, "scale", Vector2.Zero, Vector2.One, .6f, Tween.TransitionType.Bounce);
            tween.InterpolateProperty(levelNameParent, "scale", Vector2.One, Vector2.Zero, .6f, Tween.TransitionType.Cubic, Tween.EaseType.In, 6f);
            tween.Start();

            GetTree().CreateTimer(6.6f).Connect("timeout", levelNameParent, "set", new Godot.Collections.Array("visible", false));

            MoveStick = GetNode<VirtualJoystick>("UILayer/MoveJoystick");
            OrbStick = GetNode<VirtualJoystick>("UILayer/OrbJoystick");
            var pauseBtn = GetNode<Button>("UILayer/PauseButton");
            var resetBtn = GetNode<Button>("UILayer/ResetButton");

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

            if (Globals.HasTouchscreen)
            {
                pauseBtn.Connect("pressed", this, nameof(Pause));
                resetBtn.Connect("pressed", Player, "KillPlayer");

                GetNode<Label>("UILayer/YouDied/Label").Text = "You Died.\n\nTap the screen to respawn.";
            }

            Globals.UpdateDiscordActivityLevel(LevelName);

            transition = GetNode<Transition>("Transition");
        }

        // public override void _Process(float delta)
        // {
        //     base._Process(delta);

        //     Debug.Text = $"{Engine.GetFramesPerSecond()} fps";
        // }

        public override void _Input(InputEvent evt)
        {
            if (canRespawn && playerDead && (evt.IsActionReleased("g_respawn") || evt is InputEventScreenTouch est && !est.Pressed))
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
            var bg = GetNode<ColorRect>("UILayer/DeadColorRect");
            var youDied = GetNode<Node2D>("UILayer/YouDied");
            var deathCount = GetNode<Label>("UILayer/DeathCountParent/DeathCount");

            deathCount.Text = $"You've died {Globals.LevelDeathCount} time{(Globals.LevelDeathCount == 1 ? "" : "s")} on this level so far...";
            bg.Visible = youDied.Visible = deathCount.Visible = true;

            tween.InterpolateProperty(bg, "modulate", Colors.Transparent, new Color(1f, 1f, 1f, .7f), 2.5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(youDied, "modulate", Colors.Transparent, Colors.White, 2.5f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(deathCount, "modulate", Colors.Transparent, Colors.White, 2.5f, Tween.TransitionType.Cubic);

            tween.Start();

            GetTree().CreateTimer(.75f).Connect("timeout", this, nameof(EnableRespawn));
        }

        private async void RestartLevel()
        {
            if (transition.Active)
                return;

            var timer = GetTree().CreateTimer(1.25f);
            transition.StartTransition();
            await ToSignal(timer, "timeout");

            GetTree().ReloadCurrentScene();
            QueueFree();
        }

        public async void GotoNextLevel()
        {
            var timer = GetTree().CreateTimer(1.25f);
            transition.StartTransition();
            await ToSignal(timer, "timeout");

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

        private async void Exit()
        {
            paused = GetTree().Paused = false;
            GetNode<Settings>("UILayer/PausePopup/Control/PanelContainer/Settings").SaveSettings();
            GetNode<PopupPanel>("UILayer/PausePopup").Hide();

            var timer = GetTree().CreateTimer(1.25f);
            transition.StartTransition();
            await ToSignal(timer, "timeout");

            GetTree().ChangeSceneTo(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn"));
        }

        private void UpdateLevelNamePos()
        {
            var levelNameLabel = GetNode<Label>("UILayer/LabelNameParent/LevelName");
            levelNameLabel.RectPosition = -levelNameLabel.RectSize / 2f;
        }
    }
}