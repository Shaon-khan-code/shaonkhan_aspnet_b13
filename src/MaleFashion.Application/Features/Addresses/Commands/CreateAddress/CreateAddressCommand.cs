using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommand : IRequest<AddressDto>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}