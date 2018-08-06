using System.Threading.Tasks;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Services
{
    public partial class Service<TEntity> where TEntity: class, IEntity
    {
        //public virtual async Task AddOrUpdateAsync(IEnumerable<TEntity> entities)
        //{
        //    await Task.Run(() => AddOrUpdate(entities)).ConfigureAwait(false);
        //}

        //public virtual async Task AddOrUpdateAsync(TEntity entity)
        //{
        //    await Task.Run(() => AddOrUpdate(entity)).ConfigureAwait(false);
        //}

        public virtual async Task<TEntity> FindAsync(int id)
        {
            return await Task.Run(() => Find(id)).ConfigureAwait(false);
        }

        //public virtual async Task<IList<TEntity>> AllAsync()
        //{
        //    return await Task.Run(() => All()).ConfigureAwait(false);
        //}

        //public virtual async Task DeleteAsync(TEntity entity)
        //{
        //    await Task.Run(() => Delete(entity)).ConfigureAwait(false);
        //}
    }
}
