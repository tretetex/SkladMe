using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SkDAL;
using SkDAL.Model;
using SkladMe.Controls;
using SkladMe.Controls.DragDropHandlers;
using SkladMe.Infrastructure;
using SkladMe.Model;
using SkladMe.Resources;

namespace SkladMe.ViewModel
{
    public class CategoryVM : BaseVM
    {
        #region fields

        private int? _productsCount;
        private bool _isProductsHide;
        private bool _isEditing;
        private bool _isLoading;

        private ICommand _closeCommand;
        private ICommand _removeProductsCommand;
        private ICommand _renameCommand;
        private ICommand _resetFiltersCommand;
        private ICommand _showFiltersCommand;

        public readonly EventHandler<DropEventArgs> Moved;
        private bool _isExpanded;
        public event EventHandler Close;
        

        #endregion fields
            
        public CategoryVM(Category model)
        {
            Model = model;
            ProductManager = new ProductManagerVM(Products);
            Moved += CategoryMovedHandler;
            Products.CollectionChanged += (sender, args) => RefreshProductsCount();
            RefreshProductsCount();
        }

        #region Properties

        public List<ProductVM> AllProducts { get; } = new List<ProductVM>();
        public RangeObservableCollection<ProductVM> Products { get; } = new RangeObservableCollection<ProductVM>();

        public ObservableCollection<CategoryVM> ParentCollection { get; set; }
        public ObservableCollection<CategoryVM> ChildCategories { get; } = new ObservableCollection<CategoryVM>();
        
        public FiltersManagerVM FiltersManagerVM { get; } =
            new FiltersManagerVM(FiltersManagerVM.TemporaryCategoryFiltersPath);

        public ProductManagerVM ProductManager { get; }
        public Category Model { get; }

        public bool IsProductsLoaded { get; set; }

        public string Title
        {
            get { return Model.Title; }
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        public bool IsFixed
        {
            get { return Model.IsFixed; }
            set
            {
                Model.IsFixed = value;
                OnPropertyChanged();
            }
        }

        public bool IsReadonly
        {
            get { return Model.IsReadonly; }
            set
            {
                Model.IsReadonly = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsProductsHide
        {
            get { return _isProductsHide; }
            set
            {
                _isProductsHide = value;
                OnPropertyChanged();
            }
        }

        public int? ProductsCount
        {
            get => _productsCount;
            set
            {
                _productsCount = value; 
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value; 
                OnPropertyChanged();
            }
        }

        public bool IsFilterSetted { get; private set; }

        #endregion Properties

        #region commands
        public ICommand RenameCommand => _renameCommand ?? (_renameCommand = new RelayCommand(() => IsEditing = true, CanModify));

        public ICommand ShowFiltersCommand
            => _showFiltersCommand ??
                (_showFiltersCommand = new RelayCommand(ShowFilters, () => Products.Any() || AllProducts.Any()));

        public ICommand ResetFiltersCommand
            => _resetFiltersCommand ?? (_resetFiltersCommand = new RelayCommand(ResetFilters, () => IsFilterSetted));

        public ICommand RemoveProductsCommand
            => _removeProductsCommand ??
               (_removeProductsCommand =
                   new RelayCommand(RemoveProductsFromCategory, () => !IsReadonly && IsSelectedProductsExists()));

        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(() => Close?.Invoke(this, EventArgs.Empty)));

        private bool IsSelectedProductsExists() => ProductManager.SelectedProducts != null && ProductManager.SelectedProducts.Count > 0;
        public bool CanModify() => !IsFixed && !IsReadonly;
        #endregion commands

        #region methods

        private async void CategoryMovedHandler(object sender, DropEventArgs args)
        {
            var target = sender as CategoryVM;
            if (this == target) return;

            bool isSourceChildOfTarget = target.ChildCategories == ParentCollection;
            Model.ParentId = isSourceChildOfTarget ? target.Model.Id : target.Model.ParentId;
            Model.Parent = null;

            var sourceIndex = ParentCollection.IndexOf(this);
            var prevElement = ParentCollection.Take(sourceIndex).LastOrDefault();
            var nextElement = ParentCollection.Skip(sourceIndex + 1).FirstOrDefault();

            var prevSortOrder = prevElement?.Model.SortOrder ?? 0;
            var nextSortOrder = nextElement?.Model.SortOrder ?? 0;

            if (nextElement != null)
            {
                Model.SortOrder = (prevSortOrder + nextSortOrder) / 2 + 1;
            }
            else
            {
                Model.SortOrder = prevSortOrder + 100;
            }

            await Db.Categories.AddOrUpdateAsync(Model).ConfigureAwait(false);
        }

        private void ShowFilters()
        {
            FiltersManagerVM.SaveFilters(FiltersManagerVM.TemporaryCategoryFiltersPath);
            var tempFiltersVm = FiltersManagerVM.FiltersVM;
            var filtersWindow = new FiltersWindow {DataContext = this};
            if (filtersWindow.ShowDialog() == true)
            {
                if (!IsFilterSetted)
                {
                    AllProducts.Clear();
                    AllProducts.AddRange(Products);
                    IsFilterSetted = true;
                }
                Products.Clear();

                var enabledSubchapter = FiltersManagerVM.FiltersVM.GetEnabledSubchapter();

                foreach (var productVm in AllProducts)
                {
                    if (enabledSubchapter.All(s => s.Id != productVm.Model.SubchapterId)) continue;
                    bool isGoodProduct = Filters.IsGoodProduct(productVm.Model, FiltersManagerVM.FiltersVM.Model);
                    if (isGoodProduct)
                    {
                        Products.Add(productVm);
                    }
                }
            }
            else
            {
                FiltersManagerVM.LoadingFilters(FiltersManagerVM.TemporaryCategoryFiltersPath);
                FiltersManagerVM.FiltersVM = tempFiltersVm;
            }
        }

        private void ResetFilters()
        {
            IsFilterSetted = false;
            Products.Clear();
            Products.AddRange(AllProducts);
        }

        private async void RemoveProductsFromCategory()
        {
            await RemoveProductsAsync(ProductManager.SelectedProducts.ToList()).ConfigureAwait(false);
        }

        public async Task RemoveProductsAsync(IEnumerable<ProductVM> products)
        {
            var deleteFromDb = products.Select(p => p.Model).ToList();
            await Db.Categories.DeleteProductsAsync(Model, deleteFromDb).ConfigureAwait(false);

            await AsyncHelper.ExecuteAtUI(() =>
            {
                foreach (var productVm in products)
                {
                    Products.Remove(productVm);
                    AllProducts.Remove(productVm);
                }
            }).ConfigureAwait(false);
        }

        private void RefreshProductsCount()
        {
            ProductsCount = Db.Categories.ProductAndCategories.Count(p => p.CategoryId == Model.Id);
            if (ProductsCount == 0) ProductsCount = null;
        }
        #endregion methods
    }
}