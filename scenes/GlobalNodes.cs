using Godot;

namespace Inversion
{
    public class GlobalNodes : Node
    {
        public static GlobalNodes Singleton => singleton;
        private static GlobalNodes singleton;

        private AudioStreamPlayer bzztPlayer;
        private AudioStreamPlayer uiClickPlayer;

        public override void _Ready()
        {
            bzztPlayer = GetNode<AudioStreamPlayer>("BzztPlayer");
            uiClickPlayer = GetNode<AudioStreamPlayer>("UIClickPlayer");

            singleton = this;

            Globals.InitDiscord();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (Globals.CanRunDiscord)
            {
                try
                {
                    Globals.discord.RunCallbacks();
                }
                catch (Discord.ResultException e)
                {
                    GD.PrintErr($"Discord error: {e.Message} // Has discord been closed?");

                    Globals.CanRunDiscord = false;
                }
            }
        }

        public override void _Input(InputEvent evt)
        {
            if (evt is InputEventKey ek && !ek.Pressed && (ek.Scancode == (uint)KeyList.F11 || (ek.Scancode == (uint)KeyList.Enter && ek.Alt)))
            {
                OS.WindowFullscreen = !OS.WindowFullscreen;
            }
        }

        public void MakeBzztSound()
        {
            if (!bzztPlayer.Playing)
            {
                bzztPlayer.Play();
            }
        }

        public void UIClick()
        {
            uiClickPlayer.Play();
        }
    }
}