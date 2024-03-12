using LogixCore.IDP.Security;
using Microsoft.EntityFrameworkCore;
using System;

namespace LogixCore.IDP.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        if (this.Database.IsInMemory())
        {
            this.ChangeTracker.ApplyRowVersions();
        }

        var updatedUserEntries = this.ChangeTracker
            .Entries<IUser<Guid>>()
            .Where(e => e.Property(x => x.PasswordHash).IsModified)
            .OfType<IUser<Guid>>();

        foreach (var entry in updatedUserEntries)
        {
            entry.SecurityStamp = Guid.NewGuid().ToString("D");
        }

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
