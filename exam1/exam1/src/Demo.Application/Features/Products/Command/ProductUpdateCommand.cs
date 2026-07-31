using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Demo.Domain.Entities;

namespace Demo.Application.Features.Products.Command
{
    public class ProductUpdateCommand : ICommand<Product>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageName { get; set; }
    }
}
