using Microsoft.EntityFrameworkCore;
using vega.Models;
using vega.Utility;

namespace vega.Persistence {
	public class VegaDbContext : DbContext, IVegaDbContext {
		public VegaDbContext(DbContextOptions<VegaDbContext> options)
			: base(options) {
		}

		public DbSet<Make> Makes { get; set; }
		public DbSet<Model> Models { get; set; }
		public DbSet<Feature> Features { get; set; }
		public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<VehicleFeature> VehicleFeatures { get; set; }

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			User administrator = new User() {
				Id = 1,
				Email = "mihail.udot@gmail.com",
				EncryptedPassword = EncryptionUtility.Encrypt("newPassw0rd!"),
				Role = Role.Administrator,
				FirstName = "Mihail",
				LastName = "Udot",
				MobilePhone = "+79222222222"
			};
			modelBuilder.Entity<User>().HasData(administrator);
			modelBuilder.Entity<VehicleFeature>().HasKey(vf => new {vf.VehicleId, vf.FeatureId});
		}
	}
}