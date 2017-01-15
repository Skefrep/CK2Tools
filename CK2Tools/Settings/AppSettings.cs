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

        public static System.Collections.Generic.List<KnownMod> LastMods
        {
            get
            {
            }
            set
            {
            }
        }
    }
}
