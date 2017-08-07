using System.Windows.Input;

namespace CK2Tools.Controls
{
    public static class TokenizedTextBoxCommands
    {

        private static RoutedCommand _deleteCommand = new RoutedCommand();
        public static RoutedCommand Delete
        {
            get { return _deleteCommand; }
        }
    }
}
