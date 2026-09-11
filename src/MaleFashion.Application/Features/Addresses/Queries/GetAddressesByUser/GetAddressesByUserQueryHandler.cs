
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Addresses.Queries.GetAddressesByUser;

public class GetAddressesByUserQueryHandler : IRequestHandler<GetAddressesByUserQuery, List<AddressDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAddressesByUserQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<AddressDto>> Handle(GetAddressesByUserQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _unitOfWork.Addresses.GetByUserIdAsync(request.UserId);
        return _mapper.Map<List<AddressDto>>(addresses);
    }
}