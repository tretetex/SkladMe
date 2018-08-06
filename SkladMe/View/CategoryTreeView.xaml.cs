using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SkDAL;
using SkladMe.Controls.DragDropHandlers;
using SkladMe.ViewModel;
using TextBox = System.Windows.Controls.TextBox;

namespace SkladMe.Controls
{
    /// <summary>
    /// Логика взаимодействия для CategoryTreeView.xaml
    /// </summary>
    public partial class CategoryTreeView : UserControl
    {
        public CategoryTreeView()
        {
            InitializeComponent();
        }

        public CategoryDragHandler DragHandler
        {
            get { return (CategoryDragHandler) GetValue(DragHandlerProperty); }
            set { SetValue(DragHandlerProperty, value); }
        }

        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.Register("DragHandler", typeof(CategoryDragHandler), typeof(CategoryTreeView),
                new PropertyMetadata(new CategoryDragHandler()));


        public CategoryDropHandler DropHandler
        {
            get { return (CategoryDropHandler) GetValue(DropHandlerProperty); }
            set { SetValue(DropHandlerProperty, value); }
        }

        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.Register("DropHandler", typeof(CategoryDropHandler), typeof(CategoryTreeView),
                new PropertyMetadata(new CategoryDropHandler()));

        private string _oldCategoryName;

        private async void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var categoryVm = (sender as TextBox).DataContext as CategoryVM;
                if (_oldCategoryName != categoryVm.Title)
                    await Db.Categories.AddOrUpdateAsync(categoryVm.Model);

                TextBox_LostFocus(sender, e);
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Text = textBox.Text.Trim();
            textBox.Visibility = Visibility.Collapsed;
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _oldCategoryName = null;
            var textBox = sender as TextBox;
            if (textBox.Visibility == Visibility.Visible)
            {
                _oldCategoryName = (sender as TextBox).Text;
                textBox.SelectAll();
                textBox.Focus();
            }
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}