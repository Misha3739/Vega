using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Repositories;
using vega.Models;

namespace vega.Persistence.Repositories
{
	public sealed class MakesRepository : IMakesRepository {
		private readonly IVegaDbContext _dbContext;

		public MakesRepository(IVegaDbContext dbContext) {
			_dbContext = dbContext;
		}

		public Task<Make> GetAsync(int id) {
			throw new NotImplementedException();
		}

		public async Task<List<Make>> GetAllAsync() {
			return await _dbContext.Makes.Include(m=>m.Models).ToListAsync();
		}

		public Task<Make> GetWithDependenciesAsync(int id) {
			throw new NotImplementedException();
		}

		public Task CreateAsync(Make value) {
			throw new NotImplementedException();
		}

		public void Delete(Make value) {
			throw new NotImplementedException();
		}
	}
}
