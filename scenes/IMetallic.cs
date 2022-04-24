namespace Inversion
{
    public interface IMetallic
    {
        void Power();
        void UnPower();
        System.Collections.Generic.List<IMetallic> GetNearbyMetallics();
    }
}