using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Resources;
using vega.Models;
using vega.Persistence;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace vega.Controllers {
	[Route("/api/vehicles")]
	public class VehiclesController : Controller {
		private readonly IVegaDbContext _context;
		private readonly IMapper _mapper;

		public VehiclesController(IVegaDbContext context, IMapper mapper) {
			this._mapper = mapper;
			this._context = context;
		}

		[HttpPost]
		public async Task<IActionResult> CreateVehicle([FromBody] VehicleResource vehicle) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			if (!await ValidateVehicle(vehicle)) {
				return BadRequest(ModelState);
			}

			try {
				Vehicle domainVehicle = _mapper.Map<VehicleResource, Vehicle>(vehicle);
				domainVehicle.LastUpdate = DateTime.Now;
				_context.Vehicles.Add(domainVehicle);
				await _context.SaveChangesAsync();
				VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(domainVehicle);
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleResource vehicle) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			var domainVehicle = await _context.Vehicles.Include(v => v.Features).SingleOrDefaultAsync(v => v.Id == id);
			if (domainVehicle == null) {
				ModelState.AddModelError("Id", $"Vehicle with Id = {id} not found!");
			}

			if (!await ValidateVehicle(vehicle) || ModelState.ErrorCount > 0) {
				return BadRequest(ModelState);
			}

			try {
				_mapper.Map(vehicle, domainVehicle);
				// ReSharper disable once PossibleNullReferenceException
				domainVehicle.LastUpdate = DateTime.Now;
				await _context.SaveChangesAsync();
				VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(domainVehicle);
				result.Id = id;
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteVehicle(int id) {
			var domainVehicle = await _context.Vehicles.SingleOrDefaultAsync(v => v.Id == id);
			if (domainVehicle == null) {
				ModelState.AddModelError("Id", $"Vehicle with Id = {id} not found!");
			}

			if (ModelState.ErrorCount > 0) {
				return BadRequest(ModelState);
			}

			try {
				// ReSharper disable once AssignNullToNotNullAttribute
				_context.Vehicles.Remove(domainVehicle);
				await _context.SaveChangesAsync();
				return Ok(id);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}

		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetVehicle(int id) {
			var domainVehicle = await _context.Vehicles.Include(v => v.Features).SingleOrDefaultAsync(v => v.Id == id);
			if (domainVehicle == null) {
				return NotFound(id);
			}	
			VehicleResource result = _mapper.Map<VehicleResource>(domainVehicle);
			return Ok(result);
		}

		private async Task<bool> ValidateVehicle(VehicleResource vehicle) {
			Model model = await _context.Models.FirstOrDefaultAsync(m => m.Id == vehicle.ModelId);
			if (model == null) {
				ModelState.AddModelError("ModelId", $"Cannot find model with Id = {vehicle.ModelId}");
			}

			if (!vehicle.Features.Any()) {
				ModelState.AddModelError("Features", "Please specify features");
			} else {
				foreach (var featureId in vehicle.Features) {
					if (await _context.Features.FirstOrDefaultAsync(f => f.Id == featureId) == null) {
						ModelState.AddModelError("Features", $"Cannot find feature with Id = {featureId}");
					}
				}
			}

			return ModelState.ErrorCount == 0;
		}
	}
}
