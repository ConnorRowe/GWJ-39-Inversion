using Godot;

namespace Inversion
{
    public class SettingsScreen : Node2D
    {
        public override void _Ready()
        {
            GetNode("Return").Connect("pressed", this, nameof(Return));
        }

        private void Return()
        {
            GetNode<Settings>("PanelContainer/Settings").SaveSettings();

            GetTree().ChangeSceneTo(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn"));
        }
    }
}