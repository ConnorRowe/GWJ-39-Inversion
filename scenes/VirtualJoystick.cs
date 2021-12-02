using Godot;

namespace Inversion
{
    public class VirtualJoystick : Node2D
    {
        [Signal]
        public delegate void Pressed();
        [Signal]
        public delegate void Released();

        private static readonly Color opaque = new Color(1f, 1f, 1f, .75f);
        private static readonly Color transparent = new Color(1f, 1f, 1f, .27f);

        private const float maxDist = 30f;

        [Export]
        private Vector2 XRange;

        public bool IsActive { get; set; } = false;
        private int lastTouch = -1;
        private Vector2 touchPos = Vector2.Zero;

        private Sprite background;
        private Sprite foreground;
        private Tween tween;

        public override void _Ready()
        {
            base._Ready();

            background = GetNode<Sprite>("BG");
            foreground = GetNode<Sprite>("BG/FG");
            tween = GetNode<Tween>("Tween");
        }

        public override void _Input(InputEvent evt)
        {
            if (evt is InputEventScreenTouch est)
            {
                if (lastTouch == -1 && est.Pressed && est.Position.x > XRange.x && est.Position.x <= XRange.y)
                {
                    lastTouch = est.Index;

                    touchPos = est.Position;

                    Position = touchPos;

                    FadeIn();
                    EmitSignal(nameof(Pressed));
                }
                else if (lastTouch == est.Index && !est.Pressed)
                {
                    lastTouch = -1;

                    FadeOut();
                    EmitSignal(nameof(Released));
                }
            }
            else if (evt is InputEventScreenDrag esd)
            {
                if (esd.Index == lastTouch)
                {
                    touchPos = esd.Position;
                }
            }
        }

        public override void _Process(float delta)
        {
            if (lastTouch > -1)
            {
                float touchDist = Position.DistanceTo(touchPos);
                var touchDir = Position.DirectionTo(touchPos);

                foreground.Position = touchDir * Mathf.Min(touchDist, maxDist);
            }
            else
            {
                foreground.Position = foreground.Position.LinearInterpolate(Vector2.Zero, delta * 5f);
            }
        }

        private void FadeIn()
        {
            tween.InterpolateProperty(background, "modulate", background.Modulate, opaque, .2f, Tween.TransitionType.Cubic);
            tween.Start();
        }

        private void FadeOut()
        {
            tween.InterpolateProperty(background, "modulate", background.Modulate, transparent, .2f, Tween.TransitionType.Cubic);
            tween.Start();
        }

        public Vector2 GetAxis()
        {
            if (lastTouch > -1)
            {
                return foreground.Position / maxDist;
            }
            else
            {
                return Vector2.Zero;
            }
        }
    }
}