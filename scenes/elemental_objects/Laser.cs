using Godot;

namespace Inversion
{
    public class Laser : LigntningPowered
    {
        private const float LaserLength = 9999f;
        private static readonly Vector2 laserOffset = new Vector2(-8, 0);
        private Vector2[] laserLine = new Vector2[2] { Vector2.Zero, Vector2.Zero };
        private Godot.Collections.Array exclude = new Godot.Collections.Array();
        private Particles2D particles;
        private ParticlesMaterial particlesMaterial;

        public override void _Ready()
        {
            exclude.Add(GetNode("StaticBody2D"));
            particles = GetNode<Particles2D>("Particles2D");
            particlesMaterial = (ParticlesMaterial)particles.ProcessMaterial;

            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            // particles.Rotation = -Rotation;

            if (!IsPowered())
                return;

            // Trace laser line

            var direction = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
            var spaceState = GetWorld2d().DirectSpaceState;
            var start = GlobalPosition + (direction * 4);
            var end = start + (direction * LaserLength);
            var trace = spaceState.IntersectRay(start, end, exclude, collisionLayer: 1);

            if (trace.Keys.Count > 0)
            {
                end = (Vector2)trace["position"];
                var dir = ((Vector2)trace["normal"]).Rotated(Mathf.Pi);
                particlesMaterial.Direction = new Vector3(dir.y, dir.x, 0f);

                if (trace["collider"] is IDamageable damageable)
                {
                    damageable.TakeDamage();
                }
                else if (((Node)trace["collider"]).Owner is IDamageable damageableOwner)
                {
                    damageableOwner.TakeDamage();
                }
            }

            if (start != laserLine[0] || end != laserLine[1])
            {
                laserLine[0] = start;
                laserLine[1] = end;
                particles.Position = ToLocal(end);
                Update();
            }
        }

        public override void _Draw()
        {
            if (IsPowered())
                DrawLine(ToLocal(laserLine[0]), ToLocal(laserLine[1]), Colors.SkyBlue, 2.5f);
        }

        protected override void LightningPower()
        {
            base.LightningPower();

			particles.Emitting = true;

            Update();
        }

        protected override void LightningUnPower()
        {
            base.LightningUnPower();

			particles.Emitting = false;

            Update();
        }
    }
}