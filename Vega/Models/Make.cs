using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace vega.Models {
    public class Make {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
		public ICollection<Model> Models { get; set; }
        public Make() {
            Models = new Collection<Model>();
        }
    }
}