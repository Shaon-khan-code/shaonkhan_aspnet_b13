using System;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Application.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    void Add(TEntity entity);

    IQueryable<TEntity> GetAll();
}
