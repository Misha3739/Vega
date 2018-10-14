using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Repositories;
using vega.Models;

namespace vega.Persistence.Repositories {
	public sealed class VehiclesRepository : IVehiclesRepository {
		private readonly IVegaDbContext _context;

		public VehiclesRepository(IVegaDbContext context) {
			_context = context;
		}

		public async Task<Vehicle> GetAsync(int id) {
			return await _context.Vehicles.
				SingleOrDefaultAsync(v => v.Id == id);
		}

		public Task<List<Vehicle>> GetAllAsync() {
			throw new NotImplementedException();
		}

		public async Task<Vehicle> GetWithDependenciesAsync(int id) {
			return await _context.Vehicles.
				Include(v => v.Model).
				ThenInclude(m => m.Make).
				Include(v => v.Features).
				ThenInclude(vf => vf.Feature).
				SingleOrDefaultAsync(v => v.Id == id);
		}

		public async Task CreateAsync(Vehicle value) {
			await _context.Vehicles.AddAsync(value);
		}

		public void Delete(Vehicle value) {
			_context.Vehicles.Remove(value);
		}

		public async Task<bool> IsModelExists(int modelId) {
			return	await _context.Models.SingleOrDefaultAsync(m => m.Id == modelId) != null;
		}

		public async Task<bool> IsFeatureExists(int featureId) {
			return await _context.Features.SingleOrDefaultAsync(m => m.Id == featureId) != null;
		}
	}
}
