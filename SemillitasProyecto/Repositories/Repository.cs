using SemillitasProyecto.Data;

namespace SemillitasProyecto.Repositories
{
    public class Repository<T> where T : class
    {
        public SemillitasContext Context { get; }

        public Repository(SemillitasContext _context)
        {
            Context = _context;
        }
        public void Insert(T entity) {
            Context.Add(entity);
            Context.SaveChanges();
        }
        public IEnumerable<T> GetAll() {
            var data = Context.Set<T>();
            return data;
        }
        public T? GetById(object id) {
            var data = Context.Find<T>(id);
            return data;
        }
        public IQueryable<T> Query() {
            return Context.Set<T>().AsQueryable();
        }
        public void Update(T entity) {
            Context.Update(entity);
            Context.SaveChanges();
        }
        public void Delete(T entity) {
            Context.Remove(entity);
            Context.SaveChanges();
        }
    }
}
