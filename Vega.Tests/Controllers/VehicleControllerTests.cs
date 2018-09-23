using NUnit.Framework;
using Moq;
using vega.Controllers;
using vega.Mapping;
using AutoMapper;
using vega.Persistence;
using vega.Controllers.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vega.Tests.Controllers {
    [TestFixture]
    public class VehiclesControllerTests {
        private readonly IMapper _mapper;
        private readonly Mock<IVegaDbContext> _dbContext;
        private readonly VehiclesController _controller;

        public VehiclesControllerTests() {
            _dbContext = new Mock<IVegaDbContext>();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);

            _controller = new VehiclesController(_dbContext.Object, _mapper);
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase(123)]
        public async Task CanValidateModelFields_NullContact(int? modelId) {
            var vehicleResource = new VehicleResource() {
                Contact = null,
            };

            _controller.ModelState.AddModelError("Contact","The Contact Name field is required.");

            var actual = !modelId.HasValue ? 
                await _controller.CreateVehicle(vehicleResource) :
                await _controller.UpdateVehicle(modelId.Value, vehicleResource);

            Assert.IsInstanceOf<BadRequestObjectResult>(actual);
        }

    }
}