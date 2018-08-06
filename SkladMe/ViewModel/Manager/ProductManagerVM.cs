using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NLog;
using SkDAL;
using SkladMe.API;
using SkladMe.API.Methods;
using SkladMe.Infrastructure;
using SkladMe.Model;
using MessageBox = SkladMe.View.MessageBox;

namespace SkladMe.ViewModel
{
    public class ProductManagerVM : BaseVM
    {
        #region fields  
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ObservableCollection<ProductVM> _products;
        
        private int _productsCount;
        private int _openProductsCount;
        private int _activeProductsCount;
        private int _closedProductsCount;
        private int _availableProductsCount;
        private bool _isRefreshing;
        private bool _isExporting;

        private CancellationTokenSource _ctsRefreshProducts;
        private CancellationTokenSource _ctsSaveProducts;

        private ICommand _removeProductsCommand;
        private ICommand _openProductsCommand;
        private ICommand _exportCommand;
        private ICommand _refreshCommand;
        private ICommand _refreshSelectedProductsCommand;
        private ICommand _saveProductsCommand;
        private ICommand _cancelRunningOperationCommand;
        private ICommand _resetColorCommand;
        private ICommand _copyLinksCommand;

        #endregion fields

        public ProductManagerVM(ObservableCollection<ProductVM> products)
        {
            _products = products;
            _products.CollectionChanged += (sender, args) => RefreshCounters();
        }

        #region properties
        public IList<ProductVM> SelectedProducts { get; set; }


        public int ProductsCount
        {
            get { return _productsCount; }
            set
            {
                _productsCount = value;
                OnPropertyChanged();
            }
        }

        public int OpenProductsCount
        {
            get { return _openProductsCount; }
            set
            {
                _openProductsCount = value;
                OnPropertyChanged();
            }
        }

        public int ActiveProductsCount
        {
            get { return _activeProductsCount; }
            set
            {
                _activeProductsCount = value;
                OnPropertyChanged();
            }
        }

        public int ClosedProductsCount
        {
            get { return _closedProductsCount; }
            set
            {
                _closedProductsCount = value;
                OnPropertyChanged();
            }
        }

        public int AvailableProductsCount
        {
            get { return _availableProductsCount; }
            set
            {
                _availableProductsCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsExporting
        {
            get { return _isExporting; }
            set
            {
                _isExporting = value;
                OnPropertyChanged();
            }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        #endregion properties   

        #region commands

        public ICommand RemoveProductsCommand
            => _removeProductsCommand ?? (_removeProductsCommand = new RelayCommand(RemoveProducts, IsSelectedProductsExists));

        public ICommand OpenProductsCommand
            => _openProductsCommand ?? (_openProductsCommand = new RelayCommand(OpenProduct, IsSelectedProductsExists));

        public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new RelayCommand(SaveAllProducts, () => _products.Count > 0));

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new RelayCommand(Refresh, () => _products.Count > 0));

        public ICommand RefreshSelectedProductsCommand
            => _refreshSelectedProductsCommand ?? (_refreshSelectedProductsCommand = new RelayCommand(RefreshSelectedProducts, IsSelectedProductsExists));

        public ICommand SaveProductsCommand => _saveProductsCommand ?? (_saveProductsCommand = new RelayCommand(SaveSelectedProducts, IsSelectedProductsExists));
        
        public ICommand CancelRunningOperationCommand => _cancelRunningOperationCommand ?? (_cancelRunningOperationCommand = new RelayCommand(CancelRunningOperation));

        public ICommand ResetColorCommand => _resetColorCommand ?? (_resetColorCommand = new RelayCommand(ResetColor));

        public ICommand CopyLinksCommand => _copyLinksCommand ?? (_copyLinksCommand = new RelayCommand(CopyLinks));

        #endregion


        public bool IsSelectedProductsExists() => SelectedProducts != null && SelectedProducts.Any();
        #region methods

        private void RefreshCounters()
        {
            ProductsCount = _products.Count;
            OpenProductsCount = _products.Count(p => p.Model.PrefixId == (int)Products.PrefixId.Open);
            ActiveProductsCount = _products.Count(p => p.Model.PrefixId == (int)Products.PrefixId.Active);
            AvailableProductsCount = _products.Count(p => p.Model.PrefixId == (int)Products.PrefixId.Available);
            ClosedProductsCount = _products.Count(p => p.Model.PrefixId == (int)Products.PrefixId.Completed);
        }

        private void RemoveProducts()
        {
            var products = SelectedProducts.ToList();
            foreach (var product in products)
            {
                _products.Remove(product);
            }
        }

        private async void OpenProduct()
        {
            var products = SelectedProducts.ToList();
            if (products.Count > 10)
            {
                var result = await MessageBox.ShowAsync(
                    $"Вы собираетесь открыть {products.Count} складчин в браузере. Это может сильно замедлить его работу. \r\n Хотите продолжить?",
                    "", MessageBoxButton.YesNo, MessageBoxImage.Warning).ConfigureAwait(false);

                if (result != MessageBoxResult.Yes) return;
            }

            foreach (ProductVM p in products)
            {
                string link = Products.GetProductLink(p.Model);
                Process.Start(link);
            }
        }

        private async void SaveAllProducts()
        {
            IsExporting = true;
            _ctsSaveProducts = new CancellationTokenSource();


            try
            {
                var task = Export.SaveCollection(_products, _ctsSaveProducts.Token);
                CriticalTasks.Add(task);
                await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {

            }
            catch (System.Exception e)
            {
                _logger.Error(e);
                await MessageBox.ShowAsync("Произошла ошибка во время экспорта.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error).ConfigureAwait(false);
            }
            
            CriticalTasks.Cleanup();

            IsExporting = false;
        }

        private async void SaveSelectedProducts()
        {
            IsExporting = true;
            _ctsSaveProducts = new CancellationTokenSource();

            var products = SelectedProducts.ToList();
            
            try
            {
                var task = Export.SaveCollection(products, _ctsSaveProducts.Token);
                CriticalTasks.Add(task);
                await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {

            }
            catch (System.Exception e)
            {
                _logger.Error(e);
                await MessageBox.ShowAsync("Произошла ошибка во время экспорта.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error).ConfigureAwait(false);
            }
            CriticalTasks.Cleanup();

            IsExporting = false;
        }

        private async void Refresh()
        {
            if (IsRefreshing)
            {
                _ctsRefreshProducts?.Cancel();
                IsRefreshing = false;
                return;
            }

            IsRefreshing = true;

            _ctsRefreshProducts = new CancellationTokenSource();

            try
            {
                await RefreshProductsAsync(_products, _ctsRefreshProducts).ConfigureAwait(false);
                RefreshCounters();
            }
            catch (OperationCanceledException)
            {
            }

            IsRefreshing = false;
        }

        private async Task RefreshProductsAsync(IEnumerable<ProductVM> coll, CancellationTokenSource cts)
        {
            var skApi = new SkladchikApiClient(cts.Token);
            bool isNeedGetCookie = await Cookies.IsNeedGetCookie().ConfigureAwait(false);
            if (isNeedGetCookie)
            {
                var cookieContainer = Cookies.GetSkladchikCookie();
                skApi.CookieContainer = cookieContainer;
                if (cookieContainer.Count == 0)
                {
                    await MessageBox.ShowAsync("Обновление складчин невозможно. Загрузите сайт в браузере и начните обновление заново.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error).ConfigureAwait(false);
                    cts.Cancel();
                    return;
                }
            }

            foreach (var productVm in coll)
            {
                cts.Token.ThrowIfCancellationRequested();
                var updatedProduct = await skApi.Products.GetByIdAsync(productVm.Model.Id)
                    .ConfigureAwait(false);

                bool productWasDeleted = updatedProduct == null;
                if (productWasDeleted)
                {
                    await Db.Products.RemoveAsync(productVm.Model).ConfigureAwait(false);
                }
                else
                {
                    var oldProduct = productVm.Model;
                    updatedProduct.Color = oldProduct.Color;
                    updatedProduct.Note = oldProduct.Note;
                    updatedProduct.ViewCount = oldProduct.ViewCount;
                    productVm.Model = updatedProduct;
                }
            }

            await Db.Products.AddOrUpdateAsync(coll.Select(p => p.Model).ToList()).ConfigureAwait(false);
        }

        private void CopyLinks()
        {
            var products = SelectedProducts.ToList();
            var sb = new StringBuilder();
            foreach (var product in products)
            {
                var url = "https://skladchik.com/threads/" + product.Model.Id;
                sb.Append(url + Environment.NewLine);
            }

            var text = sb.ToString().TrimEnd();
            Clipboard.SetText(text);
        }

        private async void RefreshSelectedProducts()
        {
            if (IsRefreshing)
            {
                _ctsRefreshProducts?.Cancel();
                IsRefreshing = false;
                return;
            }

            IsRefreshing = true;

            _ctsRefreshProducts = new CancellationTokenSource();

            try
            {
                var products = SelectedProducts.ToList();
                await RefreshProductsAsync(products, _ctsRefreshProducts).ConfigureAwait(false);
                RefreshCounters();
            }
            catch (OperationCanceledException)
            {
            }

            IsRefreshing = false;
        }

        private void CancelRunningOperation()
        {
            if (IsExporting)
            {
                _ctsSaveProducts.Cancel();
                IsExporting = false;
            }
            else if (IsRefreshing)
            {
                _ctsRefreshProducts.Cancel();
                RefreshCounters();
                IsRefreshing = false;
            }

        }

        public static async Task AddProductToCategoryAsync(CategoryVM category, IEnumerable<ProductVM> products)
        {
            await Db.Categories.AddProductsAsync(category.Model, products.Select(p => p.Model).ToList()).ConfigureAwait(false);

            foreach (var productVm in products)
            {
                if (category.AllProducts.Any(p => p.Model.Id == productVm.Model.Id))
                {
                    continue;
                }

                var isGoodProduct = true;
                if (category.IsFilterSetted)
                {
                    isGoodProduct = Filters.IsGoodProduct(productVm.Model, category.FiltersManagerVM.FiltersVM.Model);
                }
                if (isGoodProduct)
                {
                    await AsyncHelper.ExecuteAtUI(() => category.Products.Add(productVm)).ConfigureAwait(false);
                }
                category.AllProducts.Add(productVm);
            }
        }

        private async void ResetColor()
        {
            var products = SelectedProducts.Select(p =>
            {
                p.Color = "#00FFFFFF";
                return p.Model;
            }).ToList();

            await AsyncHelper.ProcessCollectionAsync(products, Db.Products.UpdateColorAsync).ConfigureAwait(false);
        }

        #endregion
    }
}