using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkladMe.View
{
    public partial class Authorization : UserControl
    {
        private RoutedEventHandler _loadedHandler;
        public Authorization()
        {
            InitializeComponent();
            _loadedHandler = (s, e) =>
            {
                LoginButton.IsEnabled = !string.IsNullOrWhiteSpace(KeyTextBox.Text);
                Loaded -= _loadedHandler;
            };
            Loaded += _loadedHandler;
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText(Hardware.Text);
        }

        private void KeyTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LoginButton.IsEnabled = !string.IsNullOrWhiteSpace(KeyTextBox.Text);
        }

        private void KeyTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                LoginButton.Focus();
                LoginButton.Command.Execute(null);

            }
        }
    }
}
