using Godot;

namespace Inversion
{
    public class ControlsScreen : BaseMenu
    {
        public override void _Ready()
        {
            base._Ready();

            GetNode("Return").Connect("pressed", this, nameof(TransitionToScene), new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn")));
            GetNode("Return").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
        }
    }
}