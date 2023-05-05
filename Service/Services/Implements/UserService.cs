using Service.Repositories.Interfaces;
using Service.Repositories.MyDb;
using Service.Repositories.MyDb.Model;
using System.IdentityModel.Tokens.Jwt;
using Service.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Service.DTO;

namespace Service.Services.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public MyDbContext myDbContext { get; }

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<bool> Register(RegisterRequest request)
    {
        try
        {
            if (request.Email is null ||
                    request.Password is null ||
                    request.ConfirmPassword is null ||
                    request.Password != request.ConfirmPassword)
            {
                return false;
            }

            bool result = await _userRepository.Register(Email: request.Email, Password: request.Password);

            return result;
        }
        catch (Exception e)
        {

            throw e;
        }
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password)) return null;

        try
        {
            var user = await _userRepository.Login(request.Email, request.Password);

            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            var audience = _configuration.GetValue<string>("Jwt:Audience");
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                //new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            if(jwtToken is null) return null;

            var result = new LoginResponse
            {
                Token = jwtToken,
            };

            return result;
        }
        catch (Exception e)
        {

            throw e;
        }
    }
}
