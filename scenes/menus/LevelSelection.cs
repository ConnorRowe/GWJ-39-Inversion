using Godot;

namespace Inversion
{
    public class LevelSelection : BaseMenu
    {
        public override void _Ready()
        {
            base._Ready();

            int lvlCount = Globals.AllLevels.Count;

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

            GetNode("Return").Connect("pressed", this, nameof(TransitionToScene), new Godot.Collections.Array(GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn")));
            GetNode("Return").Connect("mouse_entered", GlobalNodes.Singleton, nameof(GlobalNodes.UIClick));
        }

        private void StartLevel(int level)
        {
            Globals.CurrentLevel = level;

            TransitionToScene(Globals.AllLevels[level]);
        }
    }
}