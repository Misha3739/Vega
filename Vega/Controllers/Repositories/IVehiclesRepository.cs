using System.Threading.Tasks;
using vega.Models;

namespace vega.Controllers.Repositories {
	public interface IVehiclesRepository : IRepository<Vehicle> {
		Task<bool> IsModelExists(int modelId);

		Task<bool> IsFeatureExists(int featureId);
	}
}
