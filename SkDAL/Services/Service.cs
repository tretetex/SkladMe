using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Services
{
    public partial class Service<TEntity> where TEntity: class, IEntity
    {
        public virtual async Task AddOrUpdateAsync(IEnumerable<TEntity> entities, CancellationToken token = default(CancellationToken))
        {
            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var entity in entities)
                {
                    token.ThrowIfCancellationRequested();

                    var count = await context.Set<TEntity>().AsNoTracking().CountAsync(x => x.Id == entity.Id, token).ConfigureAwait(false);
                    var entityExists = count > 0;
                    context.Entry(entity).State = entityExists ? EntityState.Modified : EntityState.Added;
                }

                context.Configuration.AutoDetectChangesEnabled = true;
                context.Configuration.ValidateOnSaveEnabled = true;

                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }

        public virtual async Task AddOrUpdateAsync(TEntity entity, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;

                var count = await context.Set<TEntity>().AsNoTracking().CountAsync(x => x.Id == entity.Id, token).ConfigureAwait(false);
                var entityExists = count > 0;
                context.Entry(entity).State = entityExists ? EntityState.Modified : EntityState.Added;

                context.Configuration.AutoDetectChangesEnabled = true;
                context.Configuration.ValidateOnSaveEnabled = true;

                await context.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }

        public virtual TEntity Find(int id)
        {
            using (var context = new DbContext())
            {
                return context.Set<TEntity>().AsNoTracking()
                    .FirstOrDefault(x => x.Id == id);
            }
        }

        public virtual async Task<IList<TEntity>> AllAsync(CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                return await context.Set<TEntity>().ToListAsync(token).ConfigureAwait(false);
            }
        }
    
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            using (var context = new DbContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                var dbEntity = await context.Set<TEntity>().FindAsync(token, entity.Id).ConfigureAwait(false);
                if (dbEntity != null)
                {
                    context.Set<TEntity>().Remove(dbEntity);
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }
        }
    }
}
