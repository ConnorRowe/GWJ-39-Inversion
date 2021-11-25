using Godot;

namespace Inversion
{
    public class PlatformSpecific : Node
    {
        [Export]
        private bool IsMobileOnly;

        public override void _Ready()
        {
            if (IsMobileOnly != Globals.HasTouchscreen)
            {
                GetParent().QueueFree();
            }
        }
    }
}