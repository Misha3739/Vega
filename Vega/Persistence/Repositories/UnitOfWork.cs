using System.Threading.Tasks;
using vega.Controllers.Repositories;

namespace vega.Persistence.Repositories {
	public sealed class UnitOfWork : IUnitOfWork {
		private readonly IVegaDbContext _dbContext;

		public UnitOfWork(IVegaDbContext dbContext) {
			_dbContext = dbContext;
		}

		public async Task CompeleteAsync() {
			await _dbContext.SaveChangesAsync();
		}
	}
}
