namespace vega.Utility {
	public interface IToken {
		string GenerateToken(string userEmail,string role, int id);
	}
}