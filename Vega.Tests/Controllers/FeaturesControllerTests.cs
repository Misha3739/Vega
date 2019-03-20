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
using vega.Models;

namespace Vega.Tests.Controllers {
	[TestFixture]
	public class FeaturesControllerTests {
		private readonly Mock<IMapper> _mapper;
		private readonly Mock<IFeaturesRepository> _featuresRepository;
		private readonly Mock<IUnitOfWork> _unitOfWork;
		private readonly FeaturesController _controller;

		private List<Feature> _features;

		private readonly FeatureResource _featureResource = new FeatureResource {
			Name = "18R Wheels",
		};

		private readonly Feature _savedFeature = new Feature {
			Name = "Automatic Shift",
		};

		public FeaturesControllerTests() {
			_featuresRepository = new Mock<IFeaturesRepository>();
			_mapper = new Mock<IMapper>();
			_unitOfWork = new Mock<IUnitOfWork>();

			_controller = new FeaturesController(_featuresRepository.Object, _mapper.Object, _unitOfWork.Object);
		}

		[SetUp]
		public void SetUp() {
			_features = new List<Feature>() {
				new Feature() {
					Id = 123,
					Name = "Wheels 17",
				},
				new Feature() {
					Id = 2,
					Name = "Wheels 18",
				}
			};
		}

		[TearDown]
		public void TearDown() {
			_featuresRepository.Reset();
			_controller.ModelState.Clear();
			_unitOfWork.Reset();
			_mapper.Reset();
		}

		[Test]
		public void CanGetFeatures() {
			_featuresRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_features);

			var Features = _controller.GetFeaturesAsync();

			_featuresRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
			_mapper.Verify(mapper => mapper.Map<List<Feature>, List<KeyValuePairResource>>(_features), Times.Once);
		}

		[Test]
		public async Task CanGetFeature() {
			const int featureId = 123;
			_featuresRepository.Setup(repo => repo.GetAsync(featureId)).ReturnsAsync(_features.First());

			var actual = await _controller.GetFeatureAsync(featureId);

			_featuresRepository.Verify(repo => repo.GetAsync(featureId), Times.Once);
			_mapper.Verify(m => m.Map<Feature, KeyValuePairResource>(_features.First()), Times.Once);

			Assert.IsInstanceOf<OkObjectResult>(actual);
		}

		[Test]
		public async Task CanGetFeatureIfDoesNotExist() {
			const int featureId = 123;
			_featuresRepository.Setup(repo => repo.GetAsync(featureId)).ReturnsAsync(default(Feature));

			var actual = await _controller.GetFeatureAsync(featureId);

			_featuresRepository.Verify(repo => repo.GetAsync(featureId), Times.Once);
			_mapper.Verify(m => m.Map<Feature, KeyValuePairResource>(_features.First()), Times.Never);

			var error = ControllerTestHelper.GetNotFoundError(actual);
			Assert.AreEqual("Feature with id = 123 does not exist!", error);
		}

		[Test]
		public async Task CanDeleteUnexistingFeature() {
			const int featureId = 123;
			_featuresRepository.Setup(repo => repo.GetAsync(featureId)).ReturnsAsync(default(Feature));

			var actual = await _controller.DeleteAsync(featureId);

			_featuresRepository.Verify(repo => repo.GetAsync(featureId), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual("Feature with id = 123 does not exist!", error);
		}

		[Test]
		public async Task CanDeleteExistingFeature() {
			const int featureId = 123;
			Feature Feature = new Feature() { Id = featureId, Name = "Audi" };
			_featuresRepository.Setup(repo => repo.GetAsync(featureId)).ReturnsAsync(Feature);

			var actual = await _controller.DeleteAsync(featureId);

			_featuresRepository.Verify(repo => repo.GetAsync(featureId), Times.Once);
			_featuresRepository.Verify(repo => repo.Delete(Feature), Times.Once);
			_unitOfWork.Verify(unit => unit.CompeleteAsync(), Times.Once);
			Assert.IsInstanceOf<OkResult>(actual);
		}

		[Test]
		public async Task CanDeleteExistingFeatureIfErrorOccured() {
			const int featureId = 123;
			Feature Feature = new Feature() { Id = featureId, Name = "Audi" };
			_featuresRepository.Setup(repo => repo.GetAsync(featureId)).ReturnsAsync(Feature);
			_featuresRepository.Setup(repo => repo.Delete(Feature)).Throws(new Exception("Sql exception occured"));

			var actual = await _controller.DeleteAsync(featureId);

			_featuresRepository.Verify(repo => repo.GetAsync(featureId), Times.Once);
			_featuresRepository.Verify(repo => repo.Delete(Feature), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual($"Cannot delete Feature with id = 123 : \"Sql exception occured\"", error);
		}

		[Test]
		public async Task CanValidateFeature_CreateFeature_RequiredFielsAreMissing() {
			FeatureResource FeatureResource = new FeatureResource {
				Name = null
			};

			_controller.ModelState.AddModelError("Name", "Name field is required.");

			IActionResult actual = await _controller.CreateFeatureAsync(FeatureResource);

			_featuresRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			var errors = ControllerTestHelper.GetBadRequestErrors(actual);
			Assert.AreEqual(new[] { "Name field is required." }, errors["Name"]);
		}

		[Test]
		public async Task CanCreateFeature() {
			_mapper.Setup(mapper => mapper.Map<FeatureResource, Feature>(_featureResource)).Returns(_savedFeature);
			_featuresRepository.Setup(db => db.GetAsync(It.IsAny<int>())).Returns(Task.FromResult(_savedFeature));

			IActionResult actual = await _controller.CreateFeatureAsync(_featureResource);

			_mapper.Verify(mapper => mapper.Map<FeatureResource, Feature>(_featureResource), Times.Once);
			_featuresRepository.Verify(db => db.CreateAsync(_savedFeature), Times.Once);
			_unitOfWork.Verify(db => db.CompeleteAsync(), Times.Once);
			_featuresRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Once);
			_mapper.Verify(mapper => mapper.Map<Feature, KeyValuePairResource>(_savedFeature), Times.Once);
			Assert.IsInstanceOf<OkObjectResult>(actual);
		}

		[Test]
		public async Task CanCreateFeature_DoesNotThrowException() {
			_mapper.Setup(mapper => mapper.Map<FeatureResource, Feature>(_featureResource)).Returns(_savedFeature);
			_unitOfWork.Setup(db => db.CompeleteAsync()).Throws(new Exception("SQL Statement error"));

			IActionResult actual = await _controller.CreateFeatureAsync(_featureResource);

			_mapper.Verify(mapper => mapper.Map<FeatureResource, Feature>(_featureResource), Times.Once);
			_featuresRepository.Verify(db => db.CreateAsync(_savedFeature), Times.Once);
			_featuresRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			_mapper.Verify(mapper => mapper.Map<Feature, KeyValuePairResource>(It.IsAny<Feature>()), Times.Never);
			Assert.AreEqual(500, (actual as ObjectResult)?.StatusCode);
			Assert.AreEqual("SQL Statement error", (actual as ObjectResult)?.Value?.ToString());
		}

		[Test]
		public async Task CanUpdateFeature() {
			const int id = 123;
			_featuresRepository.Setup(db => db.GetAsync(123)).Returns(Task.FromResult(_savedFeature));

			IActionResult actual = await _controller.UpdateFeatureAsync(id, _featureResource);

			_mapper.Verify(mapper => mapper.Map(_featureResource, _savedFeature), Times.Once);
			_featuresRepository.Verify(db => db.CreateAsync(_savedFeature), Times.Never);
			_unitOfWork.Verify(db => db.CompeleteAsync(), Times.Once);
			_featuresRepository.Verify(db => db.GetAsync(id), Times.Exactly(2));
			_mapper.Verify(mapper => mapper.Map<Feature, KeyValuePairResource>(_savedFeature), Times.Once);
			Assert.IsInstanceOf<OkObjectResult>(actual);
		}

		[Test]
		public async Task CanUpdateFeature_DoesNotExist() {
			const int id = 123;
			_featuresRepository.Setup(db => db.GetAsync(123)).Returns(Task.FromResult(default(Feature)));

			IActionResult actual = await _controller.UpdateFeatureAsync(id, _featureResource);

			_featuresRepository.Verify(db => db.GetAsync(id), Times.Once);
			var error = ControllerTestHelper.GetBadRequestError(actual);
			Assert.AreEqual("Feature with id = 123 does not exist!", error);
		}

		[Test]
		public async Task CanUpdateFeature_DoesNotThrowException() {
			const int id = 123;
			_featuresRepository.Setup(db => db.GetAsync(123)).Returns(Task.FromResult(_savedFeature));
			_unitOfWork.Setup(db => db.CompeleteAsync()).Throws(new Exception("SQL Statement error"));

			IActionResult actual = await _controller.UpdateFeatureAsync(id, _featureResource);

			_featuresRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Once);
			_mapper.Verify(mapper => mapper.Map(_featureResource, _savedFeature), Times.Once);
			_mapper.Verify(mapper => mapper.Map<Feature, KeyValuePairResource>(It.IsAny<Feature>()), Times.Never);
			Assert.AreEqual(500, (actual as ObjectResult)?.StatusCode);
			Assert.AreEqual("SQL Statement error", (actual as ObjectResult)?.Value?.ToString());
		}

		[Test]
		public async Task CanValidateFeature_UpdateFeature_RequiredFielsAreMissing() {
			FeatureResource FeatureResource = new FeatureResource {
				Name = null
			};

			_controller.ModelState.AddModelError("Name", "Name field is required.");

			IActionResult actual = await _controller.UpdateFeatureAsync(It.IsAny<int>(), FeatureResource);

			_featuresRepository.Verify(db => db.GetAsync(It.IsAny<int>()), Times.Never);
			var errors = ControllerTestHelper.GetBadRequestErrors(actual);
			Assert.AreEqual(new[] { "Name field is required." }, errors["Name"]);
		}
	}
}
