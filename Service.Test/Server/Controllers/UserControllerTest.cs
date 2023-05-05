using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Service.Controllers;
using Service.Services.Interfaces;
using Service.DTO;

namespace Service.Test.Controllers;

public class UserControllerTest
{
    UserController controller;

    // setup
    public UserControllerTest()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ASPNETCORE_ENVIRONMENT", "Development"},
            {"SectionName:SomeKey", "SectionValue"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var loggerMock = new Mock<ILogger<UserController>>();
        var configMock = configuration;
        var userServiceMock = new Mock<IUserService>();
        controller = new UserController(loggerMock.Object, configMock, userServiceMock.Object);
    }

    #region User/Greeting
    [Fact]
    public void Get_ReturnsOkObjectResult()
    {
        // Arrange

        // Act
        var result = controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Get_ReturnsExpectedValue()
    {
        // Arrange
        var expectedValue = "Service Running On Development";

        // Act
        var result = controller.Get() as OkObjectResult;

        // Assert
        Assert.Equal(expectedValue, result.Value);
    }
    #endregion User/Greeting

    #region User/Register
    [Fact]
    public async Task Register_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.Register(request)).ReturnsAsync(true);

        var loggerMock = new Mock<ILogger<UserController>>();

        var configurationMock = new Mock<IConfiguration>();

        controller = new UserController(loggerMock.Object, configurationMock.Object, userServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<RegisterResponse>(okResult.Value);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task Register_InvalidRequest_ReturnsUnauthorizedResult()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "wrongpassword"
        };

        controller = new UserController(null, null, null);

        // Act
        var result = await controller.Register(request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    #endregion User/Register
}
