using System.Windows;
using System.Windows.Controls;

namespace SkladMe.View
{

    public partial class CancellationPopup : UserControl
    {
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(CancellationPopup));


        public CancellationPopup()
        {
            InitializeComponent();
        }
    }
}
