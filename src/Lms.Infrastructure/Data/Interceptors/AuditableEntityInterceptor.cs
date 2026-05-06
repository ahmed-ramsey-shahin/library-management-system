using Lms.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lms.Infrastructure.Data.Interceptors
{
    public class AuditableEntityInterceptor(TimeProvider dateTime) : SaveChangesInterceptor
    {
        public void UpdateEntities(DbContext? db)
        {
            if (db is null)
            {
                return;
            }

            foreach (var entry in db.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Deleted || entry.HasChangedOwnedData())
                {
                    var now = dateTime.GetUtcNow();

                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedAt = now;
                    }

                    if (entry.State == EntityState.Deleted)
                    {
                        entry.Entity.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }

                    foreach (var ownedEntry in entry.References)
                    {
                        if (ownedEntry.TargetEntry is { Entity: AuditableEntity ownedEntity })
                        {
                            if (ownedEntry.TargetEntry.State is EntityState.Added)
                            {
                                ownedEntity.CreatedAt = now;
                            }

                            if (ownedEntry.TargetEntry.State is EntityState.Deleted)
                            {
                                ownedEntity.IsDeleted = true;
                                ownedEntry.TargetEntry.State = EntityState.Modified;
                            }
                        }
                    }
                }
            }
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken );
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedData(this EntityEntry entry)
        {
            return entry.References.Any(r =>
                    r.TargetEntry?.Metadata.IsOwned() == true &&
                    (r.TargetEntry.State is EntityState.Added or EntityState.Deleted)
            );
        }
    }
}
