using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Models;

namespace vega.Controllers {
	public class FeaturesController {
		private readonly IFeaturesRepository _repository;
		private readonly IMapper mapper;

		public FeaturesController(IFeaturesRepository repository, IMapper mapper) {
			this.mapper = mapper;
			this._repository = repository;
		}

		[HttpGet("/api/features")]
		public async Task<IEnumerable<KeyValuePairResource>> GetFeaturesAsync() {
			var features = await _repository.GetAllAsync();

			return mapper.Map<List<Feature>, List<KeyValuePairResource>>(features);
		}
	}
}
