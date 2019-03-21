using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vega.Models;

namespace vega.Persistence {
	public interface IVegaDbContext {
		DbSet<Make> Makes { get; set; }
		DbSet<Model> Models { get; set; }
		DbSet<Feature> Features { get; set; }
		DbSet<Vehicle> Vehicles { get; set; }
		DbSet<User> Users { get; set; }
		DbSet<VehicleFeature> VehicleFeatures { get; set; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}