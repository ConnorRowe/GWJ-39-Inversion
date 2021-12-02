using Godot;

namespace Inversion
{
    public class SettingsScreen : BaseMenu
    {
        public override void _Ready()
        {
            base._Ready();

            GetNode("Return").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
            GetNode("Return").Connect("pressed", this, nameof(Return));
        }

        private void Return()
        {
            GetNode<Settings>("PanelContainer/Settings").SaveSettings();

            TransitionToScene(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn"));
        }
    }
}