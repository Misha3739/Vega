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

            if(!await ValidateVehicle(vehicle)) {
                return BadRequest(ModelState);
            }

            try {               
                Vehicle domainVehicle = _mapper.Map<VehicleResource, Vehicle>(vehicle);
                domainVehicle.LastUpdate = DateTime.Now;
                _context.Vehicles.Add(domainVehicle);
                await _context.SaveChangesAsync();
                VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(domainVehicle);
                return Ok(result);
            } catch(Exception e) {
                return StatusCode(500, e.InnerException?.Message ?? e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleResource vehicle) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var domainVehicle = await _context.Vehicles.FindAsync(id);
            if(domainVehicle ==  null) {
                ModelState.AddModelError("id",$"Model with Id = {id} not found!");
            }
           
            if(!await ValidateVehicle(vehicle)) {
                return BadRequest(ModelState);
            }

            try {               
                _mapper.Map<VehicleResource,Vehicle>(vehicle, domainVehicle);
                domainVehicle.LastUpdate = DateTime.Now;
                await _context.SaveChangesAsync();
                VehicleResource result = _mapper.Map<Vehicle, VehicleResource>(domainVehicle);
                return Ok(result);
            } catch(Exception e) {
                return StatusCode(500, e.InnerException?.Message ?? e.Message);
            }
        }

        private async Task<bool> ValidateVehicle(VehicleResource vehicle) {
            Model model = await _context.Models.FindAsync(vehicle.ModelId);
            if(model ==  null) {
                ModelState.AddModelError("ModelId",$"Cannot find model with Id = {vehicle.ModelId}");
            }
            if(!vehicle.Features.Any()) {
                ModelState.AddModelError("Features","Please specify features"); 
            } 
            foreach(var featureId in vehicle.Features) {
                if(await _context.Features.FindAsync(featureId) == null) {
                     ModelState.AddModelError("Features",$"Cannot find feature with Id = {featureId}");
                }
            }

            return ModelState.ErrorCount == 0;
        }
    }
}