using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SkladMe.Resources
{
    public class BindableSelectedItemsDataGridBehavior : Behavior<DataGrid>
    {
        public IList SelectedItems
        {
            get { return (IList) GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList),
                typeof(BindableSelectedItemsDataGridBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var array = new object[AssociatedObject.SelectedItems.Count];
            AssociatedObject.SelectedItems.CopyTo(array, 0);
            SelectedItems = array;
        }
    }
}