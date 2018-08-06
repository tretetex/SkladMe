using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SkDAL;
using SkDAL.Model;
using SkladMe.API;

namespace SkladMe.Parsers
{
    public class ProductEventArgs : EventArgs
    {
        public List<Product> Products { get; set; }
        public int AllProductsCount { get; set; }
        public int ProductProcessed { get; set; }
    }

    public class ProductParser
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<ProductEventArgs> ProductInfoUpdated; 

        private readonly SkladchikApiClient _skApi;
        private CancellationToken _token;
        private ProductEventArgs _productEventArgs;

        public ProductParser(SkladchikApiClient skApi, CancellationToken token)
        {
            _token = token;
            _skApi = skApi;
            _productEventArgs = new ProductEventArgs()
            {
                Products = new List<Product>()
            };
        }

        private BlockingCollection<Product> _productForUpdate;
        private BlockingCollection<Product> _updatedProducts;

        public void ProductUpdateInfo(List<Product> products)
        {
            if (products.Count == 0) return;
            _productForUpdate = new BlockingCollection<Product>();
            _updatedProducts = new BlockingCollection<Product>();

            _productForProcessCount = products.Count;
            _productEventArgs.AllProductsCount = products.Count;

            var consumers = new Task[10];
            for (var i = 0; i < consumers.Length; i++)
            {
                consumers[i] = Task.Run(Updater, _token);
            }

            foreach (var product in products)
            {
                _productForUpdate.Add(product, _token);
            }

            _productForUpdate.CompleteAdding();

            Task.Run(ConsumerAdd, _token).Wait(_token);
        }

        async Task Updater()
        {
            foreach (var product in _productForUpdate.GetConsumingEnumerable())
            {
                _token.ThrowIfCancellationRequested();
                var updatedProduct = await _skApi.Products.GetByIdAsync(product.Id, product.DateUpdate, product.ViewCount,
                    product.Important).ConfigureAwait(false);
                _updatedProducts.Add(updatedProduct);
            }
        }

        private int _productForProcessCount;

        async Task ConsumerAdd()
        {
            var productToDb = new List<Product>();
            int updatedProductsCount = 0;
            foreach (var updatedProduct in _updatedProducts.GetConsumingEnumerable())
            {
                ++updatedProductsCount;
                productToDb.Add(updatedProduct);
                _productEventArgs.Products.Add(updatedProduct);
                if (_productEventArgs.Products.Count % 10 != 0 && updatedProductsCount != _productForProcessCount && !_token.IsCancellationRequested)
                {
                    continue;
                }

                _productEventArgs.AllProductsCount = _productForProcessCount;
                _productEventArgs.ProductProcessed = updatedProductsCount;
                ProductInfoUpdated?.Invoke(this, _productEventArgs);
                _productEventArgs.Products.Clear();


                if (productToDb.Count % 2000 == 0 || updatedProductsCount == _productForProcessCount || _token.IsCancellationRequested)
                {
                    await Db.Products.AddOrUpdateAsync(productToDb.Where(p => p != null).ToList()).ConfigureAwait(false);
                    productToDb.Clear();
                }

                if (updatedProductsCount == _productForProcessCount)
                {
                    _updatedProducts.CompleteAdding();
                }
                _token.ThrowIfCancellationRequested();
            }
        }
    }
}