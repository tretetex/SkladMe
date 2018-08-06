using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace SkladMe.View
{
    /// <summary>
    /// Логика взаимодействия для NoticeWindow.xaml
    /// </summary>
    public partial class NoticeWindow
    {
        public BitmapImage Image { get; set; }
        public string Link { get; set; }

        public NoticeWindow()
        {
            InitializeComponent();
            DataContext = this;
            Topmost = true;
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Link))
            {
                Process.Start(Link);
            }
        }
    }
}
