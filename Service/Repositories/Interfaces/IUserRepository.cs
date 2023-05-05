using Service.Repositories.MyDb.Model;

namespace Service.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> Register(string Email, string Password);
    Task<UserDetail> Login(string Email, string Password);
}
