using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace vega.Utility {
	public class TokenUtility : IToken {
		private readonly IEncryption _encryption;

		public TokenUtility(IEncryption encryption) {
			_encryption = encryption;
		}

		public string GenerateToken(string userEmail,string role, int id) {
			var claims = new Claim[] {
				new Claim(ClaimTypes.Name, userEmail),
				new Claim(ClaimTypes.Role, role),
				new Claim(ClaimTypes.NameIdentifier, id.ToString()),
				new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
				new Claim(JwtRegisteredClaimNames.Exp,
					new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
			};

			var token = new JwtSecurityToken(
				new JwtHeader(new SigningCredentials(
					new SymmetricSecurityKey(_encryption.GetPrivateTokenKey()),
					SecurityAlgorithms.HmacSha256)),
				new JwtPayload(claims));

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}