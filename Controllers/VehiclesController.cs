using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Resources;
using vega.Models;
using vega.Persistence;
using System.Linq;

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
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var model = await _context.Models.FindAsync(vehicle.ModelId);
            if(model ==  null) {
                ModelState.AddModelError("ModelId",$"Cannot find model with Id = {vehicle.ModelId}");
            }
            if(!vehicle.Features.Any()) {
                ModelState.AddModelError("Features","Please specify features"); 
            } 
            foreach(var id in vehicle.Features) {
                if(await _context.Features.FindAsync(id) == null) {
                     ModelState.AddModelError("Features",$"Cannot find feature with Id = {id}");
                }
            }
            if(ModelState.ErrorCount > 0) {
                return BadRequest(ModelState);
            }
            try {               
                 Vehicle vehicleModel = _mapper.Map<VehicleResource, Vehicle>(vehicle);
                vehicleModel.LastUpdate = DateTime.Now;
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