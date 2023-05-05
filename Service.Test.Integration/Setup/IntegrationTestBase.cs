using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.Repositories.MyDb;
using Test.IntegrationTests.Factory;

namespace Test.IntegrationTests.Base;

public class IntegrationTestBase : IClassFixture<IntegrationTestFactory<Program, MyDbContext>>
{
    public readonly IntegrationTestFactory<Program, MyDbContext> Factory;
    public readonly MyDbContext DbContext;

    public IntegrationTestBase(IntegrationTestFactory<Program, MyDbContext> factory)
    {
        Factory = factory;
        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        DbContext.Database.EnsureCreated();
    }
}
