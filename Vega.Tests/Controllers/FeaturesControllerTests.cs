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
	public class FeaturesControllerTests {
		private readonly Mock<IMapper> _mapper;
		private readonly Mock<IFeaturesRepository> _featuresRepository;
		private readonly FeaturesController _controller;

		private List<Feature> _features;

		public FeaturesControllerTests() {
			_featuresRepository = new Mock<IFeaturesRepository>();
			_mapper = new Mock<IMapper>();

			_controller = new FeaturesController(_featuresRepository.Object, _mapper.Object);
		}

		[SetUp]
		public void SetUp() {
			_features = new List<Feature>() {
				new Feature() {
					Id = 1,
					Name = "Wheels 17",
				},
				new Feature() {
					Id = 2,
					Name = "Wheels 18",
				}
			};
		}

		[Test]
		public void CanGetMakes() {
			_featuresRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_features);

			var makes = _controller.GetFeatures();

			_featuresRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
			_mapper.Verify(mapper => mapper.Map<List<Feature>, List<KeyValuePairResource>>(_features), Times.Once);
		}
	}
}