using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkladMe.View
{
    public partial class KeywordInput : UserControl
    {
        public IList<string> ItemsSource
        {
            get { return (IList<string>) GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }


        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<string>), typeof(KeywordInput),
                new PropertyMetadata(null));

        public KeywordInput()
        {
            InitializeComponent();
        }

        private void AddKeyword(object sender, RoutedEventArgs e)
        {
            string keyword = TextBox.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                return;
            }

            bool isKeywordExists = ItemsSource.Any(s => s == keyword);
            if (isKeywordExists)
            {
                TextBox.Clear();
                return;
            }
            ItemsSource.Add(keyword);
            TextBox.Clear();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddKeyword(null, null);
                e.Handled = true;
            }
        }
    }
}