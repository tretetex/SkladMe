using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SkDAL;
using SkDAL.Model;
using SkladMe.ViewModel;

namespace SkladMe.Controls
{
    /// <summary>
    /// Логика взаимодействия для ColorNotes.xaml
    /// </summary>
    public partial class ColorNotes
    {
        private List<ColorVM> _removedItems;
        public ObservableCollection<ColorVM> ChangedCollection { get; set; }

        public ObservableCollection<ColorVM> ItemsSource
        {
            get { return (ObservableCollection<ColorVM>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ColorVM>), typeof(ColorNotes));

        public ColorNotes()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var color = new Color() {Title = "Без названия", Value = "#000000"};
            ItemsSource.Add(new ColorVM(color));
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var removeItem = lbColors.SelectedItem as ColorVM;
            if (removeItem == null) return;
            _removedItems.Add((ColorVM)removeItem.Clone());
            var index = lbColors.SelectedIndex;
            ItemsSource.Remove(removeItem);
            if (index <= lbColors.Items.Count - 1)
            {
                lbColors.SelectedIndex = index;
            }
            else
            {
                lbColors.SelectedIndex = lbColors.Items.Count - 1;
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            if (ItemsSource != null && ChangedCollection == null)
            {
                ChangedCollection = new ObservableCollection<ColorVM>();
                _removedItems = new List<ColorVM>();
                foreach (ColorVM colorVm in ItemsSource.Select(c => c.Clone()))
                {
                    ChangedCollection.Add(colorVm);
                }
            }
            base.OnActivated(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ItemsSource.Clear();
            foreach (var colorVm in ChangedCollection)
            {
                ItemsSource.Add(colorVm);
            }
            Close();
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var tasks = new List<Task>();
            foreach (var removedItem in _removedItems)
            {
                tasks.Add(Db.Colors.DeleteAsync(removedItem.Model));
            }

            foreach (var colorVm in ItemsSource)
            {
                tasks.Add(Db.Colors.AddOrUpdateAsync(colorVm.Model));
            }

            Close();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            ChangedCollection = null;
            _removedItems = null;
        }
    }
}