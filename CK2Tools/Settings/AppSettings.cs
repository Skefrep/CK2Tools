using System.Configuration;

namespace CK2Tools
{
    public static class UserSettings
    {
        public enum eSettingsDirStatus {
            OK = 0,
            NotFound = 1,
            NotDefined = 2
        }

        public static eSettingsDirStatus IsAppDirDefined()
        {
            string appDir = Properties.Settings.Default.ApplicationDir;

            if (string.IsNullOrWhiteSpace(appDir))
                return eSettingsDirStatus.NotDefined;

            if (!System.IO.Directory.Exists(appDir))
                return eSettingsDirStatus.NotFound;
            else
                return eSettingsDirStatus.OK;
        }

        public static string ApplicationDir
        {
            get
            {
                return Properties.Settings.Default.ApplicationDir;
            }
            set
            {
                Properties.Settings.Default.ApplicationDir = value;
                Properties.Settings.Default.Save();
            }
        }

        public struct KnownMod { public int id; public string name; public string path; };

        public static System.Collections.Generic.List<KnownMod> LastMods
        {
            get
            {
                if (Properties.Settings.Default.LastMods == null)
                    Properties.Settings.Default.LastMods = new System.Collections.Specialized.StringCollection();

                if (Properties.Settings.Default.LastModsPaths == null)
                    Properties.Settings.Default.LastModsPaths = new System.Collections.Specialized.StringCollection();

                var mods = new System.Collections.Generic.List<KnownMod>();
                for (int id = 0; id < Properties.Settings.Default.LastMods.Count; id++)
                {
                    var knownMod = new KnownMod();
                    knownMod.id = id;
                    knownMod.name = Properties.Settings.Default.LastMods[id];
                    knownMod.path = Properties.Settings.Default.LastModsPaths[id];
                    mods.Add(knownMod);
                }
                return mods;
            }
            set
            {
                foreach(var mod in value)
                {
                    Properties.Settings.Default.LastMods[mod.id] = mod.name;
                    Properties.Settings.Default.LastModsPaths[mod.id] = mod.path;
                }
                Properties.Settings.Default.Save();
            }
        }

        public static void AddLastMod(string name, string path)
        {
            // If you already have opened this mod, we don't want it to appear twice
            if (Properties.Settings.Default.LastMods.Contains(name))
            {
                int index = Properties.Settings.Default.LastMods.IndexOf(name);
                if (Properties.Settings.Default.LastModsPaths[index] == path)
                {
                    Properties.Settings.Default.LastMods.RemoveAt(index);
                    Properties.Settings.Default.LastModsPaths.RemoveAt(index);
                }
            }

            Properties.Settings.Default.LastMods.Insert(0, name);
            Properties.Settings.Default.LastModsPaths.Insert(0, path);

            while (Properties.Settings.Default.LastMods.Count > 5)
            {
                Properties.Settings.Default.LastMods.RemoveAt(5);
                Properties.Settings.Default.LastModsPaths.RemoveAt(5);
            }
            Properties.Settings.Default.Save();
        }
    }
}
