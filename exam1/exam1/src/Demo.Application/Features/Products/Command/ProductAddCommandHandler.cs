using Cortex.Mediator.Commands;
using Demo.Application.Contracts;
using Demo.Application.Exceptions;
using Demo.Domain.Entities;
using Demo.Domain.Utilities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Command
{
    public class ProductAddCommandHandler : ICommandHandler<ProductAddCommand, Product>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductAddCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper) 
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Product> Handle(ProductAddCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.ProductRepository.IsDuplicateProductName(command.Name, null,
                cancellationToken);

            if (!isDuplicateName)
            {
                var product = _mapper.Map<Product>(command);
                product.Id = IdentityGenerator.NewSequentialGuid();

                await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);

                return product;
            }
            else
                throw new DuplicateDataException("Product name is duplicate");
        }
    }
}
