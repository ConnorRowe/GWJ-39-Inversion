using Godot;

namespace Inversion
{
    public class PoweredDoor : LigntningPowered, IWirePowered
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

        protected override void LightningPower()
        {
            base.LightningPower();

            PowerChanged();
        }

        protected override void LightningUnPower()
        {
            base.LightningUnPower();

            PowerChanged();
        }

        public void WirePower()
        {
            isWirePowered = true;

            PowerChanged();
        }

        public void WireUnPower()
        {
            isWirePowered = false;

            PowerChanged();
        }

        private void PowerChanged()
        {
            if (IsPowered())
            {
                doorCollision.Disabled = true;

                tween.InterpolateProperty(doorSprite, "scale", doorSprite.Scale, new Vector2(1, 0), .25f);
                tween.Start();
            }
            else
            {
                doorCollision.Disabled = false;
                tween.InterpolateProperty(doorSprite, "scale", doorSprite.Scale, new Vector2(1, 2), .25f);
                tween.Start();
            }
        }

        public override bool IsPowered()
        {
            return base.IsPowered() || isWirePowered;
        }
    }
}