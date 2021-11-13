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
    }
}