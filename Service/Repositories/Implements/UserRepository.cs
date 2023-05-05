using Service.Repositories.Interfaces;
using Service.Repositories.MyDb;
using Service.Repositories.MyDb.Model;
using Microsoft.EntityFrameworkCore;

namespace Service.Repositories.Implements;

public class UserRepository : IUserRepository
{
    private readonly MyDbContext _myDb;

    public UserRepository(MyDbContext myDb)
    {
        _myDb = myDb;
    }

    public async Task<UserDetail> Login(string Email, string Password)
    {
        UserDetail result = await _myDb.UserDetail.FirstOrDefaultAsync(
                            user => 
                            user.Email.Equals(Email) == true && 
                            user.Password.Equals(Password) == true
                            );

        return result;
    }

    public async Task<bool> Register(string Email, string Password)
    {
        if (Email is null || Password is null) return false;

        try
        {
            UserDetail user = new()
            {
                Email = Email,
                Name = Email,
                Password = Password,
            };
            await _myDb.UserDetail.AddAsync(user);
            int result = await _myDb.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

}
