using Godot;

namespace Inversion
{
    public class Level12 : BaseLevel
    {
        private Lighting.LightingHandler lightingHandler;

        public override void _Ready()
        {
            base._Ready();

            lightingHandler = GetNode<Lighting.LightingHandler>("LightingHandler");

            lightingHandler.RegisterLight(Player.GetNode<Node2D>("AnimatedSprite/Light"), Vector2.Zero);
            lightingHandler.RegisterLight(GetNode<Node2D>("GoalPlatform1/Arrow"), new Vector2(0, -2));
            lightingHandler.RegisterLight(Player.InversionOrb.BaseSprite, Vector2.Zero);
            lightingHandler.RegisterLight(GetNode<Node2D>("Squirter/FireStuff/FireSprite"), new Vector2(0, -24), Lighting.LightType.Big);
            lightingHandler.RegisterLight(GetNode<Node2D>("Squirter2/FireStuff/FireSprite"), new Vector2(0, -24), Lighting.LightType.Big);
            lightingHandler.RegisterLight(GetNode<Node2D>("Squirter3/FireStuff/FireSprite"), new Vector2(0, -24), Lighting.LightType.Big);
            lightingHandler.RegisterLight(GetNode<Node2D>("Squirter4/FireStuff/FireSprite"), new Vector2(0, -24), Lighting.LightType.Big);
            lightingHandler.RegisterLight(GetNode<Node2D>("Squirter5/FireStuff/FireSprite"), new Vector2(0, -24), Lighting.LightType.Big);
            lightingHandler.RegisterLight(GetNode<Particles2D>("BatteryOrGround/Sparks"), Vector2.Zero, Lighting.LightType.Small);
            lightingHandler.RegisterLight(GetNode<Particles2D>("PoweredDoor/PowerTerminal/Sparks"), Vector2.Zero, Lighting.LightType.Small);
            lightingHandler.RegisterLight(GetNode<Particles2D>("PowerTerminal/Sparks"), Vector2.Zero, Lighting.LightType.Small);

            GetNode<Node2D>("UILayer/LightingTexture").Visible = true;

            GetNode<Transition>("Transition").BGColour = Colors.Black;
        }
    }
}