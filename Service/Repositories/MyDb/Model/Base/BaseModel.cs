namespace Service.Repositories.MyDb.Model.Base;

public class BaseModel
{
    public Guid Id { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }
}
