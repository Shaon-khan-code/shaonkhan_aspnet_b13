using Cortex.Mediator.Commands;
using Demo.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Command
{
    public class ProductDeleteCommandHandler : ICommandHandler<ProductDeleteCommand>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public ProductDeleteCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProductDeleteCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductRepository.RemoveAsync(command.Id, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
