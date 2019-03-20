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

		public async Task<Feature> GetAsync(int id) {
			return await _dbContext.Features.FindAsync(id);
		}

		public async Task<List<Feature>> GetAllAsync() {
			return await _dbContext.Features.ToListAsync();
		}

		public async Task<Feature> GetWithDependenciesAsync(int id) {
			return await GetAsync(id);
		}

		public async Task CreateAsync(Feature value) {
			await _dbContext.Features.AddAsync(value);
		}

		public void Delete(Feature value) {
			_dbContext.Features.Remove(value);
		}
	}
}
