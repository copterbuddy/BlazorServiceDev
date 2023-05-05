using Service.Repositories.MyDb.Model.Base;

namespace Service.Repositories.MyDb.Model;

public class UserDetail : BaseModel
{
    public string? Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Role { get; set; }
}
