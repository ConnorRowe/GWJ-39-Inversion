using Godot;

namespace Inversion
{
    public static class Globals
    {
        public static RandomNumberGenerator RNG = new RandomNumberGenerator();

        static Globals()
        {
            RNG.Randomize();
        }
    }
}