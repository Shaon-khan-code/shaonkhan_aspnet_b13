using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DevSkill.Shop.Domain.Repositories
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IList<TEntity>> GetAllAsync();
        Task<(IList<TEntity> data, int total, int totalDisplay)> GetPagedAsync(
            int pageIndex, int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
