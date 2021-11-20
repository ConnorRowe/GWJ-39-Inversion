namespace Inversion
{
    public interface IInvertable
    {
        Element GetCurrentElement();
        void Invert();
        bool IsInversionDisabled();
    }
}