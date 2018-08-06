using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SkDAL;
using SkladMe.Infrastructure;
using SkladMe.ViewModel;

namespace SkladMe.Controls
{
    public partial class AddToCategoryWindow
    {
        private ICommand _removeCategoryCommand;
        public List<ProductVM> Products { get; set; }
        public IList<CategoryVM> SelectedCategories { get; set; }

        public ICommand RemoveCategoryCommand
            =>
                _removeCategoryCommand ??
                (_removeCategoryCommand =
                    new RelayCommand(RemoveCategory,
                        () => SelectedCategories != null && SelectedCategories.Any() && SelectedCategories.All(c => !c.IsFixed && !c.IsReadonly)));

        private async void RemoveCategory()
        {
           foreach (var selectedCategory in SelectedCategories)
           {
                bool hasChilds = selectedCategory.AllProducts.Count > 0 || Db.Categories.ProductAndCategories.Any(x => x.CategoryId == selectedCategory.Model.Id);
                if (hasChilds || selectedCategory.ChildCategories.Count > 0)
                {
                    var result = await View.MessageBox.ShowAsync(
                                    $"Вы действительно хотите удалить категорию \"{selectedCategory.Title}\" вместе со всем содержимым?",
                                    "Удаление",
                                    MessageBoxButton.OKCancel, MessageBoxImage.Question).ConfigureAwait(false);
                    if (result != MessageBoxResult.OK) continue;
                }
                await AsyncHelper.ExecuteAtUI(() => selectedCategory.ParentCollection.Remove(selectedCategory))
                    .ConfigureAwait(false);
                await Db.Categories.DeleteAsync(selectedCategory.Model).ConfigureAwait(false);
            }
        }

        public AddToCategoryWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SelectedCategories = new List<CategoryVM>();
            OK.IsEnabled = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MultiSelectTreeView_OnSelectionChanged(object sender, EventArgs e)
        {
            if (DialogResult == true) return;
            var array = new CategoryVM[MultiSelectTreeView.SelectedItems.Count];
            MultiSelectTreeView.SelectedItems.CopyTo(array, 0);
            SelectedCategories = array.ToList();
            OK.IsEnabled = SelectedCategories.Any();
        }
    }
}