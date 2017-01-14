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
        }

        public Mod CurrentMod;
    }
}
