using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Resources;
using vega.Models;
using vega.Persistence;

namespace vega.Controllers {
    [Route("/api/vehicles")]
    public class VehiclesController : Controller {
         private readonly VegaDbContext _context;
         private readonly IMapper _mapper;
         public VehiclesController(VegaDbContext context, IMapper mapper) {
            this._mapper = mapper;
            this._context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleResource vehicle) {
            Vehicle vehicleModel = _mapper.Map<VehicleResource, Vehicle>(vehicle);
            try {
                _context.Vehicles.Add(vehicleModel);
                await _context.SaveChangesAsync();
                VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(vehicleModel);
                return Ok(result);
            } catch(Exception e) {
                return StatusCode(500, e.InnerException?.Message ?? e.Message);
            }
        }
    }
}