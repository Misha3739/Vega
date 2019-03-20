using System.ComponentModel.DataAnnotations;

namespace vega.Models {
    public class Feature {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

		public string Description { get; set; }
    }
}