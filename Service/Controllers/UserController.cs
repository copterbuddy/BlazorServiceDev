using Service.Repositories.MyDb.Model;
using Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Service.DTO;
using Microsoft.AspNetCore.Cors;

namespace Service.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IConfiguration configuration,IUserService userServce)
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userServce;
    }

    [HttpGet("Greeting")]
    public IActionResult Get()
    {
        _logger.LogInformation("Greeting Log");

        string env = _configuration?.GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        return Ok($"Service Running On {env}");
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequest request)
    {
        try
        {
            if (request.Email is null ||
                request.Password is null ||
                request.ConfirmPassword is null ||
                request.Password != request.ConfirmPassword)
            {
                return Unauthorized();
            }

            var result = await _userService.Register(request);

            var response = new RegisterResponse
            {
                Result = true, 
            };

            return Ok(response);
        }
        catch (Exception e)
        {

            return BadRequest(e);
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest request)
    {
        try
        {
            if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password)) return BadRequest("Invalid Parameter");

            LoginResponse response = await _userService.Login(request);

            if(response is null) return Unauthorized();

            return Ok(response);
        }
        catch (Exception e)
        {

            return BadRequest(e);
        }
    }

    [HttpGet("GreetingAuthen")]
    [Authorize]
    public IActionResult GreetingAuthen()
    {
        _logger.LogInformation("Greeting Log");

        var userEmail = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userJti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var userId = User.FindFirst("Id")?.Value;
        var userCustom = User.FindFirst("MyCustomType")?.Value;

        return Ok($"Service Running On {_configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT")}");
    }
}
