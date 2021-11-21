using Godot;

namespace Inversion
{
    public class MainMenu : Node2D
    {
        public override void _Ready()
        {
            SaveData.ApplySavedSettings();

            GetNode("Play").Connect("pressed", GetTree(), "change_scene_to", new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/LevelSelection.tscn")));
            GetNode("Controls").Connect("pressed", GetTree(), "change_scene_to", new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/ControlsScreen.tscn")));
            GetNode("Settings").Connect("pressed", GetTree(), "change_scene_to", new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/SettingsScreen.tscn")));
            GetNode("Play").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            GetNode("Controls").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            GetNode("Settings").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
        }
    }
}