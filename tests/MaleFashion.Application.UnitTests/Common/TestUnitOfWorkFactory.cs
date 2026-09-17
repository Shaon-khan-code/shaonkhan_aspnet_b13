using MaleFashion.Application.Contracts.Repositories;
using Mapster;
using MapsterMapper;
using MaleFashion.Application.Mapping;
using Moq;
using System.Reflection;

namespace MaleFashion.Application.Tests.Common;

public static class TestUnitOfWorkFactory
{
    public static Mock<IApplicationUnitOfWork> CreateUnitOfWorkMock()
    {
        var unitOfWork = new Mock<IApplicationUnitOfWork>();

        unitOfWork.Setup(u => u.Categories).Returns(new Mock<ICategoryRepository>().Object);
        unitOfWork.Setup(u => u.Products).Returns(new Mock<IProductRepository>().Object);
        unitOfWork.Setup(u => u.Inventory).Returns(new Mock<IInventoryRepository>().Object);
        unitOfWork.Setup(u => u.Carts).Returns(new Mock<ICartRepository>().Object);
        unitOfWork.Setup(u => u.Orders).Returns(new Mock<IOrderRepository>().Object);
        unitOfWork.Setup(u => u.Addresses).Returns(new Mock<IAddressRepository>().Object);

        return unitOfWork;
    }

    public static IMapper CreateRealMapper()
    {
        var config = new TypeAdapterConfig();
        config.Scan(Assembly.GetAssembly(typeof(MappingConfig))!);
        return new ServiceMapper(new Mock<IServiceProvider>().Object, config);
    }
}