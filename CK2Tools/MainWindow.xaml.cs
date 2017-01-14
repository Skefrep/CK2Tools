using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CK2Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var status = AppSettings.IsAppDirDefined();

            switch (status)
            {
                case AppSettings.eSettingsDirStatus.OK:
                    break;
                case AppSettings.eSettingsDirStatus.NotFound:
                    MessageBox.Show(CK2Tools.Properties.Resources.ConfigError_AppDirNotFound);
                    goto case AppSettings.eSettingsDirStatus.NotDefined;
                case AppSettings.eSettingsDirStatus.NotDefined:
                    var window = new SetAppDirSimple();
                    window.ShowDialog();

                    if (window.Result == SetAppDirSimple.ReturnValue.OK)
                        break;
                    else
                    {
                        throw new System.Configuration.ConfigurationErrorsException(CK2Tools.Properties.Resources.Init_NoCK2Folder);
                    }
            }

            InitializeComponent();
            Appli = ((App)Application.Current);
        }

        public void FillFields()
        {
            modName.Text = Appli.CurrentMod.Name;
            path.Text = Appli.CurrentMod.Path;
            userDir.Text = Appli.CurrentMod.UserDirectory;
        }


        #region Dialog events
        private void New_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "Paradox mod file|*.mod";
            saveDialog.FileName = "MyMod";
            saveDialog.Title = "New Mod File";

            saveDialog.InitialDirectory = GetModsDefaultPath(true);
            
            if (saveDialog.ShowDialog() == true)
            {
                System.IO.File.Create(saveDialog.FileName);
                Appli.CurrentMod.ModFile = saveDialog.FileName;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.Filter = "Paradox mod file|*.mod";
            openDialog.Title = "Open Mod File";

            openDialog.InitialDirectory = GetModsDefaultPath(false);

            if (openDialog.ShowDialog() == true)
            {
                Appli.CurrentMod.ModFile = openDialog.FileName;
                Appli.CurrentMod.DecodeFile(openDialog.FileName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Appli.CurrentMod.WriteFile();
        }

        private void Set_Name(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentMod.Name = modName.Text;
        }

        private void Set_Path(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentMod.Path = path.Text;
        }

        private void Set_UserDir(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentMod.UserDirectory = userDir.Text;
        }
        #endregion

        private string GetModsDefaultPath(bool createIfNotExist)
        {
            var modsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            modsPath = System.IO.Path.Combine(modsPath, "Paradox Interactive", "Crusader Kings II", "mod");

            if (createIfNotExist)
                System.IO.Directory.CreateDirectory(modsPath);

            return modsPath;
        }

        private App Appli;
    }
}
