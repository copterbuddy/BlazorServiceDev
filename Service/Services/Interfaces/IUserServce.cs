using Service.DTO;
using Service.Repositories.MyDb.Model;

namespace Service.Services.Interfaces;

public interface IUserService
{
    Task<bool> Register(RegisterRequest request);
    Task<LoginResponse> Login(LoginRequest request);
}
