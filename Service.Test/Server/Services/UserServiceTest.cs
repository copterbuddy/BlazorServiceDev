using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Service.DTO;
using Service.Repositories.Interfaces;
using Service.Services.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using static System.Collections.Specialized.BitVector32;

namespace Service.Test.Services;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    // setup
    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configuration = new ConfigurationBuilder().Build();
        _userService = new UserService(_userRepositoryMock.Object, _configuration);
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsTrue()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        _userRepositoryMock
            .Setup(x => x.Register(request.Email, request.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.Register(request);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null, "password", "password")]
    [InlineData("test@example.com", null, "password")]
    [InlineData("test@example.com", "password", null)]
    [InlineData("test@example.com", "password", "invalid")]
    public async Task Register_WithInvalidRequest_ReturnsFalse(string email, string password, string confirmPassword)
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword
        };

        // Act
        var result = await _userService.Register(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Register_ThrowsException_ReturnsFalse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        _userRepositoryMock
            .Setup(x => x.Register(request.Email, request.Password))
            .Throws(new Exception("Something went wrong"));

        // Act
        Exception exception = await Assert.ThrowsAsync<Exception>(
            async () =>
            await _userService.Register(request)
            );

        // Assert
        Assert.Equal("Something went wrong", exception.Message);


    }

}
