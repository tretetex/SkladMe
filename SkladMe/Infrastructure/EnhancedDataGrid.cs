namespace SkladMe.Infrastructure
{
    /*
The below example code is offered "as-is" with no warrantees of any kind. Use at your own risk.
*/
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;

    namespace EnhancedwDataGridExample
    {
        class EnhancedDataGrid : DataGrid
        {
            public event EventHandler<ObservableCollection<ColumnInfo>> ColumnSizeChangedEvent ;

            private bool inWidthChange = false;
            private bool updatingColumnInfo = false;
            public static readonly DependencyProperty ColumnInfoProperty = DependencyProperty.Register("ColumnInfo",
                    typeof(ObservableCollection<ColumnInfo>), typeof(EnhancedDataGrid),
                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColumnInfoChangedCallback)
                );

            private static void ColumnInfoChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            {
                var grid = (EnhancedDataGrid)dependencyObject;
                if (!grid.updatingColumnInfo) { grid.ColumnInfoChanged(); }
            }

            protected override void OnInitialized(EventArgs e)
            {
                this.Style = new Style(GetType(), this.FindResource(typeof(System.Windows.Controls.DataGrid)) as Style);
                void SortDirectionChangedHandler(object sender, EventArgs x) => UpdateColumnInfo();
                void WidthPropertyChangedHandler(object sender, EventArgs x) => inWidthChange = true;
                var sortDirectionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.SortDirectionProperty, typeof(DataGridColumn));
                var widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));

                Loaded += (sender, x) =>
                {
                    foreach (var column in Columns)
                    {
                        sortDirectionPropertyDescriptor.AddValueChanged(column, SortDirectionChangedHandler);
                        widthPropertyDescriptor.AddValueChanged(column, WidthPropertyChangedHandler);
                    }
                };
                Unloaded += (sender, x) =>
                {
                    foreach (var column in Columns)
                    {
                        sortDirectionPropertyDescriptor.RemoveValueChanged(column, SortDirectionChangedHandler);
                        widthPropertyDescriptor.RemoveValueChanged(column, WidthPropertyChangedHandler);
                    }
                };

                base.OnInitialized(e);
            }
            public ObservableCollection<ColumnInfo> ColumnInfo
            {
                get { return (ObservableCollection<ColumnInfo>)GetValue(ColumnInfoProperty); }
                set { SetValue(ColumnInfoProperty, value); }
            }

            private void UpdateColumnInfo()
            {
                updatingColumnInfo = true;
                ColumnInfo = new ObservableCollection<ColumnInfo>(Columns.Select((x) => new ColumnInfo(x)));
                OnColumnSizeChangedEvent(ColumnInfo);
                updatingColumnInfo = false;
            }

            protected override void OnColumnReordered(DataGridColumnEventArgs e)
            {
                UpdateColumnInfo();
                base.OnColumnReordered(e);
            }
            protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
            {
                if (inWidthChange)
                {
                    inWidthChange = false;
                    UpdateColumnInfo();
                }
                base.OnPreviewMouseLeftButtonUp(e);
            }
            private void ColumnInfoChanged()
            {
                Items.SortDescriptions.Clear();
                foreach (var column in ColumnInfo)
                {
                    var realColumn = Columns.FirstOrDefault(x => column.Header.Equals(x.Header));
                    if (realColumn == null) { continue; }
                    column.Apply(realColumn, Columns.Count, Items.SortDescriptions);
                }
            }

            protected virtual void OnColumnSizeChangedEvent(ObservableCollection<ColumnInfo> e)
            {
                ColumnSizeChangedEvent?.Invoke(this, e);
            }
        }
        public struct ColumnInfo
        {
            public ColumnInfo(DataGridColumn column)
            {
                Header = column.Header;
                PropertyPath = ((Binding)((DataGridBoundColumn)column).Binding).Path.Path;
                WidthValue = column.Width.DisplayValue;
                WidthType = column.Width.UnitType;
                SortDirection = column.SortDirection;
                DisplayIndex = column.DisplayIndex;
            }
            public void Apply(DataGridColumn column, int gridColumnCount, SortDescriptionCollection sortDescriptions)
            {
                column.Width = new DataGridLength(WidthValue, WidthType);
                column.SortDirection = SortDirection;
                if (SortDirection != null)
                {
                    sortDescriptions.Add(new SortDescription(PropertyPath, SortDirection.Value));
                }
                if (column.DisplayIndex != DisplayIndex)
                {
                    var maxIndex = (gridColumnCount == 0) ? 0 : gridColumnCount - 1;
                    column.DisplayIndex = (DisplayIndex <= maxIndex) ? DisplayIndex : maxIndex;
                }
            }
            public object Header;
            public string PropertyPath;
            public ListSortDirection? SortDirection;
            public int DisplayIndex;
            public double WidthValue;
            public DataGridLengthUnitType WidthType;
        }
    }
}