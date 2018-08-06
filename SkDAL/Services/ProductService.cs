using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SkDAL.Model;

namespace SkDAL.Services
{
    public class ProductService : Service<Product>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Tag> _tags = new Dictionary<string, Tag>();
        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();

        public async Task<int> Count(int subchapterId)
        {
            using (var ctx = new DbContext())
            {
                return await ctx.Products.AsNoTracking().CountAsync(x => x.SubchapterId == subchapterId)
                    .ConfigureAwait(false);
            }
        }

        public async Task<List<Product>> GetAsync(int subchapterId, int startFrom, int count)
        {
            using (var ctx = new DbContext())
            {
                var products = await ctx.Products.AsNoTracking().Where(x => x.SubchapterId == subchapterId)
                    .OrderBy(x => x.Id)
                    .Skip(() => startFrom)
                    .Take(() => count)
                    .Include(x => x.Creator)
                    .Include(x => x.Organizer)
                    .Include(x => x.Prefix)
                    .Include(x => x.Chapter)
                    .Include(x => x.Subсhapter)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var product in products)
                {
                    if (Db.Tags.ProductsAndTags.ContainsKey(product.Id))
                        product.Tags = Db.Tags.ProductsAndTags[product.Id];
                }

                return products;
            }
        }

        public async Task UpdateViewCountAsync(List<Product> products)
        {
            using (var ctx = new DbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;

                foreach (var product in products)
                {
                    var productForUpdate = new Product
                    {
                        Id = product.Id,
                        ViewCount = product.ViewCount,
                        Title = product.Title,
                        CreatorId = product.CreatorId
                    };

                    ctx.Entry(productForUpdate).State = EntityState.Unchanged;
                    ctx.Entry(productForUpdate).Property(x => x.ViewCount).IsModified = true;
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public void UpdateColor(Product product)
        {
            var dbProduct = Find(product.Id);
            if (dbProduct != null)
            {
                dbProduct.Color = product.Color;
                AddOrUpdateAsync(dbProduct).Wait();
            }
        }

        public async Task UpdateColorAsync(Product product)
        {
            await Task.Run(() => UpdateColor(product)).ConfigureAwait(false);
        }

        public void UpdateNote(IList<Product> products)
        {
            foreach (var product in products)
            {
                var dbProduct = Find(product.Id);
                if (dbProduct != null)
                {
                    dbProduct.Note = product.Note;
                    AddOrUpdateAsync(dbProduct).Wait();
                }
            }
        }

        public async Task UpdateNoteAsync(IList<Product> products)
        {
            await Task.Run(() => UpdateNote(products)).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Product product, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                var productFromDb = await context.Products.FirstOrDefaultAsync(x => x.Id == product.Id, token)
                    .ConfigureAwait(false);
                if (productFromDb != null)
                {
                    context.Products.Remove(productFromDb);
                }

                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }

            if (Db.Tags.ProductsAndTags.ContainsKey(product.Id))
                Db.Tags.ProductsAndTags.Remove(product.Id);
        }

        public async Task AddOrUpdateAsync(List<Product> products, CancellationToken token = default(CancellationToken))
        {
            RemoveDuplicateProducts(products);

            await UpdateTagsAsync(products, token).ConfigureAwait(false);
            await UpdateUsersAsync(products, token).ConfigureAwait(false);

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var prefixes = new Dictionary<int, Prefix>();
                var chapters = new Dictionary<int?, Chapter>();
                var subchapters = new Dictionary<int?, Subсhapter>();

                foreach (var product in products)
                {
                    token.ThrowIfCancellationRequested();

                    if (product.Prefix != null)
                    {
                        if (prefixes.ContainsKey(product.PrefixId))
                        {
                            product.Prefix = prefixes[product.PrefixId];
                            context.Entry(product.Prefix).State = EntityState.Unchanged;
                        }
                        else
                        {
                            context.Entry(product.Prefix).State = EntityState.Unchanged;
                            prefixes.Add(product.PrefixId, product.Prefix);
                        }
                    }

                    if (product.Chapter != null)
                    {
                        if (chapters.ContainsKey(product.ChapterId))
                        {
                            product.Chapter = chapters[product.ChapterId];
                            context.Entry(product.Chapter).State = EntityState.Unchanged;
                        }
                        else
                        {
                            context.Entry(product.Chapter).State = EntityState.Unchanged;
                            chapters.Add(product.ChapterId, product.Chapter);
                        }
                    }

                    if (product.Subсhapter != null)
                    {
                        if (subchapters.ContainsKey(product.SubchapterId))
                        {
                            product.Subсhapter = subchapters[product.SubchapterId];
                            context.Entry(product.Subсhapter).State = EntityState.Unchanged;
                        }
                        else
                        {
                            context.Entry(product.Subсhapter).State = EntityState.Unchanged;
                            subchapters.Add(product.SubchapterId, product.Subсhapter);
                        }
                    }

                    if (product.Organizer != null)
                    {
                        product.OrganizerId = product.Organizer.Id;
                        context.Entry(product.Organizer).State = EntityState.Unchanged;
                    }

                    if (product.Creator != null)
                    {
                        product.CreatorId = product.Creator.Id;
                        context.Entry(product.Creator).State = EntityState.Unchanged;
                    }

                    if (product.MembersAsMain != null)
                    {
                        foreach (var user in product.MembersAsMain)
                        {
                            context.Entry(user).State = EntityState.Unchanged;
                        }
                    }

                    if (product.MembersAsReserve != null)
                    {
                        foreach (var user in product.MembersAsReserve)
                        {
                            context.Entry(user).State = EntityState.Unchanged;
                        }
                    }

                    if (product.UsersAsAuthor != null)
                    {
                        foreach (var user in product.UsersAsAuthor)
                        {
                            context.Entry(user).State = EntityState.Unchanged;
                        }
                    }

                    var dbProduct = await context.Products.FindAsync(token, product.Id).ConfigureAwait(false);

                    if (dbProduct != null)
                    {
                        context.Entry(dbProduct).State = EntityState.Detached;
                        product.Note = dbProduct.Note;
                        product.Color = dbProduct.Color;
                    }

                    context.Entry(product).State = dbProduct != null ? EntityState.Modified : EntityState.Added;
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }

            await Db.Tags.UpdateProductsAndTagsAsync(products, token).ConfigureAwait(false);
        }

        private async Task UpdateTagsAsync(IEnumerable<Product> products,
            CancellationToken token = default(CancellationToken))
        {
            _tags.Clear();

            foreach (var product in products)
            {
                token.ThrowIfCancellationRequested();

                if (product.Tags == null)
                    continue;
                var tags = (List<Tag>) product.Tags;
                ExtractUniqueTags(ref tags);
            }

            await Db.Tags.AddOrUpdateAsync(_tags.Values, token).ConfigureAwait(false);
        }

        private void ExtractUniqueTags(ref List<Tag> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                if (!_tags.ContainsKey(tags[i].Title))
                {
                    _tags.Add(tags[i].Title, tags[i]);
                }
                tags[i] = _tags[tags[i].Title];
            }
        }

        private async Task UpdateUsersAsync(IEnumerable<Product> products,
            CancellationToken token = default(CancellationToken))
        {
            _users.Clear();

            foreach (var product in products)
            {
                token.ThrowIfCancellationRequested();

                ExtractUniqueUsers(product);
            }

            await Db.Users.AddOrUpdateAsync(_users.Values, token).ConfigureAwait(false);
        }

        private void ExtractUniqueUsers(Product product)
        {
            if (product.Creator != null)
            {
                if (!_users.ContainsKey(product.Creator.Id))
                {
                    _users.Add(product.Creator.Id, product.Creator);
                }
                else
                {
                    product.Creator = _users[product.Creator.Id];
                }
            }
            if (product.Organizer != null)
            {
                if (!_users.ContainsKey(product.Organizer.Id))
                {
                    _users.Add(product.Organizer.Id, product.Organizer);
                }
                else
                {
                    product.Organizer = _users[product.Organizer.Id];
                }
            }
            if (product.UsersAsAuthor != null)
            {
                var users = (List<User>) product.UsersAsAuthor;
                ExtractUniqueUsersFromList(users);
            }
            if (product.MembersAsMain != null)
            {
                var users = (List<User>) product.MembersAsMain;
                ExtractUniqueUsersFromList(users);
            }
            if (product.MembersAsReserve != null)
            {
                var users = (List<User>) product.MembersAsReserve;
                ExtractUniqueUsersFromList(users);
            }
        }

        private void ExtractUniqueUsersFromList(IList<User> users)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (!_users.ContainsKey(users[i].Id))
                {
                    _users.Add(users[i].Id, users[i]);
                }
                else
                {
                    users[i] = _users[users[i].Id];
                }
            }
        }

        private void RemoveDuplicateProducts(List<Product> products)
        {
            var tempDict = new Dictionary<int, Product>();
            foreach (var product in products)
            {
                if (!tempDict.ContainsKey(product.Id))
                    tempDict.Add(product.Id, product);
            }
            products.Clear();
            products.AddRange(tempDict.Values);
        }
    }
}