using Godot;

namespace Inversion
{
    public class PoweredDoor : Node2D, IWirePowered
    {
        private bool isWirePowered = false;

        private CollisionShape2D doorCollision;
        private Tween tween;
        private Sprite doorSprite;

        public override void _Ready()
        {
            doorCollision = GetNode<CollisionShape2D>("StaticCollision/DoorShape");
            tween = GetNode<Tween>("Tween");
            doorSprite = GetNode<Sprite>("Door");

            base._Ready();
        }

        public void WirePower()
        {
            GD.Print($"{Name} powered.");

            isWirePowered = true;

            PowerChanged();
        }

        public void WireUnPower()
        {
            GD.Print($"{Name} unpowered.");

            isWirePowered = false;

            PowerChanged();
        }

        private void PowerChanged()
        {
            if (isWirePowered)
            {
                doorCollision.Disabled = true;

                tween.InterpolateProperty(doorSprite, "region_rect", doorSprite.RegionRect, new Rect2(0, 72, 16, 0), .35f, Tween.TransitionType.Cubic);
                tween.InterpolateProperty(doorSprite, "offset", doorSprite.Offset, Vector2.Zero, .35f, Tween.TransitionType.Cubic);
                tween.Start();

                GlobalNodes.Singleton.MakeBzztSound();
            }
            else
            {
                doorCollision.Disabled = false;
                tween.InterpolateProperty(doorSprite, "region_rect", doorSprite.RegionRect, new Rect2(0, 0, 16, 72), .35f, Tween.TransitionType.Cubic);
                tween.InterpolateProperty(doorSprite, "offset", doorSprite.Offset, new Vector2(0, 36), .35f, Tween.TransitionType.Cubic);
                tween.Start();
            }
        }
    }
}