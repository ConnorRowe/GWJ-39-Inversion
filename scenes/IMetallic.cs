namespace Inversion
{
    public interface IMetallic
    {
        void Power();
        void UnPower();
        System.Collections.Generic.HashSet<IMetallic> GetNearbyMetallics();
    }
}