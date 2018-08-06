using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkDAL.Model;

namespace SkDAL.Services
{
    public class TagService : Service<Tag>
    {
        public readonly Dictionary<int, List<Tag>> ProductsAndTags = new Dictionary<int, List<Tag>>();

        public TagService()
        {
            LoadProductsAndTags();
        }

        public void LoadProductsAndTags()
        {
            using (var context = new DbContext())
            {
                var productsAndTags = context.ProductTags.AsNoTracking()
                    .Include(x => x.Tag)
                    .ToList();

                ProductsAndTags.Clear();

                foreach (var productsAndTag in productsAndTags)
                {
                    if (!ProductsAndTags.ContainsKey(productsAndTag.ProductId))
                        ProductsAndTags.Add(productsAndTag.ProductId, new List<Tag>());
                    ProductsAndTags[productsAndTag.ProductId].Add(productsAndTag.Tag);
                }
            }
        }

        public async Task UpdateProductsAndTagsAsync(List<Product> products,
            CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var newProductTags = new List<ProductTags>();

                foreach (var product in products)
                {
                    token.ThrowIfCancellationRequested();

                    if (!ProductsAndTags.ContainsKey(product.Id) && product.Tags != null)
                    {
                        var tags = new List<Tag>();

                        foreach (var tag in product.Tags)
                        {
                            tags.Add(tag);
                            var productTag = new ProductTags
                            {
                                TagId = tag.Id,
                                ProductId = product.Id
                            };
                            newProductTags.Add(productTag);
                        }
                        ProductsAndTags.Add(product.Id, tags);
                    }
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                context.ProductTags.AddRange(newProductTags);

                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }

        public override async Task AddOrUpdateAsync(Tag entity, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;

                var dbEntity = await context.Tags.FirstOrDefaultAsync(x => x.Title == entity.Title, token)
                    .ConfigureAwait(false);
                if (dbEntity == null)
                {
                    context.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    context.Entry(dbEntity).State = EntityState.Detached;
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                context.Configuration.ValidateOnSaveEnabled = true;

                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }

        public override async Task AddOrUpdateAsync(IEnumerable<Tag> entities,
            CancellationToken token = default(CancellationToken))
        {
            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var entity in entities)
                {
                    token.ThrowIfCancellationRequested();
                    entity.Title = entity.Title.Trim();

                    var localEntity = context.Tags.Local.FirstOrDefault(d => d.Title == entity.Title);
                    if (localEntity != null)
                    {
                        context.Entry(localEntity).State = EntityState.Detached;
                        context.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        var dbEntity = await context.Tags.FirstOrDefaultAsync(x => x.Title == entity.Title, token)
                            .ConfigureAwait(false);
                        if (dbEntity == null)
                        {
                            context.Entry(entity).State = EntityState.Added;
                        }
                        else
                        {
                            entity.Id = dbEntity.Id;
                        }
                    }
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                context.Configuration.ValidateOnSaveEnabled = true;

                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }

        public async Task<Tag> FindAsync(string value, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            value = value.Trim();

            using (var context = new DbContext())
            {
                return await context.Tags.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Title == value, token).ConfigureAwait(false);
            }
        }
    }
}