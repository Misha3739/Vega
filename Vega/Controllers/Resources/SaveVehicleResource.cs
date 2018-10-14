using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace vega.Controllers.Resources {
	public class SaveVehicleResource {
		[Key]
		public int Id { get; set; }

		public int ModelId { get; set; }

		public bool IsRegistered { get; set; }

		[Required]
		public ContacResource Contact { get; set; }

		public DateTime LastUpdate { get; set; }

		public ICollection<int> Features { get; set; }

		public SaveVehicleResource() {
			Features = new Collection<int>();
		}
	}
}