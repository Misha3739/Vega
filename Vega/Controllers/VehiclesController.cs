using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Models;

namespace vega.Controllers {
	[Route("/api/vehicles")]
	public class VehiclesController : Controller {
		private readonly IVehiclesRepository _vehiclesRepository;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public VehiclesController(IVehiclesRepository vehiclesRepository, IUnitOfWork unitOfWork, IMapper mapper) {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_vehiclesRepository = vehiclesRepository;
		}

		[HttpPost]
		public async Task<IActionResult> CreateVehicle([FromBody] SaveVehicleResource saveVehicle) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			if (!await ValidateVehicle(saveVehicle)) {
				return BadRequest(ModelState);
			}

			try {
				Vehicle domainVehicle = _mapper.Map<SaveVehicleResource, Vehicle>(saveVehicle);
				domainVehicle.LastUpdate = DateTime.Now;
				await _vehiclesRepository.CreateAsync(domainVehicle);
				await _unitOfWork.CompeleteAsync();
				int id = domainVehicle.Id;
				domainVehicle.LastUpdate = DateTime.Now;
				domainVehicle = await _vehiclesRepository.GetWithDependenciesAsync(id);
				VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(domainVehicle);
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateVehicle(int id, [FromBody] SaveVehicleResource saveVehicle) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}
			//We need to load features to calculate existing features 
			var domainVehicle = await _vehiclesRepository.GetWithDependenciesAsync(id);
			if (domainVehicle == null) {
				ModelState.AddModelError("Id", $"Vehicle with Id = {id} not found!");
				return BadRequest(ModelState);
			}

			if (!await ValidateVehicle(saveVehicle)) {
				return BadRequest(ModelState);
			}

			try {
				_mapper.Map(saveVehicle, domainVehicle);
				// ReSharper disable once PossibleNullReferenceException
				domainVehicle.LastUpdate = DateTime.Now;
				await _unitOfWork.CompeleteAsync();
				domainVehicle = await _vehiclesRepository.GetWithDependenciesAsync(id);
				VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(domainVehicle);
				result.Id = id;
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteVehicle(int id) {
			var domainVehicle = await _vehiclesRepository.GetAsync(id);
			if (domainVehicle == null) {
				ModelState.AddModelError("Id", $"Vehicle with Id = {id} not found!");
			}

			if (ModelState.ErrorCount > 0) {
				return BadRequest(ModelState);
			}

			try {
				// ReSharper disable once AssignNullToNotNullAttribute
				 _vehiclesRepository.Delete(domainVehicle);
				await _unitOfWork.CompeleteAsync();
				return Ok(id);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetVehicle(int id) {
			Vehicle domainVehicle = await _vehiclesRepository.GetWithDependenciesAsync(id);

			if (domainVehicle == null) {
				return NotFound(id);
			}

			VehicleResource result = _mapper.Map<VehicleResource>(domainVehicle);
			return Ok(result);
		}

		private async Task<bool> ValidateVehicle(SaveVehicleResource saveVehicle) {
			if (!await _vehiclesRepository.IsModelExists(saveVehicle.ModelId)) {
				ModelState.AddModelError("ModelId", $"Cannot find model with Id = {saveVehicle.ModelId}");
			}

			if (!saveVehicle.Features.Any()) {
				ModelState.AddModelError("Features", "Please specify features");
			} else {
				foreach (var featureId in saveVehicle.Features) {
					if (!await _vehiclesRepository.IsFeatureExists(featureId)) {
						ModelState.AddModelError("Features", $"Cannot find feature with Id = {featureId}");
					}
				}
			}

			return ModelState.ErrorCount == 0;
		}
	}
}
