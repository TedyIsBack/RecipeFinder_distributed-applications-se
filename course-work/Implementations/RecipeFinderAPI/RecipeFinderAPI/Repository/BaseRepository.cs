using Microsoft.EntityFrameworkCore;
using RecipeFinderAPI.Data;
using RecipeFinderAPI.Infrastructure;
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

        public async Task<PagedResult<T>> GetAllAsync(
            IQueryable<T> query,
            Expression<Func<T, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10)
        {
            if (filter != null)
                query = query.Where(filter);

            int totalCount = await query.CountAsync();

            int pagesCount =
              (int)Math.Ceiling(totalCount / (double)itemsPerPage);

            var items = await query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                itemsPerPage = itemsPerPage,
                PagesCount = pagesCount
            };
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

        public IQueryable<T> Query()
        {
            return _items.AsQueryable();
        }
    }
}
