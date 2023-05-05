using Microsoft.EntityFrameworkCore;
using Service.Repositories.Implements;
using Service.Repositories.MyDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Test.Repositories;

public class UserRepositoryTest : IDisposable
{
    private readonly MyDbContext _dbContext;
    private readonly UserRepository _userRepository;

    public UserRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "blazor_db")
            .Options;
        _dbContext = new MyDbContext(options);
        _userRepository = new UserRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task Register_WithValidInputs_ShouldRegisterUserAndReturnTrue()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password";

        // Act
        var result = await _userRepository.Register(email, password);

        // Assert
        Assert.True(result);
        Assert.Single(_dbContext.UserDetail);
        Assert.Equal(email, _dbContext.UserDetail.First().Email);
        Assert.Equal(password, _dbContext.UserDetail.First().Password);
    }

    [Theory]
    [InlineData(null, "password")]
    [InlineData("test@example.com", null)]
    public async Task Register_WithInvalidInputs_ShouldNotRegisterUserAndReturnException(string email, string password)
    {
        //Arrange

        // Act
        var result = await _userRepository.Register(email, password);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("test@example.com", "")]
    public async Task Register_WithInvalidInputs_ShouldNotRegisterUserAndReturnFalse(string email, string password)
    {
        // Act
        var result = await _userRepository.Register(email, password);

        // Assert
        Assert.True(result);
        Assert.Equal(email,_dbContext.UserDetail.First().Email);
        Assert.Equal(password,_dbContext.UserDetail.First().Password);

    }
}
