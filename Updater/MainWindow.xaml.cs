using System.Windows;

namespace Launcher
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (sender, args) => DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}