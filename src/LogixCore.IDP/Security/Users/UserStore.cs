using LogixCore.IDP.Data;

namespace LogixCore.IDP.Security;

public interface IUserStore<TUser> : IDisposable where TUser : class
{
    Task CreateUserAsync(TUser user, CancellationToken ct);
}

public class UserStore<TUser> : IUserStore<TUser> where TUser : class
{
    private readonly IdentityDbContext db;

    public UserStore(IdentityDbContext db)
    {
        this.db = db;
    }

    public async Task CreateUserAsync(TUser user, CancellationToken ct)
    {
        await this.db.AddAsync(user, ct);
        await this.db.SaveChangesAsync(ct);
    }

    public void Dispose()
    {

    }
}
