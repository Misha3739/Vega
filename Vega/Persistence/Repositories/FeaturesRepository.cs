using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Repositories;
using vega.Models;

namespace vega.Persistence.Repositories {
	public sealed class FeaturesRepository : IFeaturesRepository {
		private readonly IVegaDbContext _dbContext;

		public FeaturesRepository(IVegaDbContext dbContext) {
			_dbContext = dbContext;
		}

		public Task<Feature> GetAsync(int id) {
			throw new NotImplementedException();
		}

		public async Task<List<Feature>> GetAllAsync() {
			return await _dbContext.Features.ToListAsync();
		}

		public Task<Feature> GetWithDependenciesAsync(int id) {
			throw new NotImplementedException();
		}

		public Task CreateAsync(Feature value) {
			throw new NotImplementedException();
		}

		public void Delete(Feature value) {
			throw new NotImplementedException();
		}
	}
}
