using Godot;

namespace Inversion
{
    public static class SaveData
    {
        private const string savePath = "user://SaveData.ini";

        public static int MaxLevel => (int)save.GetValue("Inversion", "maxLevel");
        public static (float master, float music, float sfx) Volume => ((float)save.GetValue("Inversion", "volMaster"), (float)save.GetValue("Inversion", "volMusic"), (float)save.GetValue("Inversion", "volSFX"));
        public static bool Fullscreen => (bool)save.GetValue("Inversion", "fullscreen");
        public static ConfigFile Save => save;
        private static ConfigFile save = new ConfigFile();

        static SaveData()
        {
            GD.Print("hi");
            var e = save.Load(savePath);

            if (e == Error.Ok && save.HasSection("Inversion"))
            {
                GD.Print("Save loaded successfully.");
            }
            else
            {
                GD.PrintErr("Error loading save. Re-creating.", e);
                WriteDefaultSave();
                save.Load(savePath);
            }
        }

        private static void WriteDefaultSave()
        {
            save.Clear();
            save.SetValue("Inversion", "maxLevel", 0);
            save.SetValue("Inversion", "volMaster", .5f);
            save.SetValue("Inversion", "volMusic", 1.0f);
            save.SetValue("Inversion", "volSFX", 1.0f);
            save.SetValue("Inversion", "fullscreen", false);
            save.Save(savePath);
        }

        public static void SaveMaxLevel(int maxLevel)
        {
            save.SetValue("Inversion", "maxLevel", maxLevel);
            save.Save(savePath);
        }

        public static void SaveAudioVol(float master, float music, float sfx)
        {
            save.SetValue("Inversion", "volMaster", master);
            save.SetValue("Inversion", "volMusic", music);
            save.SetValue("Inversion", "volSFX", sfx);
            save.Save(savePath);
        }

        public static void SaveFullscreen(bool fullscreen)
        {
            save.SetValue("Inversion", "fullscreen", fullscreen);
            save.Save(savePath);
        }

        public static void ApplySavedSettings()
        {
            var vol = Volume;
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), GD.Linear2Db(vol.master));
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), GD.Linear2Db(vol.music));
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), GD.Linear2Db(vol.sfx));
            bool fs = Fullscreen;
            OS.WindowFullscreen = fs;
        }
    }
}