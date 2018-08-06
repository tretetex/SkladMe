using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SkDAL;
using SkDAL.Model;
using SkladMe.API;
using SkladMe.Controls;
using SkladMe.Infrastructure;
using SkladMe.Model;
using SkladMe.Parsers;
using SkladMe.Resources;
using DbContext = SkDAL.DbContext;
using MessageBox = SkladMe.View.MessageBox;

namespace SkladMe.ViewModel
{
    public class SearchVM : BaseVM
    {
        #region fields

        private bool _isStarted;
        private string _currentSubchapterTitle;
        private int _currentSubchapterNumber;
        private int _totalSubchapter = 1;
        private CancellationTokenSource _cts;
        private SkladchikApiClient _skApi;
        private ICommand _showFiltersCommand;
        private string _status;
        private string _productProcessingStatus;

        #endregion

        public SearchVM()
        {
            _cts = new CancellationTokenSource();
            _skApi = new SkladchikApiClient(_cts.Token);
            StartCommand = new RelayCommand(StartAsync);
            FiltersManagerVM = new FiltersManagerVM(FiltersManagerVM.TemporaryFiltersPath);
        }

        #region properties

        public FiltersManagerVM FiltersManagerVM { get; }

        public RangeObservableCollection<ProductVM> Products { get; } = new RangeObservableCollection<ProductVM>();

        public int CurrentSubchapterNumber
        {
            get { return _currentSubchapterNumber; }
            set
            {
                _currentSubchapterNumber = value;
                OnPropertyChanged();
            }
        }

        public int TotalSubchapter
        {
            get { return _totalSubchapter; }
            set
            {
                _totalSubchapter = value;
                OnPropertyChanged();
            }
        }

        public string CurrentSubchapterTitle
        {
            get { return _currentSubchapterTitle; }
            set
            {
                _currentSubchapterTitle = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string ProductProcessingStatus
        {
            get { return _productProcessingStatus; }
            set
            {
                _productProcessingStatus = value;
                OnPropertyChanged();
            }
        }

        public bool IsStarted
        {
            get { return _isStarted; }
            set
            {
                _isStarted = value;
                OnPropertyChanged();
            }
        }

        public static bool UpdateViewCount { get; set; }

        public static bool IsLocalSearch { get; set; }


        public ICommand StartCommand { get; }

        public ICommand ShowFiltersCommand
            => _showFiltersCommand ?? (_showFiltersCommand = new RelayCommand(ShowFilters, () => !IsStarted));

        private void ShowFilters()
        {
            var tempFiltersVm = FiltersManagerVM.FiltersVM;
            FiltersManagerVM.SaveFilters(FiltersManagerVM.TemporaryFiltersPath);
            var filtersWindow = new FiltersWindow {VisibileIsLocalSearch = true, DataContext = this};
            if (filtersWindow.ShowDialog() != true)
            {
                FiltersManagerVM.FiltersVM = tempFiltersVm;
                FiltersManagerVM.IsFilterSetted = false;
            }
        }

        #endregion

        #region private method

        private async void StartAsync()
        {
            var task = SearchStartAsync();
            CriticalTasks.Add(task);
            await task.ConfigureAwait(false);
            CriticalTasks.Cleanup();
        }

        private async Task SearchStartAsync()
        {
            if (!IsStarted)
            {
                CurrentSubchapterNumber = 0;
                CurrentSubchapterTitle = null;
                Status = null;
                ProductProcessingStatus = null;
                IsStarted = true;

                await AsyncHelper.RedirectToThreadPool();

                var isClearLastResult = await IsNeedClearLastResultAsync().ConfigureAwait(false);
                if (isClearLastResult == true)
                {
                    await AsyncHelper.ExecuteAtUI(() => Products.Clear()).ConfigureAwait(false);
                }
                else if (isClearLastResult is null)
                {
                    IsStarted = false;
                    return;
                }

                try
                {
                    if (!await IsCookiesExistsAsync().ConfigureAwait(false))
                    {
                        IsStarted = false;
                        return;
                    }

                    _cts = new CancellationTokenSource();

                    if (IsLocalSearch)
                    {
                        await GetProductsFromDbAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        await GetProductsAsync().ConfigureAwait(false);
                    }
                    IsStarted = false;
                }
                catch (OperationCanceledException)
                {
                }
            }
            else
            {
                IsStarted = false;
                _cts.Cancel();
            }
        }

        private async Task<bool?> IsNeedClearLastResultAsync()
        {
            if (Products.Count > 0)
            {
                var mbResult = await MessageBox.ShowAsync("Очистить предыдущие результаты?", "Внимание",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Information).ConfigureAwait(false);

                switch (mbResult)
                {
                    case MessageBoxResult.Yes:
                        return true;
                    case MessageBoxResult.Cancel:
                    case MessageBoxResult.None:
                        return null;
                    default:
                        return false;
                }
            }
            return false;
        }

        private async Task<bool> IsCookiesExistsAsync()
        {
            bool isNeedGetCookie = await Cookies.IsNeedGetCookie().ConfigureAwait(false);
            if (isNeedGetCookie)
            {
                var cookieContainer = Cookies.GetSkladchikCookie();
                _skApi.CookieContainer = cookieContainer;

                if (cookieContainer.Count == 0)
                {
                    await MessageBox.ShowAsync("Поиск невозможен. Загрузите сайт в браузере и начните поиск заново.",
                        "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error).ConfigureAwait(false);
                    return false;
                }
            }

            return true;
        }

        private async void OnProductInfoUpdatedHandler(object sender, ProductEventArgs e)
        {
            double percents = (e.ProductProcessed + _dbGoodProductsCountOfChapter) /
                              ((e.AllProductsCount + _dbGoodProductsCountOfChapter) / 100.0);
            ProductProcessingStatus = $"Обработка раздела: {percents: 0.00} %";
            var products = e.Products.Where(p => Filters.IsGoodProduct(p, FiltersManagerVM.FiltersVM.Model)).ToList();

            try
            {
                await AddToResultsAsync(products).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        private async Task AddToResultsAsync(IEnumerable<Product> newProducts)
        {
            var goodProducts = new List<ProductVM>();
            lock (Products)
            {
                foreach (var newProduct in newProducts)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    bool isProductExist = Products.Any(x => x.Model.Id == newProduct.Id);
                    if (!isProductExist)
                    {
                        goodProducts.Add(new ProductVM(newProduct));
                    }
                }
            }

            await AsyncHelper.ExecuteAtUI(() =>
            {
                lock (Products)
                {
                    Products.AddRange(goodProducts);
                }
            }).ConfigureAwait(false);
        }

        private async Task GetProductsFromDbAsync()
        {
            var productSync = new ProductParser(_skApi, _cts.Token);
            productSync.ProductInfoUpdated += OnProductInfoUpdatedHandler;

            var enabledSubchapters = FiltersManagerVM.FiltersVM.GetEnabledSubchapter();
            TotalSubchapter = enabledSubchapters.Count;
            ProductProcessingStatus = "Получение складчин из базы...";

            for (var i = 0; i < enabledSubchapters.Count; i++)
            {
                _cts.Token.ThrowIfCancellationRequested();

                CurrentSubchapterNumber = i + 1;
                var subchapter = enabledSubchapters.ElementAt(i);
                CurrentSubchapterTitle = subchapter.Name;

                Status = "Раздел: " + CurrentSubchapterTitle + "    " + CurrentSubchapterNumber + "/" +
                         enabledSubchapters.Count;

                var startFrom = 0;
                List<Product> products;
                _dbGoodProductsCountOfChapter = 0;

                var productsCount = await Db.Products.Count(subchapter.Id).ConfigureAwait(false);

                do
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    products = await Db.Products.GetAsync(subchapter.Id, startFrom, 1000).ConfigureAwait(false);

                    OnProductInfoUpdatedHandler(null,
                        new ProductEventArgs
                        {
                            Products = products,
                            AllProductsCount = productsCount,
                            ProductProcessed = startFrom + products.Count
                        });

                    startFrom += 1000;
                } while (products.Count == 1000);
            }
        }

        private int _dbGoodProductsCountOfChapter = 0;

        private async Task GetProductsAsync()
        {
            var productSync = new ProductParser(_skApi, _cts.Token);
            productSync.ProductInfoUpdated += OnProductInfoUpdatedHandler;

            var enabledSubchapters = FiltersManagerVM.FiltersVM.GetEnabledSubchapter();
            TotalSubchapter = enabledSubchapters.Count;

            for (var i = 0; i < enabledSubchapters.Count; i++)
            {
                _cts.Token.ThrowIfCancellationRequested();

                CurrentSubchapterNumber = i + 1;
                var subchapter = enabledSubchapters.ElementAt(i);
                CurrentSubchapterTitle = subchapter.Name;

                Status = "Раздел: " + CurrentSubchapterTitle + "    " + CurrentSubchapterNumber + "/" +
                         enabledSubchapters.Count;
                ProductProcessingStatus = "Получение складчин из раздела...";
                var productsFromSubchapter =
                    await GetPartialProductsFromSubchapterAsync(subchapter.Id).ConfigureAwait(false);

                var filteredProducts = FiltrationPartialProducts(productsFromSubchapter);

                var productsFromDb = await GetSubchapterProductsFromDbAsync(subchapter.Id).ConfigureAwait(false);

                var products = new List<Product>();
                var goodProductsFromDb = new List<Product>();
                _dbGoodProductsCountOfChapter = 0;

                var productsWithChangedViewsCount = new List<Product>();

                for (int j = 0; j < filteredProducts.Count; j++)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    var product = filteredProducts.ElementAt(j);

                    var productFromDb = FindSimilarDbProduct(productsFromDb, product);

                    if (productFromDb == null)
                    {
                        products.Add(product);
                    }
                    else
                    {
                        if (productFromDb.ViewCount != product.ViewCount)
                        {
                            productsWithChangedViewsCount.Add(product);
                        }

                        //Проверяем складчину из БД
                        if (Filters.IsGoodProduct(productFromDb, FiltersManagerVM.FiltersVM.Model))
                        {
                            ++_dbGoodProductsCountOfChapter;
                            goodProductsFromDb.Add(productFromDb);
                        }
                    }
                }
                await AddToResultsAsync(goodProductsFromDb).ConfigureAwait(false);
                productSync.ProductUpdateInfo(products);
                if (UpdateViewCount)
                {
                    await Db.Products.UpdateViewCountAsync(productsWithChangedViewsCount).ConfigureAwait(false);
                }
            }
        }

        private async Task<List<Product>> GetPartialProductsFromSubchapterAsync(int subchapterId)
        {
            var productsFromSubchapter = new List<Product>();

            if (FiltersManagerVM.FiltersVM.Model.PrefixOpen)
            {
                var openProducts =
                    await _skApi.Chapters.GetPartialProductsFromSubchapterAsync(
                            SkladchikApiClient.ForumsUrl + subchapterId, (int) API.Methods.Products.PrefixId.Open)
                        .ConfigureAwait(false);
                if (openProducts != null)
                {
                    productsFromSubchapter.AddRange(openProducts);
                }
            }

            if (FiltersManagerVM.FiltersVM.Model.PrefixActive)
            {
                var activeProducts =
                    await _skApi.Chapters.GetPartialProductsFromSubchapterAsync(
                            SkladchikApiClient.ForumsUrl + subchapterId, (int) API.Methods.Products.PrefixId.Active)
                        .ConfigureAwait(false);
                if (activeProducts != null)
                {
                    productsFromSubchapter.AddRange(activeProducts);
                }
            }

            if (FiltersManagerVM.FiltersVM.Model.PrefixCompleted)
            {
                var completedProducts =
                    await _skApi.Chapters.GetPartialProductsFromSubchapterAsync(
                            SkladchikApiClient.ForumsUrl + subchapterId, (int) API.Methods.Products.PrefixId.Completed)
                        .ConfigureAwait(false);
                if (completedProducts != null)
                {
                    productsFromSubchapter.AddRange(completedProducts);
                }
            }

            if (FiltersManagerVM.FiltersVM.Model.PrefixAvailable)
            {
                var availableProducts =
                    await _skApi.Chapters.GetPartialProductsFromSubchapterAsync(
                            SkladchikApiClient.ForumsUrl + subchapterId, (int) API.Methods.Products.PrefixId.Available)
                        .ConfigureAwait(false);
                if (availableProducts != null)
                {
                    productsFromSubchapter.AddRange(availableProducts);
                }
            }

            return productsFromSubchapter;
        }

        private List<Product> FiltrationPartialProducts(IEnumerable<Product> productsFromSubchapter)
        {
            return productsFromSubchapter.Where(
                p => Filters.IsGoodProduct(p, FiltersManagerVM.FiltersVM.Model, true)).ToList();
        }

        private static async Task<Dictionary<int, Product>> GetSubchapterProductsFromDbAsync(int subchapterId)
        {
            Dictionary<int, Product> productsFromDb;
            using (var db = new DbContext())
            {
                var request = db.Products.AsNoTracking().Where(x => x.SubchapterId == subchapterId)
                    .Include(x => x.Creator)
                    .Include(x => x.Organizer)
                    .Include(x => x.Prefix)
                    .Include(x => x.Chapter)
                    .Include(x => x.Subсhapter);
                productsFromDb = await request.ToDictionaryAsync(p => p.Id).ConfigureAwait(false);

                foreach (var product in productsFromDb.Values)
                {
                    if (Db.Tags.ProductsAndTags.ContainsKey(product.Id))
                        product.Tags = Db.Tags.ProductsAndTags[product.Id];
                }
            }
            return productsFromDb;
        }

        private static Product FindSimilarDbProduct(IReadOnlyDictionary<int, Product> productsFromDb, Product product)
        {
            Product productFromDb = null;
            if (productsFromDb.ContainsKey(product.Id))
            {
                productFromDb = productsFromDb[product.Id];
            }

            if (productFromDb == null || product.DateUpdate > productFromDb.DateUpdate)
            {
                return null;
            }

            return productFromDb;
        }

        #endregion
    }
}