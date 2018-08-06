using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SkDAL.Base
{
    abstract class Service<T> where T: BaseModel
    {
        public virtual void AddOrUpdate(IEnumerable<T> entities)
        {
            using (var db = new DbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                foreach (var entity in entities)
                {
                    var localEntity = db.Set<T>().Local.FirstOrDefault(d => d.Id == entity.Id);
                    if (localEntity != null)
                    {
                        db.Entry(localEntity).State = EntityState.Detached;
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        var dbEntity = db.Set<T>().FirstOrDefault(x => x.Id == entity.Id);
                        if (dbEntity != null)
                        {
                            db.Entry(dbEntity).State = EntityState.Detached;
                            db.Entry(entity).State = EntityState.Modified;
                        }
                        else
                        {
                            db.Entry(entity).State = EntityState.Added;
                        }
                    }
                }
                
                db.SaveChanges();
                db.Configuration.AutoDetectChangesEnabled = true;
                db.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        public virtual async Task AddOrUpdateAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                AddOrUpdate(entities);
            }).ConfigureAwait(false);
        }

        public virtual T Find(int id)
        {
            using (var db = new DbContextWrapper())
            {
                return db.Context.Set<T>().FirstOrDefault(x => x.Id == id);
            }
        }

        public virtual async Task<T> FindAsync(int id)
        {
            return await Task.Run(() => Find(id)).ConfigureAwait(false);
        }

        public virtual IList<T> All()
        {
            using (var db = new DbContextWrapper())
            {
                return db.Context.Set<T>().ToList();
            }
        }
    
        public virtual async Task<IList<T>> AllAsync()
        {
            return await Task.Run(() => All()).ConfigureAwait(false);
        }

        public virtual IEnumerable<T> Intersect(IEnumerable<T> source)
        {
            using (var db = new DbContextWrapper())
            {
                return db.Context.Set<T>().Intersect(source);
            }
        }

        public virtual async Task<IEnumerable<T>> IntersectAsync(IEnumerable<T> source)
        {
            return await Task.Run(() => All()).ConfigureAwait(false);
        }

        public virtual void AddOrUpdate(T entity)
        {
            using (var db = new DbContextWrapper())
            {
                var dbEntity = db.Context.Set<T>().FirstOrDefault(x => x.Id == entity.Id);
                if (dbEntity == null)
                {
                    db.Context.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    db.Context.Entry(dbEntity).State = EntityState.Detached;
                    db.Context.Entry(entity).State = EntityState.Modified;
                }
            }
        }

        public virtual async Task AddOrUpdateAsync(T entity)
        {
            await Task.Run(() =>
            {
                AddOrUpdate(entity);
            }).ConfigureAwait(false);
        }

        public virtual void Delete(T entity)
        {
            using (var ctx = new DbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;

                var dbEntity = ctx.Set<T>().Find(entity.Id);
                if (dbEntity != null)
                {
                    ctx.Set<T>().Remove(dbEntity);
                    ctx.SaveChanges();
                }
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            await Task.Run(() =>
            {
                Delete(entity);
            }).ConfigureAwait(false);
        }
    }
}
