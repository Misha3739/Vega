using NUnit.Framework;
using vega.Utility;

namespace Vega.Tests {
	[TestFixture]
	public class UtilityTests {
		private readonly IEncryption _encryption;

		public UtilityTests() {
			_encryption = new EncryptionUtility();
		}

		[TestCase(null)]
		[TestCase("")]
		public void CanEncryptEmptyString(string value) {
			Assert.AreEqual(string.Empty, _encryption.Encrypt(value));
		}

		[TestCase("Passw0rd")]
		[TestCase("newPassw0rd!")]
		public void CanEncrypt(string value) {
			Assert.IsNotNull(_encryption.Encrypt(value));
		}

		[TestCase("Passw0rd")]
		[TestCase("newPassw0rd!")]
		public void CanEncryptSameValue(string value) {
			var actual1 = _encryption.Encrypt(value);
			var actual2 = _encryption.Encrypt(value);
			Assert.AreEqual(actual1, actual2);
		}
	}
}