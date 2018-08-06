using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SkDAL.Base;
using SkDAL.Model;

namespace SkDAL.Services
{
    class TagService : Service<Tag>
    {
        public override void AddOrUpdate(Tag entity)
        {
            using (var db = new DbContextWrapper())
            {
                var dbEntity = db.Context.Tags.FirstOrDefault(x => x.Title == entity.Title);
                if (dbEntity == null)
                {
                    db.Context.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    db.Context.Entry(dbEntity).State = EntityState.Detached;
                }
            }
        }

        public override void AddOrUpdate(IEnumerable<Tag> entities)
        {
            using (var db = new DbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                foreach (var entity in entities)
                {
                    entity.Title = entity.Title.Trim();

                    var localEntity = db.Tags.Local.FirstOrDefault(d => d.Title == entity.Title);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        var dbEntity = db.Tags.FirstOrDefault(x => x.Title == entity.Title);
                        if (dbEntity == null)
                        {
                            db.Entry(entity).State = EntityState.Added;
                        }
                        else
                        {
                            entity.Id = dbEntity.Id;
                        }
                    }
                }

                db.SaveChanges();
                db.Configuration.AutoDetectChangesEnabled = true;
                db.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        public Tag Find(string value)
        {
            value = value.Trim();

            using (var db = new DbContextWrapper())
            {
                return db.Context.Tags.FirstOrDefault(x => x.Title == value);
            }
        }

        public virtual async Task<Tag> FindAsync(string value)
        {
            return await Task.Run(() => Find(value)).ConfigureAwait(false);
        }
    }
}
