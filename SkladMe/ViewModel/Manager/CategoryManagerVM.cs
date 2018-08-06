using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SkDAL;
using SkDAL.Model;
using SkladMe.API;
using SkladMe.API.Methods;
using SkladMe.Controls;
using SkladMe.Infrastructure;
using SkladMe.Resources;
using SkladMe.View;
using System.Data.Entity;
using SkladMe.Properties;
using DbContext = SkDAL.DbContext;
using MessageBox = SkladMe.View.MessageBox;

namespace SkladMe.ViewModel
{
    public class CategoryManagerVM : BaseVM
    {
        #region Fields

        private readonly Dictionary<int, ProductVM> _allProducts = new Dictionary<int, ProductVM>();
        private readonly RangeObservableCollection<ProductVM> _searchColl;
        private CategoryVM _selectedCategory;
        private List<Category> _nonVisibleCategories;
        private ICommand _addCategoryCommand;
        private ICommand _addProductToCategoryFromSearchCommand;
        private ICommand _showCategoriesVisibilityWindowCommand;
        private ICommand _openCategoryCommand;
        private ICommand _addProductFromUrlCommand;
        private ICommand _addProductToCategoryCommand;
        private ICommand _removeCommand;
        private ICommand _resetCategoriesVisibilityCommand;
        private ICommand _expandAllCommand;
        private ICommand _minimizeAllCommand;

        #endregion

        public CategoryManagerVM(RangeObservableCollection<ProductVM> searchColl)
        {
            _searchColl = searchColl;
            _selectedCategory = null;
            _searchColl.CollectionChanged += SearchCollOnCollectionChanged;
            ProductManager = new ProductManagerVM(_searchColl);
            GetCategoriesFromDb().Wait();
        }

        #region Properties

        public ProductManagerVM ProductManager { get; }
        public ObservableCollection<CategoryVM> Categories { get; } = new ObservableCollection<CategoryVM>();
        public ObservableCollection<CategoryVM> OpenCategories { get; } = new ObservableCollection<CategoryVM>();
        public IList<ProductVM> SelectedProductsFromSearch { get; set; }

        public bool IsDeleteFromCurrentPlace { get; set; } = true;

        public CategoryVM SelectedCategoryInTree { get; set; }

        public CategoryVM SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region  Commands

        public ICommand AddCategoryCommand
            => _addCategoryCommand ?? (_addCategoryCommand = new RelayCommand(AddCategory));

        public ICommand RemoveCommand
            => _removeCommand ??
               (_removeCommand =
                   new RelayCommand(Remove,
                       () => SelectedCategoryInTree != null && SelectedCategoryInTree.CanModify()));

        public ICommand AddProductToCategoryCommand
            => _addProductToCategoryCommand ??
               (_addProductToCategoryCommand =
                   new RelayCommand(AddProductToCategoryHandler,
                       () => SelectedCategory?.ProductManager.SelectedProducts?.Count > 0));

        public ICommand AddProductToCategoryFromSearchCommand
            => _addProductToCategoryFromSearchCommand ??
               (_addProductToCategoryFromSearchCommand = new RelayCommand(AddProductToCategoryFromSearch,
                   () => SelectedProductsFromSearch?.Count > 0));

        public ICommand AddProductFromUrlCommand
            => _addProductFromUrlCommand ??
               (_addProductFromUrlCommand =
                   new RelayCommand(AddProductFromUrl, () => SelectedCategory != null && !SelectedCategory.IsReadonly));

        public ICommand ShowCategoriesVisibilityWindowCommand
            => _showCategoriesVisibilityWindowCommand ??
               (_showCategoriesVisibilityWindowCommand = new RelayCommand(ShowCategoriesVisibilityWindow));

        public ICommand OpenCategoryCommand
            => _openCategoryCommand ?? (_openCategoryCommand = new RelayCommand<CategoryVM>(OpenCategory));

        public ICommand ResetCategoriesVisibilityCommand => _resetCategoriesVisibilityCommand ??
                                                            (_resetCategoriesVisibilityCommand =
                                                                new RelayCommand(ResetCategoriesVisibility));

        public ICommand ExpandAllCommand =>
            _expandAllCommand ?? (_expandAllCommand = new RelayCommand(() => SetIsExpanded(true)));

        public ICommand MinimizeAllCommand =>
            _minimizeAllCommand ?? (_minimizeAllCommand = new RelayCommand(() => SetIsExpanded(false)));

        #endregion

        #region Methods

        private void SetIsExpanded(bool value)
        {
            void ProcessChildCategories(CategoryVM category)
            {
                foreach (var childCategory in category.ChildCategories)
                {
                    childCategory.IsExpanded = value;
                    ProcessChildCategories(childCategory);
                }
            }

            foreach (var categoryVm in Categories)
            {
                categoryVm.IsExpanded = value;
                ProcessChildCategories(categoryVm);
            }
        }

        private void SearchCollOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ProductVM productVm in e.NewItems)
                {
                    if (!_allProducts.ContainsKey(productVm.Model.Id))
                    {
                        _allProducts.Add(productVm.Model.Id, productVm);
                    }
                }
            }

            if (_isCategoriesVisibilitySetted)
            {
                _isCategoriesVisibilitySetted = false;
                var allCategories = GetAllCategories(Categories);
                allCategories.ForEach(c => c.IsProductsHide = false);
            }
        }

        private async Task GetCategoriesFromDb()
        {
            var expandedIds = GetExpandedCategoriesFromSettings();
            var openedCategories = GetOpenedCategoriesFromSettings();
            int selectedCategoryLastSesstion = 0;

            if (!string.IsNullOrWhiteSpace(Settings.Default.SelectedCategoryId))
            {
                selectedCategoryLastSesstion = Convert.ToInt32(Settings.Default.SelectedCategoryId);
            }


            void ParseChildren(CategoryVM currentNode)
            {
                if (openedCategories.Contains(currentNode.Model.Id))
                {
                    var currentSelectedCategory = SelectedCategory;
                    OpenCategory(currentNode);

                    SelectedCategory = currentNode.Model.Id == selectedCategoryLastSesstion
                        ? currentNode
                        : currentSelectedCategory;
                }

                if (expandedIds.Contains(currentNode.Model.Id))
                {
                    currentNode.IsExpanded = true;
                }

                if (currentNode.Model.Children is null) return;

                foreach (var child in currentNode.Model.Children.OrderBy(c => c.SortOrder))
                {
                    var childVm = new CategoryVM(child) {ParentCollection = currentNode.ChildCategories};
                    currentNode.ChildCategories.Add(childVm);
                    ParseChildren(childVm);
                }
            }

            var allCategories = await Db.Categories.AllAsync().ConfigureAwait(false);
            var firstLevel = allCategories.Where(c => c.ParentId == null).OrderBy(c => c.SortOrder).ToList();

            if (!firstLevel.Any(c => c.Title == "Избранное" && c.IsFixed))
            {
                await CreateFixedCategory("Избранное", 100).ConfigureAwait(false);
            }

            if (!firstLevel.Any(c => c.Title == "Черный список" && c.IsFixed))
            {
                await CreateFixedCategory("Черный список", 200).ConfigureAwait(false);
            }

            foreach (var child in firstLevel)
            {
                var vm = new CategoryVM(child) {ParentCollection = Categories};
                ParseChildren(vm);
                Categories.Add(vm);
            }
        }

        private int[] GetOpenedCategoriesFromSettings()
        {
            try
            {
                string value = Settings.Default.OpenedCategories;
                int[] arr = value.Split(MainVM.SettingsDelimiter).Select(int.Parse).ToArray();

                return arr;
            }
            catch (System.Exception)
            {
            }
            return new int[0];
        }

        private int[] GetExpandedCategoriesFromSettings()
        {
            try
            {
                string value = Settings.Default.ExpandedCategories;
                int[] arr = value.Split(MainVM.SettingsDelimiter).Select(int.Parse).ToArray();

                return arr;
            }
            catch (System.Exception)
            {
            }
            return new int[0];
        }

        private async Task CreateFixedCategory(string name, int sortOrder)
        {
            var blacklistCategory = new CategoryVM(new Category() {SortOrder = sortOrder})
            {
                Title = name,
                IsFixed = true,
                ParentCollection = Categories
            };
            Categories.Add(blacklistCategory);
            await Db.Categories.AddOrUpdateAsync(blacklistCategory.Model).ConfigureAwait(false);
        }

        private async void LoadProducts(CategoryVM category)
        {
            if (category is null) return;

            category.IsLoading = true;
            await AsyncHelper.ExecuteAtUI(() => category.Products.Clear()).ConfigureAwait(false);

            var productsFromDb =
                await Db.Categories.GetAllProductsAsync(category.Model).ConfigureAwait(false);

            var productVmForDataGrid = new List<ProductVM>();
            foreach (var product in productsFromDb)
            {
                ProductVM productVm;
                if (!_allProducts.ContainsKey(product.Id))
                {
                    productVm = new ProductVM(product);
                    _allProducts.Add(product.Id, productVm);
                }
                else
                {
                    productVm = _allProducts[product.Id];
                }
                productVmForDataGrid.Add(productVm);
            }
            await AsyncHelper.ExecuteAtUI(() => category.Products.AddRange(productVmForDataGrid)).ConfigureAwait(false);

            category.AllProducts.Clear();
            category.AllProducts.AddRange(category.Products);

            category.IsLoading = false;
            category.IsProductsLoaded = true;
        }

        private async void AddCategory()
        {
            var categoryDialog = new CreateCategoryWindow {Categories = Categories};
            if (categoryDialog.ShowDialog() == true)
            {
                var newCategory = new CategoryVM(new Category() {Title = categoryDialog.CategoryName.Trim()})
                {
                    ParentCollection = Categories
                };
                var parentCategory = SelectedCategoryInTree != null &&
                                     (SelectedCategoryInTree.IsReadonly || SelectedCategoryInTree.IsFixed)
                    ? null
                    : SelectedCategoryInTree;
                if (parentCategory != null)
                {
                    newCategory.ParentCollection = parentCategory.ChildCategories;
                }

                var prevElement = newCategory.ParentCollection.LastOrDefault();
                var prevSortOrder = prevElement?.Model.SortOrder ?? 0;

                newCategory.Model.SortOrder = prevSortOrder + 100;
                newCategory.Model.ParentId = prevElement?.Model.ParentId ?? parentCategory?.Model.Id;
                newCategory.ParentCollection.Add(newCategory);
                await Db.Categories.AddOrUpdateAsync(newCategory.Model).ConfigureAwait(false);
            }
        }

        private async void Remove()
        {
            var category = SelectedCategoryInTree;
            bool hasChilds = category.AllProducts.Count > 0 ||
                             Db.Categories.ProductAndCategories.Any(x => x.CategoryId == category.Model.Id);
            if (hasChilds || category.ChildCategories.Count > 0)
            {
                var result = await
                    View.MessageBox.ShowAsync(
                        $"Вы действительно хотите удалить категорию \"{category.Title}\" вместе со всем содержимым?",
                        "Удаление",
                        MessageBoxButton.OKCancel, MessageBoxImage.Question).ConfigureAwait(false);
                if (result != MessageBoxResult.OK) return;
            }

            await AsyncHelper.ExecuteAtUI(() =>
            {
                category.ParentCollection.Remove(category);
                CloseChildCategories(category);
                CloseCategory(category, EventArgs.Empty);
            }).ConfigureAwait(false);

            await Db.Categories.DeleteAsync(category.Model).ConfigureAwait(false);
        }

        private void CloseChildCategories(CategoryVM closeCategory)
        {
            foreach (var childCategory in closeCategory.ChildCategories)
            {
                if (childCategory.ChildCategories.Count > 0)
                {
                    CloseChildCategories(childCategory);
                }
                CloseCategory(childCategory, EventArgs.Empty);
            }
        }

        private void CloseCategory(object sender, EventArgs e)
        {
            if (sender is CategoryVM category)
            {
                var selectedIndex = OpenCategories.IndexOf(category);
                if (category == SelectedCategory)
                {
                    SelectedCategory = selectedIndex > 1
                        ? OpenCategories[selectedIndex - 1]
                        : OpenCategories.FirstOrDefault();
                }
                OpenCategories.Remove(category);
            }
        }

        private void OpenCategory(CategoryVM category)
        {
            if (OpenCategories.Contains(category))
            {
                SelectedCategory = category;
                return;
            }

            if (category.IsProductsLoaded == false || category.Products.Any() == false)
            {
                LoadProducts(category);
                category.Close += CloseCategory;
            }

            OpenCategories.Add(category);
            SelectedCategory = category;
        }

        private async void AddProductToCategoryHandler()
        {
            bool additionSuccess =
                await ShowAddProductToCategoryDialogAsync(ProductManager.SelectedProducts).ConfigureAwait(false);
            if (additionSuccess && IsDeleteFromCurrentPlace)
            {
                var productsToDelete = ProductManager.SelectedProducts.ToList();
                foreach (var productVm in productsToDelete)
                {
                    SelectedCategory.AllProducts.Remove(productVm);
                    await AsyncHelper.ExecuteAtUI(() => SelectedCategory.Products.Remove(productVm))
                        .ConfigureAwait(false);
                }
                await Db.Categories.DeleteProductsAsync(SelectedCategory.Model,
                        productsToDelete.Select(p => p.Model).ToList())
                    .ConfigureAwait(false);
            }
        }

        private async void AddProductToCategoryFromSearch()
        {
            bool additionSuccess =
                await ShowAddProductToCategoryDialogAsync(SelectedProductsFromSearch, true).ConfigureAwait(false);
            if (additionSuccess && IsDeleteFromCurrentPlace)
            {
                var productsToDelete = SelectedProductsFromSearch.ToList();
                foreach (var productVm in productsToDelete)
                {
                    _tempSearchColl?.Remove(productVm);
                    await AsyncHelper.ExecuteAtUI(() => _searchColl.Remove(productVm)).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> ShowAddProductToCategoryDialogAsync(IEnumerable<ProductVM> products,
            bool addFromSearch = false)
        {
            var productVms = products as IList<ProductVM> ?? products.ToList();
            var addToCategoryDialog = new AddToCategoryWindow() {DataContext = this, Products = productVms.ToList()};
            if (addToCategoryDialog.ShowDialog() == true)
            {
                if (!addFromSearch && SelectedCategory != null)
                {
                    bool sameCategory = addToCategoryDialog.SelectedCategories.Count == 1 &&
                                        addToCategoryDialog.SelectedCategories[0].Model.Id ==
                                        SelectedCategory.Model.Id;
                    if (sameCategory) return false;
                }


                foreach (var category in addToCategoryDialog.SelectedCategories)
                {
                    await ProductManagerVM.AddProductToCategoryAsync(category, productVms).ConfigureAwait(false);
                }
                return true;
            }
            return false;
        }

        private async void AddProductFromUrl()
        {
            var category = SelectedCategory;
            var dialog = new AddProductFromUrlWindow();
            if (dialog.ShowDialog() == true)
            {
                string url = dialog.Url;
                Product addedProduct = null;

                try
                {
                    var addedProductId = Products.ParseId(url);

                    if (category.Products.Any(p => p.Model.Id == addedProductId))
                    {
                        await MessageBox.ShowAsync("Данная складчина уже была добавлена ранее.", "Информация",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information).ConfigureAwait(false);
                        return;
                    }

                    Product dbProduct;
                    using (var context = new DbContext())
                    {
                        dbProduct = await context.Products.AsNoTracking().Where(x => x.Id == addedProductId)
                            .Include(p => p.Prefix)
                            .Include(p => p.Chapter)
                            .Include(p => p.Subсhapter)
                            .Include(p => p.Creator)
                            .Include(p => p.Organizer)
                            .FirstOrDefaultAsync().ConfigureAwait(false);
                    }

                    if (dbProduct != null)
                    {
                        addedProduct = dbProduct;
                    }
                    else
                    {
                        var skApi = new SkladchikApiClient(new CancellationToken());
                        bool isNeedGetCookie = await Cookies.IsNeedGetCookie().ConfigureAwait(false);
                        if (isNeedGetCookie)
                        {
                            var cookieContainer = Cookies.GetSkladchikCookie();
                            skApi.CookieContainer = cookieContainer;
                            if (cookieContainer.Count == 0)
                            {
                                await MessageBox.ShowAsync("Добавление складчины невозможно. Загрузите сайт в браузере и попробуйте заново.", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error).ConfigureAwait(false);
                                return;
                            }
                        }

                        

                        addedProduct = await skApi.Products.GetByUrlAsync(url).ConfigureAwait(false);
                    }
                }
                catch (System.Exception)
                {
                    // ignored
                }

                if (addedProduct == null)
                {
                    await MessageBox.ShowAsync("Ошибка добавления. Проверьте указанный URL.", "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error).ConfigureAwait(false);
                    return;
                }

                ProductVM productVm;
                if (_allProducts.ContainsKey(addedProduct.Id))
                {
                    productVm = _allProducts[addedProduct.Id];
                }
                else
                {
                    productVm = new ProductVM(addedProduct);
                    _allProducts.Add(addedProduct.Id, productVm);
                }

                await Db.Products.AddOrUpdateAsync(new List<Product> {addedProduct}).ConfigureAwait(false);
                await Db.Categories.AddProductsAsync(category.Model, new List<Product> {addedProduct})
                    .ConfigureAwait(false);
                await ProductManagerVM.AddProductToCategoryAsync(category, new List<ProductVM> {productVm})
                    .ConfigureAwait(false);
            }
        }

        private List<ProductVM> _tempSearchColl;
        private bool _isCategoriesVisibilitySetted;

        private void ShowCategoriesVisibilityWindow()
        {
            var categoriesWindow = new CategoriesVisibility {DataContext = this};
            var allCategories = GetAllCategories(Categories);

            var currentCategoriesState =
                allCategories.Select(c => new CategoryVM(c.Model) {IsProductsHide = c.IsProductsHide}).ToList();
            if (categoriesWindow.ShowDialog() == true)
            {
                _nonVisibleCategories =
                    GetAllCategories(Categories).Where(c => c.IsProductsHide).Select(c => c.Model).ToList();
                if (!_isCategoriesVisibilitySetted)
                {
                    _tempSearchColl = new List<ProductVM>();
                    _tempSearchColl.AddRange(_searchColl);
                }
                var visibleProducts = GetVisibleProducts(_tempSearchColl);
                _searchColl.Clear();
                _searchColl.AddRange(visibleProducts);
                _isCategoriesVisibilitySetted = true;
            }
            else
            {
                allCategories.ForEach(
                    changedCategory =>
                        changedCategory.IsProductsHide =
                            currentCategoriesState.Find(c => c.Model.Id == changedCategory.Model.Id)
                                .IsProductsHide);
            }
        }

        private void ResetCategoriesVisibility()
        {
            var allCategories = GetAllCategories(Categories);
            allCategories.ForEach(c => c.IsProductsHide = false);
        }


        private List<CategoryVM> GetAllCategories(IEnumerable<CategoryVM> categories)
        {
            var allCategories = new List<CategoryVM>();
            foreach (var category in categories)
            {
                allCategories.Add(category);
                if (category.ChildCategories.Any())
                {
                    allCategories.AddRange(GetAllCategories(category.ChildCategories));
                }
            }

            return allCategories;
        }

        private IEnumerable<ProductVM> GetVisibleProducts(IEnumerable<ProductVM> products)
        {
            var visibleProducts = new List<ProductVM>();
            var nonVisibleCategoryIds = _nonVisibleCategories.Select(c => c.Id).ToList();

            foreach (var productVm in products)
            {
                var categoryIdsOfCurrentProduct =
                    Db.Categories.ProductAndCategories.Where(pc => pc.ProductId == productVm.Model.Id)
                        .Select(pc => pc.CategoryId)
                        .ToList();
                var isProductVisible = !nonVisibleCategoryIds.Intersect(categoryIdsOfCurrentProduct).Any();

                if (isProductVisible)
                {
                    visibleProducts.Add(productVm);
                }
            }

            return visibleProducts;
        }

        #endregion Methods
    }
}