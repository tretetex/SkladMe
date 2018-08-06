using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SkDAL.Base;
using SkDAL.Model;

namespace SkDAL.Services
{
    class CategoryService : Service<Category>
    {
        private static List<ProductCategories> _productsAndCategories;

        public CategoryService()
        {
            LoadProductsAndCategories();
        }

        public IReadOnlyCollection<ProductCategories> ProductAndCategories => _productsAndCategories;

        private void LoadProductsAndCategories()
        {
            using (var db = new DbContext())
            {
                _productsAndCategories = new List<ProductCategories>(db.ProductCategories.AsNoTracking().ToList());
            }
        }

        public override void AddOrUpdate(Category category)
        {
            using (var db = new DbContext())
            {
                var categoryFromDb = db.Categories.Find(category.Id);

                if (categoryFromDb == null ||
                    categoryFromDb.ParentId != category.ParentId || categoryFromDb.SortOrder != category.SortOrder)
                {
                    var nextCategories = db.Categories.Where(x =>
                        x.ParentId == category.ParentId &&
                        x.SortOrder >= category.SortOrder).OrderBy(x => x.SortOrder).ToList();

                    if (nextCategories.Count > 0 && nextCategories[0].SortOrder <= category.SortOrder)
                    {
                        nextCategories[0].SortOrder = category.SortOrder + 100;

                        for (var i = 1; i < nextCategories.Count; i++)
                        {
                            if (nextCategories[i].SortOrder <= nextCategories[i - 1].SortOrder)
                            {
                                nextCategories[i].SortOrder = nextCategories[i - 1].SortOrder + 100;
                            }
                        }
                        db.SaveChanges();
                    }
                }

                base.AddOrUpdate(category);
            }
        }

        public void AddProducts(Category category, List<Product> products)
        {
            using (var db = new DbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                var productCategories = db.ProductCategories.Where(c => c.CategoryId == category.Id).ToList();
                var newProductCategories = new List<ProductCategories>();

                foreach (var product in products)
                {
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

                db.Configuration.AutoDetectChangesEnabled = true;
                db.ProductCategories.AddRange(newProductCategories);
                db.SaveChanges();

                _productsAndCategories.AddRange(newProductCategories);
            }
        }

        public async Task AddProductsAsync(Category category, List<Product> products)
        {
            await Task.Run(() => AddProducts(category, products)).ConfigureAwait(false);
        }

        public void DeleteProducts(Category category, List<Product> products)
        {
            var count = 0;

            using (var db = new DbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                var productCategories = db.ProductCategories.Where(c => c.CategoryId == category.Id).ToList();
                var deletedProductCategories = new List<ProductCategories>();

                foreach (var product in products)
                {
                    var productCategory = productCategories.Find(pc => pc.ProductId == product.Id);
                    if (productCategory != null)
                    {
                        db.Entry(productCategory).State = EntityState.Deleted;
                    }
                    deletedProductCategories.Add(productCategory);
                }

                db.Configuration.AutoDetectChangesEnabled = true;
                db.SaveChanges();

                foreach (var deletedProductCategory in deletedProductCategories)
                {
                    _productsAndCategories.Remove(_productsAndCategories.FirstOrDefault(
                        x =>
                            x.CategoryId == deletedProductCategory.CategoryId &
                            x.ProductId == deletedProductCategory.ProductId));
                }
            }
        }

        public async Task DeleteProductsAsync(Category category, List<Product> products)
        {
            await Task.Run(() => DeleteProducts(category, products)).ConfigureAwait(false);
        }

        public List<Product> GetProductsFromCategory(Category category, int startFrom, int count = 50)
        {
            using (var db = new DbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                var products = new List<Product>();
                var productCategories = _productsAndCategories.Where(c => c.CategoryId == category.Id)
                    .Skip(startFrom)
                    .Take(count)
                    .ToList();

                foreach (var productCategory in productCategories)
                {
                    var product = db.Products.Where(p => p.Id == productCategory.ProductId)
                        .Include(p => p.Prefix)
                        .Include(p => p.Chapter)
                        .Include(p => p.Subсhapter)
                        .Include(p => p.Creator)
                        .Include(p => p.Organizer)
                        .First();

                    products.Add(product);
                }
                return products;
            }
        }

        public async Task<ICollection<Product>> GetProductsFromCategoryAsync(Category category, int startFrom,
            int count = 50)
        {
            return await Task.Run(() => GetProductsFromCategory(category, startFrom, count)).ConfigureAwait(false);
        }

        public ICollection<Product> GetAllProducts(Category category)
        {
            using (var db = new DbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                var products = new List<Product>();
                var productCategories = _productsAndCategories.Where(c => c.CategoryId == category.Id).ToList();

                foreach (var productCategory in productCategories)
                {
                    var product = db.Products.Where(p => p.Id == productCategory.ProductId)
                        .Include(p => p.Prefix)
                        .Include(p => p.Chapter)
                        .Include(p => p.Subсhapter)
                        .Include(p => p.Creator)
                        .Include(p => p.Organizer)
                        .First();

                    products.Add(product);
                }
                return products;
            }
        }

        public async Task<ICollection<Product>> GetAllProductsAsync(Category category)
        {
            return await Task.Run(() => GetAllProducts(category)).ConfigureAwait(false);
        }

        public override void Delete(Category entity)
        {
            base.Delete(entity);
            _productsAndCategories.RemoveAll(x => x.CategoryId == entity.Id);
        }
    }
}