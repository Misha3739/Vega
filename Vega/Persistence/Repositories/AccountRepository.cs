using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Repositories;
using vega.Models;

namespace vega.Persistence.Repositories {
	public class AccountRepository : IAccountRepository {
		private readonly IVegaDbContext _dbContext;

		public AccountRepository(IVegaDbContext dbContext) {
			_dbContext = dbContext;
		}

		public async Task<User> FindUser(string email) {
			return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
	}
}