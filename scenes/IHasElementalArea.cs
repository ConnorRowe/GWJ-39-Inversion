namespace Inversion
{
    public interface IHasElementalArea
    {
        Element GetAreaElement();
        bool IsDisabled();
        bool IsSource();
        IHasElementalArea GetSource();
    }
}