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
				ContactEmail = "Name@mail.com",
				ContactName = "Name",
				ContactPhone = "3333333",
				Features = new List<VehicleFeature> {
					new VehicleFeature {
						FeatureId = 5,
						VehicleId = 23
					},
					new VehicleFeature {
						FeatureId = 6,
						VehicleId = 22
					}
				},
				LastUpdate = new DateTime(1991, 1, 2)
			};

			VehicleResource actual = _mapper.Map<Vehicle, VehicleResource>(vehicle);

			Assert.IsNotNull(actual);
			Assert.AreEqual(123, actual.Id);
			Assert.AreEqual(2, actual.ModelId);
			Assert.IsNotNull(actual.Contact);
			Assert.AreEqual("Name@mail.com", actual.Contact.Email);
			Assert.AreEqual("Name", actual.Contact.Name);
			Assert.AreEqual("3333333", actual.Contact.Phone);
			Assert.IsNotNull(actual.Features);
			CollectionAssert.AreEqual(new List<int> { 5, 6 }, actual.Features);
			Assert.AreEqual(actual.LastUpdate, new DateTime(1991, 1, 2));
		}

		[Test]
		public void CanMapVehicleResourceToVehicle() {
			VehicleResource vehicleResource = new VehicleResource {
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

			Vehicle actual = _mapper.Map<Vehicle>(vehicleResource);

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
		public void CanMapVehicleResourceToExistingVehicle() {
			VehicleResource vehicleResource = new VehicleResource {
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

			Vehicle actual = _mapper.Map(vehicleResource, vehicle);

			Assert.IsNotNull(actual);
			Assert.AreEqual(123, actual.Id);  //We use Id of existing vehicle
			Assert.AreEqual(2, actual.ModelId); //But all following properties being assigned from VehiceResource
			Assert.AreEqual("Name1@mail.com", actual.ContactEmail);
			Assert.AreEqual("Name1", actual.ContactName);
			Assert.AreEqual("4444444", actual.ContactPhone);
			Assert.IsNotNull(actual.Features);
			//Should be 5,7,9 in features
			Assert.AreEqual(new List<int> {5,7,9}, actual.Features.Select(f => f.FeatureId));
			Assert.AreEqual(actual.LastUpdate, new DateTime(1991, 1, 2));
		}
	}
}
