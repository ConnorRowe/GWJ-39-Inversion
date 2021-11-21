using Godot;

namespace Inversion
{
    public class LevelSelection : Node2D
    {
        public override void _Ready()
        {
            int cnt = Globals.AllLevels.Count;

            GD.Print($"Level count: {cnt}");
            GD.Print($"MaxLevel: {SaveData.MaxLevel}");

            Button buttonToCopy = GetNode<Button>("ScrollContainer/GridContainer/Button");
            buttonToCopy.GetParent().RemoveChild(buttonToCopy);

            GridContainer gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");

            int i = 0;
            foreach (var level in Globals.AllLevels)
            {
                var newButton = (Button)buttonToCopy.Duplicate();
                newButton.Text = $"{i + 1}";
                newButton.Disabled = i > SaveData.MaxLevel;
                newButton.Connect("pressed", this, nameof(StartLevel), new Godot.Collections.Array(i));
                newButton.Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));

                gridContainer.AddChild(newButton);

                i++;
            }

            GetNode("Return").Connect("pressed", GetTree(), "change_scene_to", new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn")));
            GetNode("Return").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
        }

        private void StartLevel(int level)
        {
            GetTree().ChangeSceneTo(Globals.AllLevels[level]);
            Globals.CurrentLevel = level;
        }
    }
}