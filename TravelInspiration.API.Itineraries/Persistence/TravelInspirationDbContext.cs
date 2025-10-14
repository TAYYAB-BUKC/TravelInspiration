using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TravelInspiration.API.Itineraries.Domain.Entities;

namespace TravelInspiration.API.Itineraries.Persistence
{
	public sealed class TravelInspirationDbContext(
	DbContextOptions<TravelInspirationDbContext> options) : DbContext(options)
	{
		public DbSet<Itinerary> Itineraries => Set<Itinerary>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(
				typeof(TravelInspirationDbContext).Assembly);
			base.OnModelCreating(modelBuilder);
		}

		public override async Task<int> SaveChangesAsync(
			CancellationToken cancellationToken = default)
		{
			foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatedOn = DateTime.UtcNow;
						entry.Entity.CreatedBy = "SYSTEM";
						entry.Entity.LastModifiedOn = DateTime.UtcNow;
						entry.Entity.LastModifiedBy = "SYSTEM";
						break;
					case EntityState.Modified:
						entry.Entity.LastModifiedOn = DateTime.UtcNow;
						entry.Entity.LastModifiedBy = "SYSTEM";
						break;
				}
			}

			return await base.SaveChangesAsync(cancellationToken);
		}
	}
}