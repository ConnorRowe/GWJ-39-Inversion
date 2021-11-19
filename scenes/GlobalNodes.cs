using Godot;

namespace Inversion
{
    public class GlobalNodes : Node
    {
        public static GlobalNodes Singleton => singleton;
        private static GlobalNodes singleton;

        private AudioStreamPlayer bzztPlayer;

        public override void _Ready()
        {
            bzztPlayer = GetNode<AudioStreamPlayer>("BzztPlayer");

            singleton = this;
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
    }
}