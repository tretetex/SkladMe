using System.Windows;
using System.Windows.Controls;

namespace SkladMe.Controls
{
    /// <summary>
    /// Логика взаимодействия для RangeTextbox.xaml
    /// </summary>
    public partial class RangeTextbox : UserControl
    {
        public RangeTextbox()
        {
            InitializeComponent();
        }
        
        public string TextFrom
        {
            get { return (string)GetValue(TextFromProperty); }
            set { SetValue(TextFromProperty, value); }
        }

        public static readonly DependencyProperty TextFromProperty =
            DependencyProperty.Register("TextFrom", typeof(string), typeof(RangeTextbox), new PropertyMetadata(null));
        
        public string TextTo
        {
            get { return (string)GetValue(TextToProperty); }
            set { SetValue(TextToProperty, value); }
        }

        public static readonly DependencyProperty TextToProperty =
            DependencyProperty.Register("TextTo", typeof(string), typeof(RangeTextbox), new PropertyMetadata(null));

        public double TextboxWidth
        {
            get { return (double)GetValue(TextboxWidthProperty); }
            set { SetValue(TextboxWidthProperty, value); }
        }

        public static readonly DependencyProperty TextboxWidthProperty =
            DependencyProperty.Register("TextboxWidth", typeof(double), typeof(RangeTextbox), new PropertyMetadata(70.0));

        public double TextboxHeight
        {
            get { return (double)GetValue(TextboxHeightProperty); }
            set { SetValue(TextboxHeightProperty, value); }
        }

        public static readonly DependencyProperty TextboxHeightProperty =
            DependencyProperty.Register("TextboxHeight", typeof(double), typeof(RangeTextbox), new PropertyMetadata(25.0));


        public string HintFrom
        {
            get { return (string)GetValue(HintFromProperty); }
            set { SetValue(HintFromProperty, value); }
        }

        public static readonly DependencyProperty HintFromProperty =
            DependencyProperty.Register("HintFrom", typeof(string), typeof(RangeTextbox), new PropertyMetadata("От"));


        public string HintTo
        {
            get { return (string)GetValue(HintToProperty); }
            set { SetValue(HintToProperty, value); }
        }

        public static readonly DependencyProperty HintToProperty =
            DependencyProperty.Register("HintTo", typeof(string), typeof(RangeTextbox), new PropertyMetadata("До"));

    }
}
