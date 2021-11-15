using Godot;

namespace Inversion
{
    public enum Element
    {
        Fire,
        Water,
        Earth,
        Lightning,
    }

    public static class ElementExtensions
    {
        public static Element Invert(this Element e)
        {
            switch (e)
            {
                case Element.Fire:
                    return Element.Water;
                case Element.Water:
                    return Element.Fire;
                case Element.Earth:
                    return Element.Lightning;
                case Element.Lightning:
                    return Element.Earth;
            }

            return Element.Fire;
        }

        public static string ToString(this Element e)
        {
            switch (e)
            {
                case Element.Fire:
                    return "Fire";
                case Element.Water:
                    return "Water";
                case Element.Earth:
                    return "Earth";
                case Element.Lightning:
                    return "Lightning";
            }

            return "error";
        }

        private static readonly Color FireColour = new Color("e64539");
        private static readonly Color WaterColour = new Color("4fa4b8");
        private static readonly Color EarthColour = new Color("2f5753");
        private static readonly Color LightningColour = new Color("ffee83");

        public static Color GetColour(this Element e)
        {
            switch (e)
            {
                case Element.Fire:
                    return FireColour;
                case Element.Water:
                    return WaterColour;
                case Element.Earth:
                    return EarthColour;
                case Element.Lightning:
                    return LightningColour;
            }

            return Godot.Colors.Black;
        }
    }
}