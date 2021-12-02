using Godot;

namespace Inversion
{
    public class Transition : Node2D
    {
        [Export]
        public Color BGColour { get { return bgColour; } set { bgColour = value; bg.Color = bgColour; } }
        private Color bgColour = new Color("14182e");

        public bool Active { get; set; } = false;

        private ColorRect bg;
        private Particles2D particles;

        public override void _Ready()
        {
            base._Ready();

            particles = GetNode<Particles2D>("CanvasLayer/Particles2D");
            bg = GetNode<ColorRect>("CanvasLayer/Background");
            bg.Color = bgColour;
            bg.Visible = true;

            Position = Vector2.Zero;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (bg.Visible && !particles.Emitting)
            {
                bg.Modulate -= new Color(0f, 0f, 0f, delta * 3f);

                if (bg.Modulate.a <= 0f)
                {
                    bg.Visible = false;
                    bg.Modulate = Colors.White;
                }
            }
        }

        public async void StartTransition()
        {
            if (bg == null)
                bg = GetNode<ColorRect>("Background");

            Active = true;

            bg.Color = bgColour;
            bg.Modulate = Colors.White;

            GetViewport().RenderTargetClearMode = Viewport.ClearMode.OnlyNextFrame;

            await ToSignal(VisualServer.Singleton, "frame_post_draw");

            var img = GetViewport().GetTexture().GetData();
            img.Resize(640, 360, Image.Interpolation.Nearest);
            img.FlipY();

            var tex = new ImageTexture();
            tex.CreateFromImage(img);

            var mat = (ShaderMaterial)particles.ProcessMaterial;
            mat.SetShaderParam("sprite", tex);
            mat.SetShaderParam("bg_colour", new Color(bgColour));

            particles.Emitting = true;

            GetViewport().RenderTargetClearMode = Viewport.ClearMode.Always;

            bg.Visible = true;
        }
    }
}