using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Resources;
using vega.Models;
using vega.Persistence;

namespace vega.Controllers {
    public class FeaturesController {
         private readonly VegaDbContext _context;
         private readonly IMapper mapper;
         public FeaturesController(VegaDbContext context, IMapper mapper) {
            this.mapper = mapper;
            this._context = context;

        }
        [HttpGet("/api/features")]
        public async Task<IEnumerable<FeatureResource>> GetFeatures() {
            var features = await _context.Features.ToListAsync();

            return mapper.Map<List<Feature>, List<FeatureResource>>(features);
        }
    }
}