using Cortex.Mediator.Commands;
using Demo.Application.Contracts;
using Demo.Application.Exceptions;
using Demo.Domain.Contracts;
using Demo.Domain.Entities;
using Demo.Domain.Utilities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Command
{
    public class ProductUpdateCommandHandler : ICommandHandler<ProductUpdateCommand, Product>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductUpdateCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper) 
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Product> Handle(ProductUpdateCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.ProductRepository.IsDuplicateProductName(command.Name, command.Id,
                cancellationToken);

            if (!isDuplicateName)
            {
                var product = _unitOfWork.ProductRepository.GetById(command.Id);
                product = _mapper.Map(command, product);

                await _unitOfWork.ProductRepository.EditAsync(product, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return product;
            }
            else
                throw new DuplicateDataException("Product name is duplicate");
        }
    }
}
