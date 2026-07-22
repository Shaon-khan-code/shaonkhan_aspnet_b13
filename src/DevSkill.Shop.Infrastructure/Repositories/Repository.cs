using DevSkill.Shop.Domain.Repositories;
using DevSkill.Shop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevSkill.Shop.Infrastructure.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id) =>
            await _dbSet.FindAsync(id);

        public async Task<IList<TEntity>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<(IList<TEntity> data, int total, int totalDisplay)> GetPagedAsync(
            int pageIndex, int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var prop in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(prop.Trim());

            int total = await query.CountAsync();

            if (filter != null)
                query = query.Where(filter);

            int totalDisplay = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total, totalDisplay);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id)
                ?? throw new InvalidOperationException("Entity not found.");
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter) =>
            await _dbSet.AnyAsync(filter);
    }
}