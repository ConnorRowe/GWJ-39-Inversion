using Godot;

namespace Inversion
{
    public static class Globals
    {
        public static RandomNumberGenerator RNG = new RandomNumberGenerator();
        public static int LevelDeathCount { get; set; } = 0;

        static Globals()
        {
            RNG.Randomize();
        }
    }
}