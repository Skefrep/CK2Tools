using Microsoft.Win32;
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
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;

namespace CK2Tools
{
    /// <summary>
    /// Interaction logic for SetAppDirSimple.xaml
    /// </summary>
    public partial class SetAppDirSimple : Window
    {
        public enum ReturnValue { OK, Cancel };

        public ReturnValue Result { get; set; }

        public SetAppDirSimple()
        {
            InitializeComponent();
            SetPathBoxValue();
        }

        /// <summary>
        /// Fill the textbox with the defined value or with a default.
        /// </summary>
        private void SetPathBoxValue()
        {
            if (!String.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["ApplicationDir"]))
            {
                pathBox.Text = System.Configuration.ConfigurationManager.AppSettings["ApplicationDir"];
                return;
            }
            pathBox.Text = TryCommonAppPaths();
        }

        /// <summary>
        /// Test if theres a CK2 folder set in the registry or in the steam libraries
        /// </summary>
        /// <returns>The path to CK2 if found. Empty String otherwise.</returns>
        private String TryCommonAppPaths()
        {
            String path;

            // Check if there's a paradox key to CK2
            if (TryCKInstallerKey(out path))
                return path;

            // Look in the steam library
            if (TrySteamInstallDirs(out path))
                return path;

            return "";
        }

        /// <summary>
        /// Checks if there's a registry key containing the CK2 folder, as created by the official installer.
        /// </summary>
        /// <param name="path">Output parameter where the registry key will be stored. Empty String if not found.</param>
        /// <returns>True if key has been found, false otherwise</returns>
        private bool TryCKInstallerKey(out String path)
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"SOFTWARE\WOW6432Node\Paradox Interactive\CrusaderKingsII");

            if (regKey != null)
            {
                path = regKey.GetValue("Path").ToString();
                if (CheckValidCK2Folder(path))
                    return true;
            }
            path = "";
            return false;
        }

        /// <summary>
        /// Checks the 
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if path to CK2 has been found, false otherwise</returns>
        private bool TrySteamInstallDirs(out String path)
        {
            // First, try default steam library
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.OpenSubKey(@"Software\Valve\Steam");

            if (regKey != null)
            {
                String steamPath = regKey.GetValue("SteamPath").ToString();
                path = System.IO.Path.Combine(steamPath, "steamapps", "common", "Crusader Kings II");
                if (CheckValidCK2Folder(path))
                    return true;

                // try the configured library
                var rgx = new Regex(@"^\s*""BaseInstallFolder_1""\s*""(.*)""\s*$");
                using (var reader = new StreamReader(System.IO.Path.Combine(steamPath, "config", "config.vdf")))
                {
                    do
                    {
                        var line = reader.ReadLine();
                        var match = rgx.Match(line);
                        if (match.Success)
                        {
                            path = match.Groups[1].ToString();
                            if (CheckValidCK2Folder(path))
                                return true;
                        }
                    } while (!reader.EndOfStream);
                }
            }
            path = "";
            return false;
        }

        /// <summary>
        /// Checks if the directory given in path contains a CK2game.exe file.
        /// </summary>
        /// <param name="path">Path to the directory to check</param>
        /// <returns>true if file present, false otherwise.</returns>
        private bool CheckValidCK2Folder(String path)
        {
            return File.Exists(System.IO.Path.Combine(path, "CK2game.exe"));
        }

        #region Dialog Events
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidCK2Folder(pathBox.Text))
            {
                MessageBox.Show(this, Properties.Resources.SetAppDirSimple_CK2NotValid);    // Invalid CK2 folder.
                return;
            }

            UserSettings.ApplicationDir = pathBox.Text;
            Result = ReturnValue.OK;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = ReturnValue.Cancel;
            Close();
        }
        #endregion
    }
}
