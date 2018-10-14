using System.Collections.Generic;
using AutoMapper;
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
		private readonly MakesController _controller;

		private List<Make> _makes;

		public MakesControllerTests() {
			_makesRepository = new Mock<IMakesRepository>();
			_mapper = new Mock<IMapper>();

			_controller = new MakesController(_makesRepository.Object, _mapper.Object);
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

		[Test]
		public void CanGetMakes() {
			_makesRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_makes);

			var makes = _controller.GetMakes();

			_makesRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
			_mapper.Verify(mapper => mapper.Map<List<Make>, List<MakeResource>>(_makes), Times.Once);
		}
	}
}
