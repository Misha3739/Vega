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

		public MakesControllerTests() {
			_makesRepository = new Mock<IMakesRepository>();
			_unitOfWork = new Mock<IUnitOfWork>();
			_mapper = new Mock<IMapper>();

			_controller = new MakesController(_makesRepository.Object,  _mapper.Object, _unitOfWork.Object);
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
		}

		[Test]
		public async Task CanGetMakes() {
			_makesRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_makes);

			var makes = await _controller.GetMakesAsync();

			_makesRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
			_mapper.Verify(mapper => mapper.Map<List<Make>, List<MakeResource>>(_makes), Times.Once);
		}

		[Test]
		public async Task CanDeleteUnexistingMake() {
			const int makeId = 123;
			_makesRepository.Setup(repo => repo.GetAsync(makeId)).ReturnsAsync(default(Make));

			var actual = await _controller.DeleteAsync(makeId);

			_makesRepository.Verify(repo => repo.GetAsync(makeId), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual("Make with id = 123 does not exist!",error);
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
	}
}
