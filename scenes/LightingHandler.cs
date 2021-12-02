using Godot;

namespace Inversion.Lighting
{
    public enum LightType
    {
        Normal,
        Small,
        Big
    }

    public class LightingHandler : Viewport
    {

        private LightRenderer lightRenderer;

        public override void _Ready()
        {
            lightRenderer = GetNode<LightRenderer>("LightRenderer");
        }

        public void RegisterLight(Node2D node2D, Vector2 offset, LightType lightType = LightType.Normal)
        {
            lightRenderer.RegisteredLights.Add((node2D, offset, lightType));
        }

        public void UnregisterLight(Node2D node2D, Vector2 offset, LightType lightType = LightType.Normal)
        {
            lightRenderer.RegisteredLights.Remove((node2D, offset, lightType));
        }
    }
}