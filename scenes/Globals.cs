using Godot;
using System.Collections.Generic;

namespace Inversion
{
    public static class Globals
    {
        private const long CLIENT_ID = 913519594843562034;

        public static RandomNumberGenerator RNG = new RandomNumberGenerator();
        public static int LevelDeathCount { get; set; } = 0;
        public static List<PackedScene> AllLevels { get; set; } = new List<PackedScene>();
        public static int CurrentLevel { get; set; } = 0;
        public static bool HasTouchscreen { get; set; } = false;
        public static Discord.Discord discord { get; set; }
        public static bool CanRunDiscord { get; set; } = false;

        //Discord stuff
        private static Discord.ActivityAssets activityAssets;
        private static Discord.ActivityTimestamps timestamps;
        private static Discord.Activity baseActivity;

        static Globals()
        {
            RNG.Randomize();

            LoadFromDirectory<PackedScene>("res://scenes/levels/", AllLevels, ".tscn");
        }

        public static void InitDiscord()
        {
            if (!CanRunDiscord)
                return;
            /*
               Grab that Client ID from earlier
               Discord.CreateFlags.Default will require Discord to be running for the game to work
               If Discord is not running, it will:
               1. Close your game
               2. Open Discord
               3. Attempt to re-open your game
               Step 3 may fail when running directly from your editor
               Therefore, always keep Discord running during tests, or use Discord.CreateFlags.NoRequireDiscord
           */

            discord = new Discord.Discord(CLIENT_ID, (ulong)Discord.CreateFlags.NoRequireDiscord);

            activityAssets = new Discord.ActivityAssets()
            {
                LargeImage = "icon"
            };

            timestamps = new Discord.ActivityTimestamps()
            {
                Start = (long)OS.GetUnixTime(),
            };

            baseActivity = new Discord.Activity()
            {
                Type = Discord.ActivityType.Playing,
                ApplicationId = CLIENT_ID,
                Assets = activityAssets,
                Name = "Elemental Inversion",
                Timestamps = timestamps,
            };
        }

        public static void UpdateDiscordActivityLevel(string levelName)
        {
            if (!CanRunDiscord)
                return;

            UpdateDiscordActivityOther($"In level: {levelName}");
        }

        public static void UpdateDiscordActivityMenu()
        {
            if (!CanRunDiscord)
                return;

            UpdateDiscordActivityOther("Chilling in a menu...");
        }

        public static void UpdateDiscordActivityOther(string state)
        {
            if (!CanRunDiscord)
                return;

            timestamps.Start = (long)OS.GetUnixTime();
            baseActivity.State = state;

            discord.GetActivityManager().UpdateActivity(baseActivity, DiscordActivityUpdateCallback);
        }

        private static void DiscordActivityUpdateCallback(Discord.Result result)
        {
            GD.Print($"DiscordActivityUpdateCallback={result.ToString()}");
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