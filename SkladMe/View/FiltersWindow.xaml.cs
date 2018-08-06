using System.Windows;

namespace SkladMe.Controls
{
    public partial class FiltersWindow
    {
        public bool VisibileIsLocalSearch
        {
            get { return (bool) GetValue(VisibileIsLocalSearchProperty); }
            set { SetValue(VisibileIsLocalSearchProperty, value); }
        }

        public static readonly DependencyProperty VisibileIsLocalSearchProperty =
            DependencyProperty.Register("VisibileIsLocalSearch", typeof(bool), typeof(FiltersWindow),
                new PropertyMetadata(false));


        public FiltersWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}