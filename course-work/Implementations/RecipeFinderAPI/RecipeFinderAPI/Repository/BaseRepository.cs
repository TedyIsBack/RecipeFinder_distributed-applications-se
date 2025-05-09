using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Data;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Repositories
{
    public class BaseRepository<T>
      where T : class
    {
        private DbContext _context { get; set; }
        private DbSet<T> _items { get; set; }

        public BaseRepository(RecipeFinderDbContext context)
        {
            _context = context;
            _items = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _items;
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return await _items.FirstOrDefaultAsync(filter);
        }

        public async Task AddAsync(T item)
        {
            await _items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T item)
        {
            _items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T item)
        {
            _items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
