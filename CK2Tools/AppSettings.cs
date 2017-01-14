using System.Configuration;

namespace CK2Tools
{
    public static class AppSettings
    {
        public enum eSettingsDirStatus {
            OK = 0,
            NotFound = 1,
            NotDefined = 2
        }

        public static eSettingsDirStatus IsAppDirDefined()
        {
            string appDir = ConfigurationManager.AppSettings["ApplicationDir"];

            if (string.IsNullOrWhiteSpace(appDir))
                return eSettingsDirStatus.NotDefined;

            if (System.IO.Directory.Exists(appDir))
                return eSettingsDirStatus.NotFound;
            else
                return eSettingsDirStatus.OK;
        }

        public static void SetAppDir(string AppDir)
        {
            ConfigurationManager.AppSettings["ApplicationDir"] = AppDir;
        }
    }
}
