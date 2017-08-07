using System.Windows;
using System.Windows.Controls;

namespace CK2Tools.Controls
{
    public class TokenItem : ContentControl
    {
        static TokenItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TokenItem), new FrameworkPropertyMetadata(typeof(TokenItem)));
        }

        public static readonly DependencyProperty TokenKeyProperty = DependencyProperty.Register("TokenKey", typeof(string), typeof(TokenItem), new UIPropertyMetadata(null));
        public string TokenKey
        {
            get { return (string)GetValue(TokenKeyProperty); }
            set { SetValue(TokenKeyProperty, value); }
        }
    }
}
