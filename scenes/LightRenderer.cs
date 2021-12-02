using Godot;
using System.Collections.Generic;

namespace Inversion.Lighting
{
    public class LightRenderer : Node2D
    {
        private static readonly StreamTexture lightTex = GD.Load<StreamTexture>("res://textures/light.png");
        private static readonly StreamTexture lightSmallTex = GD.Load<StreamTexture>("res://textures/light_small.png");
        private static readonly StreamTexture lightBigTex = GD.Load<StreamTexture>("res://textures/light_big.png");
        private static readonly Rect2 fullRect = new Rect2(0, 0, 640, 360);

        public HashSet<(Node2D node, Vector2 offset, LightType lightType)> RegisteredLights => registeredLights;
        private HashSet<(Node2D node, Vector2 offset, LightType lightType)> registeredLights = new HashSet<(Node2D node, Vector2 offset, LightType lightType)>();

        private Camera2D camera;

        public override void _Process(float delta)
        {
            base._Process(delta);

            Update();
        }

        public override void _Draw()
        {
            DrawRect(fullRect, Colors.Black);

            var cameraOffset = Vector2.Zero;

            if (camera == null)
                camera = GetTree().CurrentScene.GetNode<Camera2D>("Player/Camera2D");

            if (camera == null)
                return;

            foreach (var lightData in registeredLights)
            {
                if (lightData.node.Visible != (lightData.node is Particles2D particles2D && !particles2D.Emitting))
                {
                    var offset = Mathf.IsEqualApprox(lightData.node.GlobalRotation, 0f) ? lightData.offset : lightData.offset.Rotated(lightData.node.GlobalRotation);
                    var tex = GetLightTypeTexture(lightData.lightType);
                    var scaledSize = tex.GetSize() * lightData.node.Scale;
                    var pos = new Vector2(320, 180) + (lightData.node.GlobalPosition - (camera.GetCameraScreenCenter())) - (scaledSize / 2);
                    DrawTextureRect(tex, new Rect2(pos, scaledSize), false);
                }
            }
        }

        private Texture GetLightTypeTexture(LightType lightType)
        {
            switch (lightType)
            {
                case LightType.Small:
                    return lightSmallTex;

                case LightType.Big:
                    return lightBigTex;

                case LightType.Normal:
                default:
                    return lightTex;
            }
        }
    }
}