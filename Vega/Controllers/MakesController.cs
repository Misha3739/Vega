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
		private readonly IMapper mapper;
		private readonly IUnitOfWork _unitOfWork;

		public MakesController(IMakesRepository repository, IMapper mapper, IUnitOfWork unitOfWork) {
			this.mapper = mapper;
			this._unitOfWork = unitOfWork;
			this._repository = repository;
		}

		[HttpGet("/api/makes")]
		public async Task<IEnumerable<MakeResource>> GetMakesAsync() {
			var makes = await _repository.GetAllAsync();

			return mapper.Map<List<Make>, List<MakeResource>>(makes);
		}

		[HttpDelete("/api/makes/delete/{id}")]
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
