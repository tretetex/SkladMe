using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkladMe.View
{
    public partial class AddProductFromUrlWindow
    {
        public string Url { get; set; }
        public AddProductFromUrlWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            OK.IsEnabled = false;

            TextBox.CaretIndex = TextBox.Text.Length;
            TextBox.ScrollToEnd();
            TextBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Url = TextBox.Text.Trim();  
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) 
        {   
            DialogResult = false;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (OK.IsEnabled && e.Key == Key.Enter)
            {
                Ok_Click(null, null);
                e.Handled = true;
            }
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OK.IsEnabled = !string.IsNullOrWhiteSpace(TextBox.Text);
        }
    }
}
