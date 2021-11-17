using Godot;

namespace Inversion
{
    public class GoalPlatform : Node2D
    {
        private AnimatedSprite eolExplosion;
        private Tween tween;
        private Player player;
        private bool hasStarted = false;
        private bool levelEnded = false;

        public override void _Ready()
        {
            base._Ready();

            tween = GetNode<Tween>("Tween");
            GetNode<Area2D>("PlayerDetection").Connect("area_entered", this, nameof(AreaEntered));
            eolExplosion = GetNode<AnimatedSprite>("EOLExplosion");
            eolExplosion.Connect("frame_changed", this, nameof(EOLExplosionFrameChanged));
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Arrow");
        }

        private void AreaEntered(Area2D area)
        {
            if (!hasStarted && area.Owner is Player p)
            {
                hasStarted = true;
                player = p;
                player.IsFrozen = true;
                tween.InterpolateProperty(player, "global_position", player.GlobalPosition, GlobalPosition + new Vector2(0, -5), .25f, Tween.TransitionType.Sine);
                tween.InterpolateProperty(GetNode("Arrow"), "modulate", new Color(.78f, .83f, .36f), new Color(.78f, .83f, .36f, 0f), .25f, Tween.TransitionType.Cubic);
                tween.Start();
                eolExplosion.Play("default");
            }
        }

        private void EOLExplosionFrameChanged()
        {
            if (eolExplosion.Frame == 4)
            {
                tween.InterpolateProperty(player, "modulate", Colors.White, Colors.Transparent, .05f, Tween.TransitionType.Cubic);
                tween.Start();
            }
            else if (eolExplosion.Frame == 11 && !levelEnded)
            {
                levelEnded = true;
                GetTree().CreateTimer(.4f).Connect("timeout", this, nameof(EndLevel));
            }
        }

        private void EndLevel()
        {
            GD.Print($"{GetTree().CurrentScene.Name} ended.");
        }
    }
}