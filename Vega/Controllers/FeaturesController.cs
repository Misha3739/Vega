using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Models;

namespace vega.Controllers {
	public class FeaturesController : Controller {
		private readonly IFeaturesRepository _repository;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public FeaturesController(IFeaturesRepository repository, IMapper mapper, IUnitOfWork unitOfWork) {
			this._mapper = mapper;
			_unitOfWork = unitOfWork;
			this._repository = repository;
		}

		[HttpGet("/api/features")]
		public async Task<IEnumerable<KeyValuePairResource>> GetFeaturesAsync() {
			var features = await _repository.GetAllAsync();

			return _mapper.Map<List<Feature>, List<KeyValuePairResource>>(features);
		}

		[HttpGet("/api/features/{id}")]
		public async Task<IActionResult> GetFeatureAsync(int id) {
			var make = await _repository.GetAsync(id);
			if (make != null) {
				var result = _mapper.Map<Feature, KeyValuePairResource>(make);
				return Ok(result);
			} else {
				return NotFound($"Feature with id = {id} does not exist!");
			}
		}

		[HttpPost("/api/features")]
		public async Task<IActionResult> CreateFeatureAsync([FromBody] FeatureResource feature) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				Feature domainFeature = _mapper.Map<FeatureResource, Feature>(feature);
				await _repository.CreateAsync(domainFeature);
				await _unitOfWork.CompeleteAsync();
				int id = domainFeature.Id;
				Feature found = await _repository.GetAsync(id);
				KeyValuePairResource result = _mapper.Map<Feature, KeyValuePairResource>(found);
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpPut("/api/features/{id}")]
		public async Task<IActionResult> UpdateFeatureAsync(int id, [FromBody] FeatureResource feature) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				Feature domainFeature = await _repository.GetAsync(id);
				if (domainFeature == null) {
					return BadRequest($"Feature with id = {id} does not exist!");
				}

				_mapper.Map(feature, domainFeature);
				domainFeature.Id = id;
				await _unitOfWork.CompeleteAsync();
				Feature found = await _repository.GetAsync(id);
				KeyValuePairResource result = _mapper.Map<Feature, KeyValuePairResource>(found);
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpDelete("/api/feature/delete/{id}")]
		public async Task<IActionResult> DeleteAsync(int id) {
			var Feature = await _repository.GetAsync(id);
			if (Feature != null) {
				try {
					_repository.Delete(Feature);
					await _unitOfWork.CompeleteAsync();
					return Ok();
				} catch (Exception e) {
					return BadRequest($"Cannot delete Feature with id = {id} : \"{e.Message}\"");
				}
			} else {
				return BadRequest($"Feature with id = {id} does not exist!");
			}
		}
	}
}
