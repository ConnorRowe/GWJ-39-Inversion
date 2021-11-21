using Godot;

namespace Inversion
{
    public class ControlsScreen : Node2D
    {
        public override void _Ready()
        {
            GetNode("Return").Connect("pressed", GetTree(), "change_scene_to", new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn")));
            GetNode("Return").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
        }
    }
}