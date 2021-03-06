﻿using System;
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
            var status = UserSettings.IsAppDirDefined();

            switch (status)
            {
                case UserSettings.eSettingsDirStatus.OK:
                    break;
                case UserSettings.eSettingsDirStatus.NotFound:
                    MessageBox.Show(CK2Tools.Properties.Resources.ConfigError_AppDirNotFound);
                    goto case UserSettings.eSettingsDirStatus.NotDefined;
                case UserSettings.eSettingsDirStatus.NotDefined:
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
            LoadRecentMenu();
            Appli = ((App)Application.Current);
            RepPathNum = 0;

            modName.TextChanged += NameChanged;
            path.TextChanged += PathChanged;
            userDir.TextChanged += UserDirChanged;
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
                var stream = System.IO.File.Create(saveDialog.FileName);
                stream.Close();
                stream.Dispose();
                Appli.CurrentMod.ModFile = saveDialog.FileName;
                LoadRecentMenu();
            }

            RefreshWindowBody();
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
                LoadRecentMenu();
            }

            RefreshWindowBody();
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

        private void btnAddRepPath_Click(object sender, RoutedEventArgs e)
        {
            CreateReplacePathTextBox();
        }

        private void NameChanged(object sender, RoutedEventArgs e)
        {
            Appli.CurrentMod.Name = modName.Text;
        }

        private void PathChanged(object sender, RoutedEventArgs e)
        {
            Appli.CurrentMod.Path = path.Text;
        }

        private void UserDirChanged(object sender, RoutedEventArgs e)
        {
            Appli.CurrentMod.UserDirectory = userDir.Text;
        }

        private void ReplacePathChanged(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            int id;
            int.TryParse(textBox.Name.Split('_')[1], out id);
            Appli.CurrentMod.ReplacePath[id] = textBox.Text;
        }
        #endregion

        #region Local Methods
        private string GetModsDefaultPath(bool createIfNotExist)
        {
            var modsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            modsPath = System.IO.Path.Combine(modsPath, "Paradox Interactive", "Crusader Kings II", "mod");

            if (createIfNotExist)
                System.IO.Directory.CreateDirectory(modsPath);

            return modsPath;
        }

        private void CreateReplacePathTextBox(string value = "")
        {
            var textBox = new TextBox();
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.Margin = new Thickness(0, 5, 0, 0);
            textBox.Height = 28;
            textBox.Width = 200;
            textBox.Text = value;
            textBox.Name = "replacePath_" + RepPathNum++;
            textBox.TextChanged += ReplacePathChanged;
            pannelRepPath.Children.Insert(pannelRepPath.Children.Count - 1, textBox);

            if (Appli.CurrentMod.ReplacePath == null)
                Appli.CurrentMod.ReplacePath = new List<string>();
            if (Appli.CurrentMod.ReplacePath.Count < RepPathNum)
                Appli.CurrentMod.ReplacePath.Add("");
        }

        private void LoadRecentMenu()
        {
            if (UserSettings.LastMods.Count == 0)
            {
                var item = new MenuItem();
                item.Header = Properties.Resources.Global_Empty;
                item.IsEnabled = false;
                recentModsMenu.Items.Add(item);
                return;
            }

            recentModsMenu.Items.Clear();
            foreach (var entry in UserSettings.LastMods)
            {
                var item = new MenuItem();
                item.Header = entry.name;
                item.ToolTip = entry.path;
                item.Click += (s, e) =>
                {
                    Appli.CurrentMod.ModFile = entry.path;
                    RefreshWindowBody();
                };
                recentModsMenu.Items.Add(item);
            }
        }

        public void FillFields()
        {
            modName.Text = Appli.CurrentMod.Name;
            path.Text = Appli.CurrentMod.Path;
            userDir.Text = Appli.CurrentMod.UserDirectory;
            if (Appli.CurrentMod.ReplacePath != null)
            {
                foreach (var item in Appli.CurrentMod.ReplacePath)
                {
                    CreateReplacePathTextBox(item);
                }
            }
        }

        public void RefreshWindowBody()
        {
            Body.IsEnabled = !string.IsNullOrWhiteSpace(Appli.CurrentMod.ModFile);
        }
        #endregion // Local Methods

        private App Appli;
        private int RepPathNum;
    }
}
