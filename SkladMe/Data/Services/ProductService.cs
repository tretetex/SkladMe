using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SkDAL.Base;
using SkDAL.Model;

namespace SkDAL.Services
{
    class ProductService : Service<Product>
    {
        private readonly Dictionary<string, Tag> _tags = new Dictionary<string, Tag>();
        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();

        public void UpdateColor(Product product)
        {
            var dbProduct = Find(product.Id);
            if (dbProduct != null)
            {
                dbProduct.Color = product.Color;
                AddOrUpdate(dbProduct);
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
                    AddOrUpdate(dbProduct);
                }
            }
        }

        public async Task UpdateNoteAsync(IList<Product> products)
        {
            await Task.Run(() => UpdateNote(products)).ConfigureAwait(false);
        }

        public void Remove(Product product)
        {
            using (var db = new DbContext())
            {
                var productFromDb = db.Products.FirstOrDefault(x => x.Id == product.Id);
                if (productFromDb != null)
                {
                    db.Products.Remove(productFromDb);
                }

                db.SaveChanges();
            }
        }

        public async Task RemoveAsync(Product product)
        {
            await Task.Run(() => Remove(product)).ConfigureAwait(false);
        }

        public async Task AddOrUpdateAsync(List<Product> products)
        {
            await UpdateTagsAsync(products).ConfigureAwait(false);
            await UpdateUsersAsync(products).ConfigureAwait(false);
            await Task.Run(() =>
            {
                using (DbContext db = new DbContext())
                {
                    try
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        var prefixes = new Dictionary<int, Prefix>();
                        var chapters = new Dictionary<int?, Chapter>();
                        var subchapters = new Dictionary<int?, Subсhapter>();

                        foreach (var product in products)
                        {
                            if (product.Prefix != null)
                            {
                                if (prefixes.ContainsKey(product.PrefixId))
                                {
                                    product.Prefix = prefixes[product.PrefixId];
                                }
                                else
                                {
                                    db.Entry(product.Prefix).State = EntityState.Unchanged;
                                    prefixes.Add(product.PrefixId, product.Prefix);
                                }
                            }

                            if (product.Chapter != null && product.ChapterId != null)
                            {
                                if (chapters.ContainsKey(product.ChapterId))
                                {
                                    product.Chapter = chapters[product.ChapterId];
                                }
                                else
                                {
                                    db.Entry(product.Chapter).State = EntityState.Unchanged;
                                    chapters.Add(product.ChapterId, product.Chapter);
                                }
                            }

                            if (product.Subсhapter != null)
                            {
                                if (subchapters.ContainsKey(product.SubchapterId))
                                {
                                    product.Subсhapter = subchapters[product.SubchapterId];
                                }
                                else
                                {
                                    db.Entry(product.Subсhapter).State = EntityState.Unchanged;
                                    subchapters.Add(product.SubchapterId, product.Subсhapter);
                                }
                            }

                            if (product.Organizer != null)
                            {
                                product.OrganizerId = product.Organizer.Id;
                                db.Entry(product.Organizer).State = EntityState.Unchanged;
                            }

                            if (product.Creator != null)
                            {
                                product.CreatorId = product.Creator.Id;
                                db.Entry(product.Creator).State = EntityState.Unchanged;
                            }

                            if (product.MembersAsMain != null)
                            {
                                foreach (var user in product.MembersAsMain)
                                {
                                    db.Entry(user).State = EntityState.Unchanged;
                                }
                            }

                            if (product.MembersAsReserve != null)
                            {
                                foreach (var user in product.MembersAsReserve)
                                {
                                    db.Entry(user).State = EntityState.Unchanged;
                                }
                            }

                            if (product.UsersAsAuthor != null)
                            {
                                foreach (var user in product.UsersAsAuthor)
                                {
                                    db.Entry(user).State = EntityState.Unchanged;
                                }
                            }

                            if (product.Tags != null)
                            {
                                foreach (var tag in product.Tags)
                                {
                                    db.Entry(tag).State = EntityState.Unchanged;
                                }
                            }

                            var dbProduct =
                                db.Products.AsNoTracking().FirstOrDefault(p => p.Id == product.Id);
                            if (dbProduct != null)
                            {
                                db.Entry(dbProduct).State = EntityState.Detached;
                                product.Note = dbProduct.Note;
                                product.Color = dbProduct.Color;
                            }

                            db.Entry(product).State = dbProduct != null ? EntityState.Modified : EntityState.Added;
                        }

                        db.Configuration.AutoDetectChangesEnabled = true;
                        db.SaveChanges();
                    }
                    catch (System.Exception e)
                    {
                        throw;
                    }
                }
            }).ConfigureAwait(false);
        }

        private async Task UpdateTagsAsync(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                if (product.Tags == null)
                    continue;
                var tags = (List<Tag>)product.Tags;
                ExtractUniqueTags(ref tags);
            }

            await Db.Tags.AddOrUpdateAsync(_tags.Values).ConfigureAwait(false);
            _tags.Clear();
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

        private async Task UpdateUsersAsync(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                ExtractUniqueUsers(product);
            }

            await Db.Users.AddOrUpdateAsync(_users.Values).ConfigureAwait(false);
            _users.Clear();
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
                var users = (List<User>)product.UsersAsAuthor;
                ExtractUniqueUsersFromList(ref users);
            }
            if (product.MembersAsMain != null)
            {
                var users = (List<User>)product.MembersAsMain;
                ExtractUniqueUsersFromList(ref users);
            }
            if (product.MembersAsReserve != null)
            {
                var users = (List<User>)product.MembersAsReserve;
                ExtractUniqueUsersFromList(ref users);
            }
        }

        private void ExtractUniqueUsersFromList(ref List<User> users)
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
    }
}