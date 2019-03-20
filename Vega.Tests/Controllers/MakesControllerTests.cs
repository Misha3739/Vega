using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vega.Controllers;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Models;

namespace Vega.Tests.Controllers {
	[TestFixture]
	public class MakesControllerTests {
		private readonly Mock<IMapper> _mapper;
		private readonly Mock<IMakesRepository> _makesRepository;
		private readonly Mock<IUnitOfWork> _unitOfWork;
		private readonly MakesController _controller;

		private List<Make> _makes;

		private readonly MakeResource _makeResource = new MakeResource {
			Name = "Audi",
			Models = new List<KeyValuePairResource>() {
				new KeyValuePairResource() {
					Name = "Q7"
				}
			}
		};

		private readonly Make _savedMake = new Make {
			Name = "Audi",
			Models = new List<Model>() {
				new Model() {
					Name = "Q7"
				}
			}
		};

		public MakesControllerTests() {
			_makesRepository = new Mock<IMakesRepository>();
			_unitOfWork = new Mock<IUnitOfWork>();
			_mapper = new Mock<IMapper>();

			_controller = new MakesController(_makesRepository.Object, _mapper.Object, _unitOfWork.Object);
		}

		[SetUp]
		public void SetUp() {
			_makes = new List<Make>() {
				new Make() {
					Id = 1,
					Name = "Audi",
					Models = new List<Model> {
						new Model {
							Id = 1,
							Name = "Q5"
						},
						new Model {
							Id = 2,
							Name = "Q7"
						}
					}
				}
			};
		}

		[TearDown]
		public void TearDown() {
			_makesRepository.Reset();
			_controller.ModelState.Clear();
			_unitOfWork.Reset();
			_mapper.Reset();
		}

		[Test]
		public async Task CanGetMakes() {
			_makesRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_makes);

			var makes = await _controller.GetMakesAsync();

			_makesRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
			_mapper.Verify(mapper => mapper.Map<List<Make>, List<MakeResource>>(_makes), Times.Once);
		}

		[Test]
		public async Task CanGetMake() {
			const int makeId = 123;
			_makesRepository.Setup(repo => repo.GetAsync(makeId)).ReturnsAsync(_makes.First());

			var actual = await _controller.GetMakeAsync(makeId);

			_makesRepository.Verify(repo => repo.GetAsync(makeId), Times.Once);
			_mapper.Verify(m => m.Map<Make, MakeResource>(_makes.First()), Times.Once);

			Assert.IsInstanceOf<OkObjectResult>(actual);
		}

		[Test]
		public async Task CanGetMakeIfDoesNotExist() {
			const int makeId = 123;
			_makesRepository.Setup(repo => repo.GetAsync(makeId)).ReturnsAsync(default(Make));

			var actual = await _controller.GetMakeAsync(makeId);

			_makesRepository.Verify(repo => repo.GetAsync(makeId), Times.Once);
			_mapper.Verify(m => m.Map<Make, MakeResource>(_makes.First()), Times.Never);

			var error = ControllerTestHelper.GetNotFoundError(actual);
			Assert.AreEqual("Make with id = 123 does not exist!", error);
		}

		[Test]
		public async Task CanDeleteUnexistingMake() {
			const int makeId = 123;
			_makesRepository.Setup(repo => repo.GetAsync(makeId)).ReturnsAsync(default(Make));

			var actual = await _controller.DeleteAsync(makeId);

			_makesRepository.Verify(repo => repo.GetAsync(makeId), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual("Make with id = 123 does not exist!", error);
		}

		[Test]
		public async Task CanDeleteExistingMake() {
			const int makeId = 123;
			Make make = new Make() { Id = makeId, Name = "Audi" };
			_makesRepository.Setup(repo => repo.GetAsync(makeId)).ReturnsAsync(make);

			var actual = await _controller.DeleteAsync(makeId);

			_makesRepository.Verify(repo => repo.GetAsync(makeId), Times.Once);
			_makesRepository.Verify(repo => repo.Delete(make), Times.Once);
			_unitOfWork.Verify(unit => unit.CompeleteAsync(), Times.Once);
			Assert.IsInstanceOf<OkResult>(actual);
		}

		[Test]
		public async Task CanDeleteExistingMakeIfErrorOccured() {
			const int makeId = 123;
			Make make = new Make() { Id = makeId, Name = "Audi" };
			_makesRepository.Setup(repo => repo.GetAsync(makeId)).ReturnsAsync(make);
			_makesRepository.Setup(repo => repo.Delete(make)).Throws(new Exception("Sql exception occured"));

			var actual = await _controller.DeleteAsync(makeId);

			_makesRepository.Verify(repo => repo.GetAsync(makeId), Times.Once);
			_makesRepository.Verify(repo => repo.Delete(make), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual($"Cannot delete make with id = 123 : \"Sql exception occured\"", error);
		}

		[Test]
		public async Task CanValidateMake_CreateMake_RequiredFielsAreMissing() {
			MakeResource makeResource = new MakeResource {
				Name = null
			};

			_controller.ModelState.AddModelError("Name", "Name field is required.");

			IActionResult actual = await _controller.CreateMakeAsync(makeResource);

			_makesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			var errors = ControllerTestHelper.GetBadRequestErrors(actual);
			Assert.AreEqual(new[] { "Name field is required." }, errors["Name"]);
		}

		[Test]
		public async Task CanCreateMake() {
			_mapper.Setup(mapper => mapper.Map<MakeResource, Make>(_makeResource)).Returns(_savedMake);
			_makesRepository.Setup(db => db.GetAsync(It.IsAny<int>())).Returns(Task.FromResult(_savedMake));

			IActionResult actual = await _controller.CreateMakeAsync(_makeResource);

			_mapper.Verify(mapper => mapper.Map<MakeResource, Make>(_makeResource), Times.Once);
			_makesRepository.Verify(db => db.CreateAsync(_savedMake), Times.Once);
			_unitOfWork.Verify(db => db.CompeleteAsync(), Times.Once);
			_makesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Once);
			_mapper.Verify(mapper => mapper.Map<Make, MakeResource>(_savedMake), Times.Once);
			Assert.IsInstanceOf<OkObjectResult>(actual);
		}

		[Test]
		public async Task CanCreateMake_DoesNotThrowException() {
			_mapper.Setup(mapper => mapper.Map<MakeResource, Make>(_makeResource)).Returns(_savedMake);
			_unitOfWork.Setup(db => db.CompeleteAsync()).Throws(new Exception("SQL Statement error"));

			IActionResult actual = await _controller.CreateMakeAsync(_makeResource);

			_mapper.Verify(mapper => mapper.Map<MakeResource, Make>(_makeResource), Times.Once);
			_makesRepository.Verify(db => db.CreateAsync(_savedMake), Times.Once);
			_makesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			_mapper.Verify(mapper => mapper.Map<Make, MakeResource>(It.IsAny<Make>()), Times.Never);
			Assert.AreEqual(500, (actual as ObjectResult)?.StatusCode);
			Assert.AreEqual("SQL Statement error", (actual as ObjectResult)?.Value?.ToString());
		}

		[Test]
		public async Task CanUpdateMake() {
			const int id = 123;
			_makesRepository.Setup(db => db.GetAsync(123)).Returns(Task.FromResult(_savedMake));

			IActionResult actual = await _controller.UpdateMakeAsync(id, _makeResource);

			_mapper.Verify(mapper => mapper.Map(_makeResource, _savedMake), Times.Once);
			_makesRepository.Verify(db => db.CreateAsync(_savedMake), Times.Never);
			_unitOfWork.Verify(db => db.CompeleteAsync(), Times.Once);
			_makesRepository.Verify(db => db.GetAsync(id), Times.Exactly(2));
			_mapper.Verify(mapper => mapper.Map<Make, MakeResource>(_savedMake), Times.Once);
			Assert.IsInstanceOf<OkObjectResult>(actual);
		}

		[Test]
		public async Task CanUpdateMake_DoesNotExist() {
			const int id = 123;
			_makesRepository.Setup(db => db.GetAsync(123)).Returns(Task.FromResult(default(Make)));

			IActionResult actual = await _controller.UpdateMakeAsync(id, _makeResource);

			_makesRepository.Verify(db => db.GetAsync(id), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual("Make with id = 123 does not exist!", error);
		}

		[Test]
		public async Task CanUpdateMake_DoesNotThrowException() {
			const int id = 123;
			_makesRepository.Setup(db => db.GetAsync(123)).Returns(Task.FromResult(_savedMake));
			_unitOfWork.Setup(db => db.CompeleteAsync()).Throws(new Exception("SQL Statement error"));

			IActionResult actual = await _controller.UpdateMakeAsync(id,_makeResource);

			_makesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Once);
			_mapper.Verify(mapper => mapper.Map(_makeResource, _savedMake), Times.Once);
			_mapper.Verify(mapper => mapper.Map<Make, MakeResource>(It.IsAny<Make>()), Times.Never);
			Assert.AreEqual(500, (actual as ObjectResult)?.StatusCode);
			Assert.AreEqual("SQL Statement error", (actual as ObjectResult)?.Value?.ToString());
		}

		[Test]
		public async Task CanValidateMake_UpdateMake_RequiredFielsAreMissing() {
			MakeResource makeResource = new MakeResource {
				Name = null
			};

			_controller.ModelState.AddModelError("Name", "Name field is required.");

			IActionResult actual = await _controller.UpdateMakeAsync(It.IsAny<int>(), makeResource);

			_makesRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			var errors = ControllerTestHelper.GetBadRequestErrors(actual);
			Assert.AreEqual(new[] { "Name field is required." }, errors["Name"]);
		}
	}
}
