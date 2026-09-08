using Mapster;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Category -> CategoryDto: straightforward, all property names match,
        // so Mapster's convention-based mapping handles this with no extra rules.
        config.NewConfig<Category, CategoryDto>();

        // Product -> ProductDto: demonstrates a custom mapping rule —
        // flattening the nested Category.Name into a single CategoryName field.
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category.Name);
    }
}