using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Command
{
    public class ProductDeleteCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}
