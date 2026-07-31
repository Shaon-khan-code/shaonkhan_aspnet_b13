using Demo.Application.Features.Products.Command;
using Demo.Domain.Entities;
using Demo.Web.Areas.Admin.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Web
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProductAddCommand, Product>();
            config.NewConfig<ProductModel, ProductAddCommand>();
        }
    }
}
