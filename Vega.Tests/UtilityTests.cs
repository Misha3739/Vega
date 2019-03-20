using NUnit.Framework;
using vega.Utility;

namespace Vega.Tests {
	[TestFixture]
	public class UtilityTests {
		[TestCase("Passw0rd")]
		[TestCase("newPassw0rd!")]
		public void CanEncrypt(string value) {
			Assert.IsNotNull(EncryptionUtility.Encrypt(value));
		}
	}
}