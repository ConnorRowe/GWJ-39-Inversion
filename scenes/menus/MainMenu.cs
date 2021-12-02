using Godot;

namespace Inversion
{
    public class MainMenu : BaseMenu
    {
        public override void _Ready()
        {
            base._Ready();

            SaveData.ApplySavedSettings();

            GetNode("Play").Connect("pressed", this, nameof(TransitionToScene), new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/LevelSelection.tscn")));
            GetNode("Controls").Connect("pressed", this, nameof(TransitionToScene), new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/ControlsScreen.tscn")));
            GetNode("Settings").Connect("pressed", this, nameof(TransitionToScene), new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/SettingsScreen.tscn")));
            GetNode("Play").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            GetNode("Controls").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            GetNode("Settings").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));

            Globals.HasTouchscreen = OS.HasTouchscreenUiHint();

            Globals.UpdateDiscordActivityMenu();
        }
    }
}