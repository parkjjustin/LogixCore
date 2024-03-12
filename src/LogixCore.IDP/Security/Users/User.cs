using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogixCore.IDP.Security;

public interface IUser<Guid>
{
    Guid UserId { get; }
    string UserName { get; }
    string Email { get; }
    string PasswordHash { get; }
    string SecurityStamp { get; set; }
}

public sealed class User : IUser<Guid>
{
    public Guid UserId { get; set; }
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public bool EmailConfirmed { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string? PhoneNumber { get; private set; } = default!;
    public bool PhoneNumberConfirmed { get; private set; } = default!;
    public bool TwoFactorEnabled { get; private set; } = default!;
    public int AccessFailedCount { get; private set; } = default!;
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("D");
    public DateTime CreatedOn { get; private set; }
    public bool IsActive { get; private set; } = default!;
    public byte[] RowVersion { get; init; } = default!;
    public ICollection<UserClaim> Claims { get; private set; } = new List<UserClaim>();

    public User() : base()
    {
        this.UserId = Guid.NewGuid();
        this.CreatedOn = DateTime.UtcNow;
    }

    public User(string username, string email, string passwordHash)
    {
        this.UserName = username;
        this.Email = email;
        this.PasswordHash = passwordHash;
    }

    public void ChangeUserName(string username)
    {
        this.UserName = username;
    }

    public void ChangeEmail(string email)
    {
        this.Email = email;
    }

    public void ChangePassword(string passwordHash)
    {
        this.PasswordHash = passwordHash;
    }

    internal class EntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "Security");
            builder.HasKey(x => x.UserId).IsClustered();
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(15);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(320);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.OwnsMany(x => x.Claims, claims =>
            {
                claims.ToTable("UserClaims", "Security");
                claims.HasKey(x => x.Id).IsClustered();
                claims.Property(x => x.Type).IsRequired().HasMaxLength(250);
                claims.Property(x => x.Value).IsRequired().HasMaxLength(250);
                claims.WithOwner().HasForeignKey(nameof(UserId));
            });
        }
    }
}