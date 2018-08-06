using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SkladMe.Controls.DragDropHandlers;
using SkladMe.Infrastructure.EnhancedwDataGridExample;
using SkladMe.ViewModel;

namespace SkladMe.Controls
{
    /// <summary>
    /// Логика взаимодействия для MyDataGrid.xaml
    /// </summary>
    public partial class ProductsDataGrid : UserControl
    {
        public bool IsDragAvailable
        {
            get { return (bool)GetValue(IsDragAvailableProperty); }
            set { SetValue(IsDragAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsDragAvailableProperty =
            DependencyProperty.Register("IsDragAvailable", typeof(bool), typeof(ProductsDataGrid), new PropertyMetadata(true));


        public ContextMenu GridContextMenu
        {
            get { return (ContextMenu) GetValue(GridContextMenuProperty); }
            set { SetValue(GridContextMenuProperty, value); }
        }

        public static readonly DependencyProperty GridContextMenuProperty =
            DependencyProperty.Register("GridContextMenu", typeof(ContextMenu), typeof(ProductsDataGrid));


        public List<ProductVM> SelectedItems
        {
            get { return (List<ProductVM>) GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(List<ProductVM>), typeof(ProductsDataGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ObservableCollection<ProductVM> ItemsSource
        {
            get { return (ObservableCollection<ProductVM>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ProductVM>), typeof(ProductsDataGrid));


        public Style RowStyle
        {
            get { return (Style) GetValue(RowStyleProperty); }
            set { SetValue(RowStyleProperty, value); }
        }

        public static readonly DependencyProperty RowStyleProperty =
            DependencyProperty.Register("RowStyle", typeof(Style), typeof(ProductsDataGrid));



        public CategoryDragHandler DragHandler  
        {
            get { return (CategoryDragHandler)GetValue(DragHandlerProperty); }
            set { SetValue(DragHandlerProperty, value); }
        }

        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.Register("DragHandler", typeof(CategoryDragHandler), typeof(ProductsDataGrid), new PropertyMetadata(new CategoryDragHandler()));



        public ObservableCollection<ColumnInfo> ColumnsSizeCollection
        {
            get { return (ObservableCollection<ColumnInfo>)GetValue(ColumnsSizeCollectionProperty); }
            set { SetValue(ColumnsSizeCollectionProperty, value); }
        }

        public static readonly DependencyProperty ColumnsSizeCollectionProperty =
            DependencyProperty.Register("ColumnsSizeCollection", typeof(ObservableCollection<ColumnInfo>), typeof(ProductsDataGrid));

        public ProductsDataGrid()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private MenuItem _addNote;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var miAddNote = new MenuItem {Header = "Заметка"};
            miAddNote.Click += AddNote_Click;
            dg?.ContextMenu?.Items.Add(miAddNote);
            _addNote = miAddNote;
            _addNote.IsEnabled = false;
            Loaded -= UserControl_Loaded;
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            var productVms = dg.SelectedItems.Cast<ProductVM>();
            var textNotes = new TextNotes
            {
                ItemsSource = productVms,
            };
            var location = dg.ContextMenu?.PointToScreen(new Point(0, 0));
            textNotes.Left = location.Value.X;
            textNotes.Top = location.Value.Y;
            textNotes.ShowDialog();
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSelectedItems();
        }

        private void SetSelectedItems()
        {
            var array = new object[dg.SelectedItems.Count];
            dg.SelectedItems.CopyTo(array, 0);
            SelectedItems = new List<ProductVM>(array.Cast<ProductVM>());
            _addNote.IsEnabled = SelectedItems != null && SelectedItems.Count > 0;
        }

        private void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Delete || Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                var target = this;
                var routedEvent = Keyboard.KeyDownEvent;
                var visual = PresentationSource.FromVisual(target);
                
                if (visual != null)
                {
                    target.RaiseEvent(
                        new KeyEventArgs(
                                Keyboard.PrimaryDevice,
                                visual, 0, e.Key)
                            {RoutedEvent = routedEvent});

                    e.Handled = true;
                }
            }
        }

        private void Dg_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetSelectedItems();
        }

        private void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Dg_OnColumnSizeChangedEvent(object sender, ObservableCollection<ColumnInfo> e)
        {
            App.MainVM.ColumnsSizeChangedHandle(e);
        }
    }
}