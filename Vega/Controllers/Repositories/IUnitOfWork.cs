using System.Threading.Tasks;

namespace vega.Controllers.Repositories {
	public interface IUnitOfWork {
		Task CompeleteAsync();
	}
}
