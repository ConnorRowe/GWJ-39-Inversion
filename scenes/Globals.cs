using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public static class Globals
    {
        public static RandomNumberGenerator RNG = new RandomNumberGenerator();
        public static int LevelDeathCount { get; set; } = 0;
        public static List<PackedScene> AllLevels { get; set; } = new List<PackedScene>();
        public static int CurrentLevel { get; set; } = 0;

        static Globals()
        {
            RNG.Randomize();

            LoadFromDirectory<PackedScene>("res://scenes/levels/", AllLevels, ".tscn");
        }

        public static void LoadFromDirectory<T>(string fileDirectory, HashSet<T> objectSet, string targetFileExtension = ".png") where T : Godot.Object
        {
            Directory directory = new Directory();
            directory.Open(fileDirectory);
            directory.ListDirBegin(skipNavigational: true);

            string file = directory.GetNext();
            int extLen = targetFileExtension.Length;
            do
            {
                // check extension
                if (file.Length - 1 >= targetFileExtension.Length && file.Substring(file.Length - extLen) == targetFileExtension)
                {
                    objectSet.Add(GD.Load<T>(fileDirectory + file));
                    GD.Print($"Loaded {fileDirectory}{file}");
                }

                file = directory.GetNext();
            } while (!file.Empty());
        }

        public static void LoadFromDirectory<T>(string fileDirectory, List<T> objectList, string targetFileExtension = ".png") where T : Godot.Object
        {
            HashSet<T> objects = new HashSet<T>();
            LoadFromDirectory<T>(fileDirectory, objects, targetFileExtension);
            foreach (T o in objects)
            {
                objectList.Add(o);
            }
        }
    }
}