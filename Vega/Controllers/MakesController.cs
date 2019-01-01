using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Models;

namespace vega.Controllers {
	public class MakesController : Controller {
		private readonly IMakesRepository _repository;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public MakesController(IMakesRepository repository, IMapper mapper, IUnitOfWork unitOfWork) {
			this._mapper = mapper;
			this._unitOfWork = unitOfWork;
			this._repository = repository;
		}

		[HttpGet("/api/makes")]
		public async Task<IEnumerable<MakeResource>> GetMakesAsync() {
			var makes = await _repository.GetAllAsync();

			return _mapper.Map<List<Make>, List<MakeResource>>(makes);
		}

		[HttpGet("/api/makes/{id}")]
		public async Task<IActionResult> GetMakeAsync(int id) {
			var make = await _repository.GetAsync(id);
			if (make != null) {
				var result = _mapper.Map<Make, MakeResource>(make);
				return Ok(result);
			} else {
				return NotFound($"Make with id = {id} does not exist!");
			}
		}

		[HttpPost("/api/makes")]
		public async Task<IActionResult> CreateMakeAsync([FromBody] MakeResource make) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				Make domainMake = _mapper.Map<MakeResource, Make>(make);
				await _repository.CreateAsync(domainMake);
				await _unitOfWork.CompeleteAsync();
				int id = domainMake.Id;
				Make found = await _repository.GetAsync(id);
				MakeResource result = _mapper.Map<Make, MakeResource>(found);
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpPut("/api/makes/{id}")]
		public async Task<IActionResult> UpdateMakeAsync(int id, [FromBody] MakeResource make) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				Make domainMake = await _repository.GetAsync(id);
				if (domainMake == null) {
					return BadRequest($"Make with id = {id} does not exist!");
				}

				_mapper.Map(make, domainMake);
				domainMake.Id = id;
				await _unitOfWork.CompeleteAsync();
				Make found = await _repository.GetAsync(id);
				MakeResource result = _mapper.Map<Make, MakeResource>(found);
				return Ok(result);
			} catch (Exception e) {
				return StatusCode(500, e.InnerException?.Message ?? e.Message);
			}
		}

		[HttpDelete("/api/make/delete/{id}")]
		public async Task<IActionResult> DeleteAsync(int id) {
			var make = await _repository.GetAsync(id);
			if (make != null) {
				try {
					_repository.Delete(make);
					await _unitOfWork.CompeleteAsync();
					return Ok();
				} catch (Exception e) {
					return BadRequest($"Cannot delete make with id = {id} : \"{e.Message}\"");
				}
			} else {
				return BadRequest($"Make with id = {id} does not exist!");
			}
		}
	}
}
