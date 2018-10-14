using System.Collections.Generic;
using System.Threading.Tasks;

namespace vega.Controllers.Repositories {
	public interface IRepository<T> {
		Task<T> GetAsync(int id);

		Task<List<T>> GetAllAsync();

		Task<T> GetWithDependenciesAsync(int id);

		Task CreateAsync(T value);

		void Delete(T value);
	}
}
