using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkDAL.Model;

namespace SkDAL.Services
{
    public class CategoryService : Service<Category>
    {
        private static List<ProductCategories> _productsAndCategories;
        public IReadOnlyCollection<ProductCategories> ProductAndCategories => _productsAndCategories;

        public CategoryService()
        {
            LoadProductsAndCategories();
        }

        private void LoadProductsAndCategories()
        {
            using (var context = new DbContext())
            {
                var productsAndCategories = context.ProductCategories.AsNoTracking().ToList();
                _productsAndCategories = new List<ProductCategories>(productsAndCategories);
            }
        }

        public override async Task AddOrUpdateAsync(Category category,
            CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                var categoryFromDb = await context.Categories.FindAsync(token, category.Id).ConfigureAwait(false);

                if (categoryFromDb == null ||
                    categoryFromDb.ParentId != category.ParentId || categoryFromDb.SortOrder != category.SortOrder)
                {
                    var nextCategories = await context.Categories.Where(x =>
                            x.ParentId == category.ParentId &&
                            x.SortOrder >= category.SortOrder).OrderBy(x => x.SortOrder)
                        .ToListAsync(token).ConfigureAwait(false);

                    if (nextCategories.Count > 0 && nextCategories[0].SortOrder <= category.SortOrder)
                    {
                        nextCategories[0].SortOrder = category.SortOrder + 100;

                        for (var i = 1; i < nextCategories.Count; i++)
                        {
                            token.ThrowIfCancellationRequested();

                            if (nextCategories[i].SortOrder <= nextCategories[i - 1].SortOrder)
                            {
                                nextCategories[i].SortOrder = nextCategories[i - 1].SortOrder + 100;
                            }
                        }

                        await context.SaveChangesAsync(token).ConfigureAwait(false);
                    }
                }

                await base.AddOrUpdateAsync(category, token).ConfigureAwait(false);
            }
        }

        public async Task AddProductsAsync(Category category, List<Product> products,
            CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var productCategories = await context.ProductCategories.Where(c => c.CategoryId == category.Id)
                    .ToListAsync(token).ConfigureAwait(false);
                var newProductCategories = new List<ProductCategories>();

                foreach (var product in products)
                {
                    token.ThrowIfCancellationRequested();

                    var isContainsProduct = productCategories.Find(pc => pc.ProductId == product.Id) != null;
                    if (!isContainsProduct)
                    {
                        newProductCategories.Add(new ProductCategories
                        {
                            CategoryId = category.Id,
                            ProductId = product.Id
                        });
                    }
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                context.ProductCategories.AddRange(newProductCategories);

                _productsAndCategories.AddRange(newProductCategories);
                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }

        public async Task DeleteProductsAsync(Category category, List<Product> products,
            CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var productCategories = await context.ProductCategories.Where(c => c.CategoryId == category.Id)
                    .ToListAsync(token).ConfigureAwait(false);
                var deletedProductCategories = new List<ProductCategories>();

                foreach (var product in products)
                {
                    var productCategory = productCategories.Find(pc => pc.ProductId == product.Id);
                    if (productCategory != null)
                    {
                        context.Entry(productCategory).State = EntityState.Deleted;
                    }
                    deletedProductCategories.Add(productCategory);
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                await context.SaveChangesAsync(token).ConfigureAwait(false);

                foreach (var deletedProductCategory in deletedProductCategories)
                {
                    _productsAndCategories.Remove(_productsAndCategories.FirstOrDefault(
                        x =>
                            x.CategoryId == deletedProductCategory.CategoryId &
                            x.ProductId == deletedProductCategory.ProductId));
                }
            }
        }

        public async Task<List<Product>> GetProductsFromCategoryAsync(Category category, int startFrom, int count = 50,
            CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var products = new List<Product>();
                var productCategories = _productsAndCategories.Where(c => c.CategoryId == category.Id)
                    .Skip(startFrom)
                    .Take(count)
                    .ToList();

                foreach (var productCategory in productCategories)
                {
                    token.ThrowIfCancellationRequested();

                    var product = await context.Products.Where(p => p.Id == productCategory.ProductId)
                        .Include(p => p.Prefix)
                        .Include(p => p.Chapter)
                        .Include(p => p.Subсhapter)
                        .Include(p => p.Creator)
                        .Include(p => p.Organizer)
                        .FirstAsync(token).ConfigureAwait(false);

                    products.Add(product);
                }
                return products;
            }
        }

        public async Task<ICollection<Product>> GetAllProductsAsync(Category category,
            CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var products = new List<Product>();
                var productCategories = _productsAndCategories.Where(c => c.CategoryId == category.Id).ToList();

                foreach (var productCategory in productCategories)
                {
                    token.ThrowIfCancellationRequested();

                    var product = await context.Products.Where(p => p.Id == productCategory.ProductId)
                        .Include(p => p.Prefix)
                        .Include(p => p.Chapter)
                        .Include(p => p.Subсhapter)
                        .Include(p => p.Creator)
                        .Include(p => p.Organizer)
                        .FirstAsync(token).ConfigureAwait(false);

                    if (Db.Tags.ProductsAndTags.ContainsKey(product.Id))
                        product.Tags = Db.Tags.ProductsAndTags[product.Id];
                        
                    products.Add(product);
                }
                return products;
            }
        }

        public override async Task DeleteAsync(Category entity, CancellationToken token = default(CancellationToken))
        {
            await base.DeleteAsync(entity, token).ConfigureAwait(false);
            _productsAndCategories.RemoveAll(x => x.CategoryId == entity.Id);
        }
    }
}