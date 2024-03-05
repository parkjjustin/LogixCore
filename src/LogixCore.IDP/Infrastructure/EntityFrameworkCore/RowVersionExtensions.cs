using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

internal static class RowVersionExtensions
{
    private static readonly EntityState[] StatesToIgnore = new[] { EntityState.Unchanged, EntityState.Detached };

    public static void ApplyRowVersions(this ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries().Where(entry => !StatesToIgnore.Contains(entry.State)))
        {
            var property = entry.Properties.SingleOrDefault(p => p.Metadata.IsConcurrencyToken);
            if (property is null)
            {
                continue;
            }

            if (entry.State != EntityState.Added && property.CurrentValue is null)
            {
                throw new Exception("Row version has been incorrectly set to null");
            }

            if (entry.State == EntityState.Modified)
            {
                var current = (byte[])(property.CurrentValue ?? throw new Exception("Could not get current row version"));
                var original = (byte[])(property.OriginalValue ?? throw new Exception("Could not get original row version"));
                if (!current.SequenceEqual(original))
                {
                    throw new System.Data.DBConcurrencyException($"RowVersion does not match on entity type '{entry.Entity.GetType()}'{Environment.NewLine}{entry.DebugView.LongView}.");
                }
            }

            property.CurrentValue = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        }
    }
}
