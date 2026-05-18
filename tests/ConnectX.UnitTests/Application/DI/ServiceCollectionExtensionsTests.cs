using Application;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.UnitTests.Application.DI;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplicationServices();

        Assert.Same(services, result);
    }
}
