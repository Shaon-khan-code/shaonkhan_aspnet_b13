using Demo.Application.Contracts;
using Demo.Application.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Infrastructure.Data
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {
        public ISqlUtility SqlUtility { get; private set; }
        public IProductRepository ProductRepository { get; private set; }

        public ApplicationUnitOfWork(ApplicationDbContext dbContext, IProductRepository productRepository)
            : base (dbContext)
        {
            SqlUtility = new SqlUtility(dbContext.Database.GetDbConnection());
            ProductRepository = productRepository;
        }
    }
}
