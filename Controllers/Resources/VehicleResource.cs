using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using vega.Models;

namespace vega.Controllers.Resources {
    public class VehicleResource {
        [Key]
        public int Id { get; set; }
        public int ModelId { get; set; }
        public bool IsRegistered { get; set; }
        public ContacResource Contact { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<int> Features { get; set; }
        public VehicleResource() {
            Features = new Collection<int>();
        }
    }
}