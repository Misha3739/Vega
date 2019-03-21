namespace vega.Utility {
	public interface IEncryption {
		string Encrypt(string value);

		byte[] GetPrivateTokenKey();
	}
}