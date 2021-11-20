using Godot;

namespace Inversion
{
    public class LigntningPowered : Node2D
    {
        [Export]
        private NodePath reactionHandlerPath;
        [Export]
        protected bool alwaysLightningPowered = false;
        protected bool isLightningPowered = false;
        private ReactionHandler reactionHandler;

        public override void _Ready()
        {
            base._Ready();

            reactionHandler = GetNode<ReactionHandler>(reactionHandlerPath);
            reactionHandler.Connect(nameof(ReactionHandler.ElementStarted), this, nameof(ElementStarted));
            reactionHandler.Connect(nameof(ReactionHandler.ElementEnded), this, nameof(ElementEnded));

            if (alwaysLightningPowered)
            {
                LightningPower();
            }
            else
            {
                LightningUnPower();
            }
        }

        protected virtual void LightningPower()
        {
            if (isLightningPowered)
                return;

            GD.Print($"{Name} powered");

            isLightningPowered = true;
        }

        protected virtual void LightningUnPower()
        {
            if (alwaysLightningPowered)
                return;

            GD.Print($"{Name} unpowered");

            isLightningPowered = false;
        }

        private void ElementStarted(Element element, IHasElementalArea source)
        {
            if (element == Element.Lightning)
            {
                reactionHandler.LightningSource = source;

                LightningPower();
            }
        }

        private void ElementEnded(Element element)
        {
            if (element == Element.Lightning)
            {
                LightningUnPower();
            }
        }

        public virtual bool IsPowered()
        {
            return alwaysLightningPowered || isLightningPowered;
        }
    }
}