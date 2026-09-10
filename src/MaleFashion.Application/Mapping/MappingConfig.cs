using Mapster;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>();

        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category.Name);

        config.NewConfig<Inventory, InventoryDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductSku, src => src.Product.Sku);

        config.NewConfig<Address, AddressDto>();
        config.NewConfig<OrderItem, OrderItemDto>();
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.ShippingAddress, src => src.ShippingAddress);

        config.NewConfig<Order, OrderSummaryDto>()
    .Map(dest => dest.ItemCount, src => src.OrderItems.Count);
    }
}