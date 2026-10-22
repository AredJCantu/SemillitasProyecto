using SemillitasProyecto.Data;

namespace SemillitasProyecto.Repositories
{
    public class Repository<T> where T : class
    {
        public SemillitasContext _context { get; set; } = new();
        public void Insert(T entity) {
            _context.Add(entity);
            _context.SaveChanges();
        }
        public IEnumerable<T> GetAll(T entity) {
            var data = _context.Set<T>();
            return data;
        }
        public T? GetById(object id) {
            var data = _context.Find<T>(id);
            return data;
        }
        public void Update(T entity) {
            _context.Update(entity);
            _context.SaveChanges();
        }
        public void Delete(object id) {
            var data = _context.Find<T>(id);
            if(data != null)
            {
                _context.Remove(data);
                _context.SaveChanges();
            }
        }
    }
}
