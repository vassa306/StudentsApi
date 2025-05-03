using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using studentsapi.Model;

namespace studentsapi.Data.Repository
{
    public class CollegeRepository<T> : ICollegeRepository<T> where T : class
    {
        private readonly CollegeDBContext _context;
        private readonly DbSet<T> _dbSet;
        public CollegeRepository(CollegeDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T> CreateAsync(T record)
        {
            Console.WriteLine(">>> Saving new record");
            _dbSet.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentNullException($"{typeof(T).Name} with ID {id} not found.");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<T> GetByName(Expression<Func<T, bool>> predicate)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(predicate);
            if (entity == null)
            {
                throw new ArgumentNullException($"{typeof(T).Name} not found with the given condition.");
            }

            return entity;
        }

        public async Task<T> UpdateAsync(T record)
        {
            _context.Update(record);
            await _context.SaveChangesAsync();
            return record;
        }

    }
}

