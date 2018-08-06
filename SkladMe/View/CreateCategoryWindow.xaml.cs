using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SkladMe.ViewModel;

namespace SkladMe.Controls
{
    public partial class CreateCategoryWindow
    {
        public CreateCategoryWindow()
        {
            InitializeComponent();
            DataContext = this;
            ResizeMode = ResizeMode.NoResize;
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ButtonOk.IsEnabled = false;

            TextBox.CaretIndex = TextBox.Text.Length;
            TextBox.ScrollToEnd();
            TextBox.Focus();
        }

        public string CategoryName { get; set; }
        public ObservableCollection<CategoryVM> Categories { get; set; }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ButtonOk.IsEnabled = !string.IsNullOrWhiteSpace(CategoryName);
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (ButtonOk.IsEnabled && e.Key == Key.Enter)
            {
                OK_Click(null, null);
                e.Handled = true;
            }
        }
    }
}