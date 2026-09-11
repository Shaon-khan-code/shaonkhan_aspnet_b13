using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Addresses.Queries.GetAddressesByUser;

public class GetAddressesByUserQuery : IRequest<List<AddressDto>>
{
    public string UserId { get; set; } = string.Empty;
}