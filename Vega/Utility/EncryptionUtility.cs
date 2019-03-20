using System;
using System.Security.Cryptography;

namespace vega.Utility {
	public static class EncryptionUtility {
		public static string Encrypt(string value) {
			RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
			byte[] encrypted = provider.Encrypt(System.Text.Encoding.UTF8.GetBytes(value), true);
			return Convert.ToBase64String(encrypted);
		}
	}
}