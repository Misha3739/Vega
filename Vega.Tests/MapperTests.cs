using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NUnit.Framework;
using vega.Controllers.Resources;
using vega.Mapping;
using vega.Models;

namespace Vega.Tests {
	[TestFixture]
	public class MapperTests {
		private readonly IMapper _mapper;

		public MapperTests() {
			MapperConfiguration config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
			_mapper = new Mapper(config);
		}

		[Test]
		public void CanMapVehicleToVehicleResource() {
			Vehicle vehicle = new Vehicle {
				Id = 123,
				ModelId = 2,
				Model = new Model {
					Id = 2,
					Name = "Audi Q5",
					Make = new Make {
						Id = 1,
						Name = "Audi"
					}
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
				},
				LastUpdate = new DateTime(1991, 1, 2)
			};

			VehicleResource actual = _mapper.Map<Vehicle, VehicleResource>(vehicle);

			Assert.IsNotNull(actual);
			Assert.IsNotNull(actual.Model);
			Assert.IsNotNull(actual.Make);
			Assert.IsNotNull(actual.Contact);
			Assert.IsNotNull(actual.Features);

			Assert.AreEqual(123, actual.Id);
			Assert.AreEqual(2, actual.Model.Id);
			Assert.AreEqual("Audi Q5", actual.Model.Name);
			Assert.AreEqual(1, actual.Make.Id);
			Assert.AreEqual("Audi", actual.Make.Name);
			Assert.AreEqual("Name@mail.com", actual.Contact.Email);
			Assert.AreEqual("Name", actual.Contact.Name);
			Assert.AreEqual("3333333", actual.Contact.Phone);
			Assert.AreEqual(1, actual.Features.Count);
			Assert.AreEqual(5, actual.Features.First().Id);
			Assert.AreEqual("Feature 5", actual.Features.First().Name);
			Assert.AreEqual(new DateTime(1991, 1, 2), actual.LastUpdate);
		}

		[Test]
		public void CanMapSaveVehicleResourceToVehicle() {
			SaveVehicleResource saveVehicleResource = new SaveVehicleResource {
				Id = 123,
				ModelId = 2,
				Contact = new ContacResource {
					Email = "Name@mail.com",
					Name = "Name",
					Phone = "3333333"
				},
				Features = new List<int> { 3, 5 },
				LastUpdate = new DateTime(1991, 1, 2)
			};

			Vehicle actual = _mapper.Map<Vehicle>(saveVehicleResource);

			Assert.IsNotNull(actual);
			Assert.AreEqual(0, actual.Id);
			Assert.AreEqual(2, actual.ModelId);
			Assert.AreEqual("Name@mail.com", actual.ContactEmail);
			Assert.AreEqual("Name", actual.ContactName);
			Assert.AreEqual("3333333", actual.ContactPhone);
			Assert.IsNotNull(actual.Features);
			Assert.AreEqual(new List<int> { 3, 5 }, actual.Features.Select(f => f.FeatureId));
			Assert.AreEqual(actual.LastUpdate, new DateTime(1991, 1, 2));
		}

		[Test]
		public void CanMapSaveVehicleResourceToExistingVehicle() {
			SaveVehicleResource saveVehicleResource = new SaveVehicleResource {
				Id = 125,
				ModelId = 2,
				Contact = new ContacResource {
					Email = "Name1@mail.com",
					Name = "Name1",
					Phone = "4444444"
				},
				Features = new List<int> { 5, 7, 9 },
				LastUpdate = new DateTime(1991, 1, 2)
			};

			Vehicle vehicle = new Vehicle {
				Id = 123,
				ModelId = 3,
				ContactEmail = "Name@mail.com",
				ContactName = "Name",
				ContactPhone = "3333333",
				Features = new List<VehicleFeature> {
					new VehicleFeature {
						FeatureId = 5,
					},
					new VehicleFeature {
						FeatureId = 6,
					}
				},
				LastUpdate = new DateTime(1991, 1, 2)
			};

			Vehicle actual = _mapper.Map(saveVehicleResource, vehicle);

			Assert.IsNotNull(actual);
			Assert.AreEqual(123, actual.Id); //We use Id of existing saveVehicle
			Assert.AreEqual(2, actual.ModelId); //But all following properties being assigned from VehiceResource
			Assert.AreEqual("Name1@mail.com", actual.ContactEmail);
			Assert.AreEqual("Name1", actual.ContactName);
			Assert.AreEqual("4444444", actual.ContactPhone);
			Assert.IsNotNull(actual.Features);
			//Should be 5,7,9 in features
			Assert.AreEqual(new List<int> { 5, 7, 9 }, actual.Features.Select(f => f.FeatureId));
			Assert.AreEqual(actual.LastUpdate, new DateTime(1991, 1, 2));
		}

		[Test]
		public void CanMapFeatureToKeyValuePairResource() {
			var feature = new Feature() {
				Id = 1,
				Name = "Wheels 17",
			};

			var actual = _mapper.Map<Feature, KeyValuePairResource>(feature);

			Assert.IsNotNull(actual);
			Assert.AreEqual(1, actual.Id);
			Assert.AreEqual("Wheels 17", actual.Name);
		}

		[Test]
		public void CanMapMakeToMakeResource() {
			var make = new Make() {
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
			};

			var actual = _mapper.Map<Make, MakeResource>(make);

			Assert.IsNotNull(actual);
			Assert.AreEqual(1, actual.Id);
			Assert.AreEqual("Audi", actual.Name);

			Assert.IsNotNull(actual.Models);
			Assert.AreEqual(2, actual.Models.Count);
			CollectionAssert.AreEqual(new[] { 1, 2 }, actual.Models.Select(m => m.Id));
			CollectionAssert.AreEqual(new[] { "Q5", "Q7" }, actual.Models.Select(m => m.Name));
		}
	}
}
