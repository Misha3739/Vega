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
        public MakesController(IMakesRepository repository, IMapper mapper) {
            this.mapper = mapper;
            this._repository = repository;

        }
        [HttpGet("/api/makes")]
        public async Task<IEnumerable<MakeResource>> GetMakes() {
            var makes = await _repository.GetAllAsync();

            return mapper.Map<List<Make>, List<MakeResource>>(makes);
        }
    }
}