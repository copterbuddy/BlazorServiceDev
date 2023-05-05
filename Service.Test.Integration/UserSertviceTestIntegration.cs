using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using System.Net.Http.Json;
using Test.Integration.Creators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNet.Testcontainers.Containers;
using Test.IntegrationTests.Base;
using Service;
using Test.IntegrationTests.Factory;
using Service.Repositories.MyDb;
using Service.DTO;

namespace Test.Integration;

public class UserServiceTestIntegration : IntegrationTestBase
{
    private readonly IntegrationTestFactory<Program, MyDbContext> _integrationTestFactory;
    private readonly UserCreators _userCreators;

    public UserServiceTestIntegration(IntegrationTestFactory<Program, MyDbContext> factory) : base(factory)
    {
        _integrationTestFactory = factory;
        var scope = factory.Services.CreateScope();
        _userCreators = scope.ServiceProvider.GetRequiredService<UserCreators>();
    }

    [Fact]
    public async Task GreetingService()
    {
        //var webAppFacotry = new WebApplicationFactory<Program>();
        var _client = _integrationTestFactory.CreateDefaultClient();

        HttpResponseMessage response = await _client.GetAsync("/User/Greeting");
        var stringResult = await response.Content.ReadAsStringAsync();

        Assert.Equal("Service Running On ", stringResult);
    }

    [Fact]
    public async Task RegisterService()
    {
        var _client = _integrationTestFactory.CreateDefaultClient();

        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        HttpResponseMessage resp = await _client.PostAsJsonAsync("/User/Register", request);
        RegisterResponse respObj = await resp.Content.ReadFromJsonAsync<RegisterResponse>();

        Assert.True(resp.IsSuccessStatusCode);
        Assert.True(respObj.Result);
    }
}
