using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Tests.Common;
using MaleFashion.Domain.Entities;
using Moq;
using Shouldly;
using Xunit;

namespace MaleFashion.Application.Tests.Infrastructure;

public class ProjectSetupTests
{
    [Fact]
    public void UnitOfWorkMock_CanBeConfiguredAndUsed()
    {
        var unitOfWorkMock = TestUnitOfWorkFactory.CreateUnitOfWorkMock();
        var category = new Category { Id = 1, Name = "Shirts", Slug = "shirts" };

        Mock.Get(unitOfWorkMock.Object.Categories)
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        var result = unitOfWorkMock.Object.Categories.GetByIdAsync(1).Result;

        result.ShouldNotBeNull();
        result.Name.ShouldBe("Shirts");
    }

    [Fact]
    public void RealMapper_MapsCategoryToCategoryDto_Correctly()
    {
        var mapper = TestUnitOfWorkFactory.CreateRealMapper();
        var category = new Category { Id = 1, Name = "Shirts", Slug = "shirts", Description = "Casual shirts" };

        var dto = mapper.Map<CategoryDto>(category);

        dto.Id.ShouldBe(1);
        dto.Name.ShouldBe("Shirts");
        dto.Slug.ShouldBe("shirts");
        dto.Description.ShouldBe("Casual shirts");
    }
}