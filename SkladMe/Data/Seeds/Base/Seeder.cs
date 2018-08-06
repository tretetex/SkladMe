namespace SkDAL.Seeds.Base
{
    public abstract class Seeder
    {
        protected DbContext Db { get; }

        public Seeder(DbContext db)
        {
            Db = db;
        }

        public void Call()
        {
            try
            {
                Run();
            }
            catch
            {
                Db.Dispose();
                throw;
            }
        }

        protected abstract void Run();
    }
}
