using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using vega.Controllers;
using vega.Controllers.Resources;
using vega.Mapping;
using vega.Models;
using vega.Persistence;

namespace Vega.Tests.Controllers {
	[TestFixture]
	public class VehiclesControllerTests {
		private readonly IMapper _mapper;
		private readonly Mock<IVegaDbContext> _dbContext;
		private VehiclesController _controller;

		public VehiclesControllerTests() {
			_dbContext = new Mock<IVegaDbContext>();
			var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
			_mapper = new Mapper(config);
		}

		[SetUp]
		public void SetUp() {
			List<Model> models = new List<Model> {
				new Model {
					Id = 2,
					Name = "Audi Q5"
				},
				new Model {
					Id = 3,
					Name = "Audi Q7"
				}
			};
			_dbContext.Setup(c => c.Models).Returns(GetQueryableMockDbSet(models));

			List<Feature> features = new List<Feature> {
				new Feature {
					Id = 5,
					Name = "Feature 5"
				},
				new Feature {
					Id = 6,
					Name = "Feature 6"
				},
				new Feature {
					Id = 7,
					Name = "Feature 7"
				},
				new Feature {
					Id = 8,
					Name = "Feature 8"
				}
			};
			_dbContext.Setup(c => c.Features).Returns(GetQueryableMockDbSet(features));

			var vehicles = new List<Vehicle>() {
				new Vehicle() {
					Id = 123,
					ModelId = 2,
					ContactEmail = "Name@mail.com",
					ContactName = "Name",
					ContactPhone = "3333333",
					Features = new List<VehicleFeature>() {
						new VehicleFeature() {
							FeatureId = 5,
							VehicleId = 23
						}
					}
				}
			};
			_dbContext.Setup(c => c.Vehicles).Returns(GetQueryableMockDbSet(vehicles));
			_controller = new VehiclesController(_dbContext.Object, _mapper);
		}

		[TearDown]
		public void TearDown() {
			_dbContext.Reset();
			_controller.ModelState.Clear();
		}

		[TestCase(null)]
		[TestCase(123)]
		public async Task CanValidateModelFields_RequiredFielsAreMissing(int? vehicleId) {
			var vehicleResource = new VehicleResource {
				Contact = null
			};

			_controller.ModelState.AddModelError("Contact", "The Contact Name field is required.");

			var actual = !vehicleId.HasValue ? await _controller.CreateVehicle(vehicleResource) : await _controller.UpdateVehicle(vehicleId.Value, vehicleResource);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
		}

		[TestCase(null)]
		[TestCase(123)]
		public async Task CanValidateModelFields_ModelDoesNotExistAndFeatureListIsEmpty(int? vehicleId) {
			var vehicleResource = new VehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 1
			};

			var actual = !vehicleId.HasValue ? await _controller.CreateVehicle(vehicleResource) : await _controller.UpdateVehicle(vehicleId.Value, vehicleResource);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(2, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Please specify features" }, errorList["Features"] as string[]);
		}

		[TestCase(null)]
		[TestCase(123)]
		public async Task CanValidateModelFields_ModelAndFeaturesDoNotExist(int? vehicleId) {
			var vehicleResource = new VehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 1,
				Features = new List<int> { 3, 5, 7, 9 }
			};

			var actual = !vehicleId.HasValue ? await _controller.CreateVehicle(vehicleResource) : await _controller.UpdateVehicle(vehicleId.Value, vehicleResource);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(2, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Cannot find feature with Id = 3", "Cannot find feature with Id = 9" }, errorList["Features"] as string[]);
		}

		[Test]
		public async Task CanValidateModelFields_UpdateVehicle_VehicleAndModelDoNotExist() {
			var vehicleResource = new VehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 1
			};

			var actual = await _controller.UpdateVehicle(122, vehicleResource);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(3, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Vehicle with Id = 122 not found!" }, errorList["Id"] as string[]);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Please specify features" }, errorList["Features"] as string[]);
		}

		[Test]
		public async Task CanValidateModelFields_UpdateVehicle_VehicleDoesNotExist() {
			var vehicleResource = new VehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 2,
				Features = new List<int> { 5, 6 }
			};

			var actual = await _controller.UpdateVehicle(122, vehicleResource);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(1, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Vehicle with Id = 122 not found!" }, errorList["Id"] as string[]);
		}

		private static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class {
			var queryable = sourceList.AsQueryable();
			return queryable.BuildMockDbSet().Object;
		}
	}
}
