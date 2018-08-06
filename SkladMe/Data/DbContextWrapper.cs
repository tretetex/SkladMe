using System;

namespace SkDAL
{
    class DbContextWrapper : IDisposable
    {
        public DbContext Context { get; }

        public DbContextWrapper()
        {
            Context = new DbContext();
            Context.Configuration.AutoDetectChangesEnabled = false;
            Context.Configuration.ValidateOnSaveEnabled = false;
        }

        private bool _disposed;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.SaveChanges();
                    Context.Configuration.AutoDetectChangesEnabled = true;
                    Context.Configuration.ValidateOnSaveEnabled = true;
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
