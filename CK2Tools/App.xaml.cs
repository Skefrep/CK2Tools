using System.Windows;

namespace CK2Tools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base() {
            CurrentMod = new Mod();

            var status = AppSettings.IsAppDirDefined();
            
            switch (status)
            {
                case AppSettings.eSettingsDirStatus.OK:
                    return;
                case AppSettings.eSettingsDirStatus.NotFound:
                    MessageBox.Show(CK2Tools.Properties.Resources.ConfigError_AppDirNotFound);
                    goto case AppSettings.eSettingsDirStatus.NotDefined;
                case AppSettings.eSettingsDirStatus.NotDefined:
                    var window = new SetAppDirSimple();
                    window.ShowDialog();

                    if (window.Result == SetAppDirSimple.ReturnValue.OK)
                        return;
                    break;
            }
        }

        public Mod CurrentMod;
    }
}
