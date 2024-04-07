using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Service.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("")]
    public class BaseController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        public BaseController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet()]
        [Authorize]
        public IActionResult Get()
        {
            _logger.LogInformation("Greeting Log");
            return Ok($"Service Running On {_configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT")}");
        }
    }
}
