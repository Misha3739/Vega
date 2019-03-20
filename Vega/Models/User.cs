using System.ComponentModel.DataAnnotations;

namespace vega.Models {
	public class User {
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(255)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(255)]
		public string LastName { get; set; }

		[Required]
		[StringLength(255)]
		public string Email { get; set; }

		[RegularExpression(@"^\+[1-9]{1}[0-9]{3,14}$")]
		public string MobilePhone { get; set; }

		[Required]
		public string EncryptedPassword { get; set; }

		public Role Role { get; set; }
	}
}