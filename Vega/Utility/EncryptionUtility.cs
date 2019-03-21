using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace vega.Utility {
	public sealed class EncryptionUtility : IEncryption {
		public string Encrypt(string value) {
			if (string.IsNullOrEmpty(value)) return string.Empty;
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			byte[] shifted = new byte[bytes.Length];
			for (int i = 0; i < bytes.Length; i++) {
				shifted[i] = (byte)(bytes[i] << 4);
			}
			return Convert.ToBase64String(shifted);
		}

		public byte[] GetPrivateTokenKey() {
			var assembly = typeof(EncryptionUtility).GetTypeInfo().Assembly;
			var embeddedFileProvider = new EmbeddedFileProvider(assembly, "vega");
			var keyFileInfo = embeddedFileProvider.GetFileInfo(Path.Combine("Certificates", "bearer"));
			using (var certificateStream = keyFileInfo.CreateReadStream()) {
				using (var memoryStream = new MemoryStream()) {
					certificateStream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
		}
	}
}