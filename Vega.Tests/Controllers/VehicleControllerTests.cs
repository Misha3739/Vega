using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vega.Controllers;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Mapping;
using vega.Models;

namespace Vega.Tests.Controllers {
	[TestFixture]
	public class VehiclesControllerTests {
		private readonly IMapper _mapper;
		private readonly Mock<IVehiclesRepository> _vehiclesRepository;
		private readonly VehiclesController _controller;
		private readonly Mock<IUnitOfWork> _unitOfWork;

		private Vehicle _vehicleWithId123;
		private Vehicle _createdVehicle, _updatedVehcile;

		public VehiclesControllerTests() {
			_vehiclesRepository = new Mock<IVehiclesRepository>();
			MapperConfiguration config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
			_mapper = new Mapper(config);
			_unitOfWork = new Mock<IUnitOfWork>();

			_controller = new VehiclesController(_vehiclesRepository.Object, _unitOfWork.Object, _mapper);
		}

		[SetUp]
		public void SetUp() {
			_vehicleWithId123 = new Vehicle {
				Id = 123,
				ModelId = 2,
				Model = new Model {
					Id = 2,
					Name = "Audi Q5"
				},
				ContactEmail = "Name@mail.com",
				ContactName = "Name",
				ContactPhone = "3333333",
				Features = new List<VehicleFeature> {
					new VehicleFeature {
						FeatureId = 5,
						Feature = new Feature {
							Id = 5,
							Name = "Feature 5"
						},
						VehicleId = 123
					}
				}
			};

			_createdVehicle = new Vehicle {
				ContactName = "Person",
				ContactEmail = "Person@mail.com",
				ContactPhone = "2222222",
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 2,
				Model = new Model {
					Id = 2,
					Name = "Q5"
				},
				Features = new List<VehicleFeature> {
					new VehicleFeature {
						FeatureId = 5,
						Feature = new Feature {
							Id = 5,
							Name = "Feature 5"
						},
						VehicleId = 123
					},
					new VehicleFeature {
						FeatureId = 6,
						Feature = new Feature {
							Id = 6,
							Name = "Feature 6"
						},
						VehicleId = 123
					}
				}
			};

			_updatedVehcile = new Vehicle {
				ContactName = "Person",
				ContactEmail = "Person@mail.com",
				ContactPhone = "2222222",
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 3,
				Model = new Model {
					Id = 3,
					Name = "Q7"
				},
				Features = new List<VehicleFeature> {
					new VehicleFeature {
						FeatureId = 5,
						Feature = new Feature {
							Id = 5,
							Name = "Feature 5"
						},
						VehicleId = 123
					},
					new VehicleFeature {
						FeatureId = 6,
						Feature = new Feature {
							Id = 6,
							Name = "Feature 6"
						},
						VehicleId = 123
					},
					new VehicleFeature {
						FeatureId = 8,
						Feature = new Feature {
							Id = 8,
							Name = "Feature 6"
						},
						VehicleId = 123
					}
				}
			};
		}

		[TearDown]
		public void TearDown() {
			_vehiclesRepository.Reset();
			_controller.ModelState.Clear();
			_unitOfWork.Reset();
		}

		[Test]
		public async Task CanValidateModelFields_CreateVehicle_RequiredFielsAreMissing() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
				Contact = null
			};

			_controller.ModelState.AddModelError("Contact", "The Contact Name field is required.");

			IActionResult actual = await _controller.CreateVehicleAsync(vehicleResource);

			_vehiclesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsModelExists(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Never);
			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
		}
		
		[Test]
		public async Task CanValidateModelFields_UpdateVehicle_RequiredFielsAreMissing() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
				Contact = null
			};

			_controller.ModelState.AddModelError("Contact", "The Contact Name field is required.");

			IActionResult actual = await _controller.UpdateVehicleAsync(123, vehicleResource);

			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsModelExists(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Never);
			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
		}

		[Test]
		public async Task CanValidateModelFields_CreateVehicle_ModelDoesNotExistAndFeaturesEmpty() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 1
			};

			_vehiclesRepository.Setup(db => db.IsModelExists(1)).ReturnsAsync(false);

			IActionResult actual = await _controller.CreateVehicleAsync(vehicleResource);

			_vehiclesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsModelExists(1), Times.Once);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Never);
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Never);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(2, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Please specify features" }, errorList["Features"] as string[]);
		}

		[Test]
		public async Task CanValidateModelFields_CreateVehicle_ModelAndFeaturesDoNotExistInDb() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
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

			_vehiclesRepository.Setup(db => db.IsModelExists(1)).ReturnsAsync(false);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(It.IsInRange(5, 7, Range.Inclusive))).ReturnsAsync(true);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(3)).ReturnsAsync(false);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(9)).ReturnsAsync(false);

			IActionResult actual = await _controller.CreateVehicleAsync(vehicleResource);

			_vehiclesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsModelExists(1), Times.Once);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Exactly(4));
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Never);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(2, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Cannot find feature with Id = 3", "Cannot find feature with Id = 9" }, errorList["Features"] as string[]);
		}

		[Test]
		public async Task CanValidateModelFields_UpdateVehicle_VehicleDoesNotExist() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
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

			_vehiclesRepository.Setup(db => db.GetWithDependenciesAsync(122)).ReturnsAsync(default(Vehicle));

			IActionResult actual = await _controller.UpdateVehicleAsync(122, vehicleResource);

			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(122), Times.Once);
			_vehiclesRepository.Verify(db => db.IsModelExists(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Never);
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Never);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(1, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Vehicle with Id = 122 not found!" }, errorList["Id"] as string[]);
		}

		[Test]
		public async Task CanValidateModelFields_UpdateVehicle_ModelAndFeatureListIsEmpty() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = DateTime.Now,
				IsRegistered = true,
				ModelId = 1
			};

			_vehiclesRepository.Setup(db => db.GetWithDependenciesAsync(123)).ReturnsAsync(_vehicleWithId123);
			_vehiclesRepository.Setup(db => db.IsModelExists(1)).ReturnsAsync(false);

			IActionResult actual = await _controller.UpdateVehicleAsync(123, vehicleResource);

			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(123), Times.Once);
			_vehiclesRepository.Verify(db => db.IsModelExists(1), Times.Once);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Never);
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Never);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(2, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Please specify features" }, errorList["Features"] as string[]);
		}

		[Test]
		public async Task CanValidateModelFields_UpdateVehicle_ModelAndFeaturesDoNotExistInDb() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
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

			_vehiclesRepository.Setup(db => db.GetWithDependenciesAsync(123)).ReturnsAsync(_vehicleWithId123);
			_vehiclesRepository.Setup(db => db.IsModelExists(1)).ReturnsAsync(false);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(It.IsInRange(5, 7, Range.Inclusive))).ReturnsAsync(true);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(3)).ReturnsAsync(false);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(9)).ReturnsAsync(false);

			IActionResult actual = await _controller.UpdateVehicleAsync(123, vehicleResource);

			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(123), Times.Once);
			_vehiclesRepository.Verify(db => db.IsModelExists(1), Times.Once);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Exactly(4));
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Never);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(2, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Cannot find model with Id = 1" }, errorList["ModelId"] as string[]);
			CollectionAssert.AreEqual(new[] { "Cannot find feature with Id = 3", "Cannot find feature with Id = 9" }, errorList["Features"] as string[]);
		}

		[Test]
		public async Task DeleteVehicle_CanValidateInput() {
			IActionResult actual = await _controller.DeleteVehicleAsync(122);

			_vehiclesRepository.Verify(db => db.GetAsync(122), Times.Once);
			_vehiclesRepository.Verify(db => db.Delete(It.IsAny<Vehicle>()), Times.Never);
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Never);

			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			Assert.IsNotNull(errorList);
			Assert.AreEqual(1, errorList.Count);
			CollectionAssert.AreEqual(new[] { "Vehicle with Id = 122 not found!" }, errorList["Id"] as string[]);
		}

		[Test]
		public async Task GetVehicle_CanValidateInput() {
			IActionResult actual = await _controller.GetVehicleAsync(122);

			Assert.IsInstanceOf<NotFoundObjectResult>(actual);
			Assert.AreEqual(122, (int) ((NotFoundObjectResult) actual).Value);
		}

		[Test]
		public async Task CanCreateNewVehicle() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = new DateTime(2018, 1, 10, 15, 20, 33),
				IsRegistered = true,
				ModelId = 2,
				Features = new List<int> { 5, 6 }
			};

			_vehiclesRepository.Setup(db => db.IsModelExists(2)).ReturnsAsync(true);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(It.IsInRange(5, 6, Range.Inclusive))).ReturnsAsync(true);
			//Stub reload entity after saving, no Id specified
			_vehiclesRepository.Setup(db => db.GetWithDependenciesAsync(It.IsAny<int>())).ReturnsAsync(_createdVehicle);

			IActionResult actual = await _controller.CreateVehicleAsync(vehicleResource);

			_vehiclesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			_vehiclesRepository.Verify(db => db.IsModelExists(2), Times.Once);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Exactly(2));
			_vehiclesRepository.Verify(db => db.CreateAsync(It.Is<Vehicle>(v => v.ModelId == 2 && v.ContactEmail == "Person@mail.com")), Times.Once);
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Once);
			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(It.IsAny<int>()), Times.Once);

			Assert.IsInstanceOf<OkObjectResult>(actual);
			VehicleResource returnResult = ((OkObjectResult) actual).Value as VehicleResource;
			Assert.IsNotNull(returnResult);
			Assert.AreEqual("Person", returnResult.Contact.Name);
			Assert.AreEqual("Person@mail.com", returnResult.Contact.Email);
			Assert.AreEqual("2222222", returnResult.Contact.Phone);
			Assert.AreNotEqual(new DateTime(2018, 1, 10, 15, 20, 33), returnResult.LastUpdate);
			CollectionAssert.AreEqual(new List<int> { 5, 6 }, returnResult.Features.Select(f => f.Id));
			Assert.IsTrue(returnResult.IsRegistered);
			Assert.AreEqual(2, returnResult.Model.Id);
		}

		[Test]
		public async Task CanUpdateVehicle() {
			SaveVehicleResource vehicleResource = new SaveVehicleResource {
				Contact = new ContacResource {
					Name = "Person",
					Email = "Person@mail.com",
					Phone = "2222222"
				},
				LastUpdate = new DateTime(2018, 1, 10, 15, 20, 33),
				IsRegistered = true,
				ModelId = 3,
				Features = new List<int> { 5, 6, 8 }
			};

			_vehiclesRepository.Setup(db => db.GetWithDependenciesAsync(123)).ReturnsAsync(_updatedVehcile);
			_vehiclesRepository.Setup(db => db.IsModelExists(3)).ReturnsAsync(true);
			_vehiclesRepository.Setup(db => db.IsFeatureExists(It.IsInRange(5, 8, Range.Inclusive))).ReturnsAsync(true);

			IActionResult actual = await _controller.UpdateVehicleAsync(123, vehicleResource);

			_vehiclesRepository.Verify(db => db.IsModelExists(3), Times.Once);
			_vehiclesRepository.Verify(db => db.IsFeatureExists(It.IsAny<int>()), Times.Exactly(3));
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Once);
			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(123), Times.Exactly(2));

			Assert.IsInstanceOf<OkObjectResult>(actual);
			VehicleResource returnResult = ((OkObjectResult) actual).Value as VehicleResource;
			Assert.IsNotNull(returnResult);
			Assert.AreEqual("Person", returnResult.Contact.Name);
			Assert.AreEqual("Person@mail.com", returnResult.Contact.Email);
			Assert.AreEqual("2222222", returnResult.Contact.Phone);
			Assert.AreNotEqual(new DateTime(2018, 1, 10, 15, 20, 33), returnResult.LastUpdate);
			CollectionAssert.AreEqual(new List<int> { 5, 6, 8 }, returnResult.Features.Select(f => f.Id));
			Assert.IsTrue(returnResult.IsRegistered);
			Assert.AreEqual(3, returnResult.Model.Id);
		}

		[Test]
		public async Task CanDeleteVehicle() {
			_vehiclesRepository.Setup(db => db.GetAsync(123)).ReturnsAsync(_vehicleWithId123);

			IActionResult actual = await _controller.DeleteVehicleAsync(123);

			_vehiclesRepository.Verify(db => db.GetAsync(123), Times.Once);
			_vehiclesRepository.Verify(db => db.Delete(It.Is<Vehicle>(v => v.Equals(_vehicleWithId123))), Times.Once);
			_unitOfWork.Verify(u => u.CompeleteAsync(), Times.Once);

			Assert.IsInstanceOf<OkObjectResult>(actual);
			Assert.AreEqual(123, (int) ((OkObjectResult) actual).Value);
			_vehiclesRepository.Verify(db => db.Delete(It.Is<Vehicle>(v => v.Id == 123)), Times.Once);
		}

		[Test]
		public async Task CanGetVehicle() {
			_vehiclesRepository.Setup(db => db.GetWithDependenciesAsync(123)).ReturnsAsync(_vehicleWithId123);

			IActionResult actual = await _controller.GetVehicleAsync(123);

			_vehiclesRepository.Verify(db => db.GetWithDependenciesAsync(123), Times.Once);
			_vehiclesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);

			Assert.IsInstanceOf<VehicleResource>(((OkObjectResult) actual).Value);
			VehicleResource result = ((OkObjectResult) actual).Value as VehicleResource;
			Assert.IsNotNull(result);
			Assert.AreEqual(123, result.Id);
			Assert.AreEqual(2, result.Model.Id);
			Assert.AreEqual("Audi Q5", result.Model.Name);
			Assert.AreEqual("Name@mail.com", result.Contact.Email);
			Assert.AreEqual(1, result.Features.Count);
			Assert.AreEqual(5, result.Features.First().Id);
			Assert.AreEqual("Feature 5", result.Features.First().Name);
		}
	}
}
