using Godot;

namespace Inversion
{
    public class CrateWood : RigidBody2D, IHasElementalArea
    {
        private bool nearHeat = false;
        private bool ignited = false;
        private float ignitionTime = .5f;
        private float life = 3f;
        private Particles2D flames;
        private RigidBody2D[] chunks;

        public override void _Ready()
        {
            flames = GetNode<Particles2D>("Flames");
            GetNode("ReactionHandler").Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            GetNode("ReactionHandler").Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));
            chunks = new RigidBody2D[3] { GetNode<RigidBody2D>("BrokenChunks/Chunk1"), GetNode<RigidBody2D>("BrokenChunks/Chunk2"), GetNode<RigidBody2D>("BrokenChunks/Chunk3") };
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (nearHeat)
                ignitionTime -= delta;

            if (ignitionTime <= 0f && !ignited)
            {
                Ignite();
            }

            if (ignited)
                life -= delta;

            if (life <= 0f)
            {
                Destroy();
            }
        }

        private void ElementStarted(Element element)
        {
            if (!ignited && element == Element.Fire)
            {
                nearHeat = true;
            }
        }

        private void ElementEnded(Element element)
        {
            if (!ignited && element == Element.Fire)
            {
                nearHeat = false;
            }
        }

        private void Ignite()
        {
            ignited = true;

            flames.Emitting = true;
        }

        private void Destroy()
        {
            //make cool wood fragments

            flames.Emitting = false;
            GetNode<Sprite>("Sprite").Visible = false;
            Sleeping = true;
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;

            foreach (var chunk in chunks)
            {
                var pos = chunk.GlobalPosition;

                chunk.Mode = ModeEnum.Rigid;
                chunk.Layers = 1;
                chunk.Sleeping = false;
                chunk.GetParent().RemoveChild(chunk);
                GetTree().CurrentScene.AddChild(chunk);
                chunk.GlobalPosition = pos;
                chunk.ApplyCentralImpulse(new Vector2(Globals.RNG.RandfRange(-40, 40), Globals.RNG.RandfRange(-80, -40)));

                GetTree().CreateTimer(3f).Connect("timeout", chunk, "queue_free");
            }

            // GetTree().CreateTimer(.7f).Connect("timeout", this, "queue_free");
            QueueFree();
        }

        public Element GetAreaElement()
        {
            return Element.Fire;
        }

        public bool IsDisabled()
        {
            return !ignited;
        }
    }
}