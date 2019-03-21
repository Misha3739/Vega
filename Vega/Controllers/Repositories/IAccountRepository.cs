using System.Threading.Tasks;
using vega.Controllers.Resources;
using vega.Models;

namespace vega.Controllers.Repositories {
	public interface IAccountRepository {
		Task<User> FindUser(string email);
	}
}