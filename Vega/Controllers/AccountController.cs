using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Utility;

namespace vega.Controllers {
	public class AccountController : ControllerBase {
		private readonly IAccountRepository _accountRepository;

		private readonly IToken _token;

		private readonly IEncryption _encryption;

		public AccountController(IAccountRepository accountRepository, IToken token, IEncryption encryption) {
			_accountRepository = accountRepository;
			_token = token;
			_encryption = encryption;
		}

		[HttpPost("api/account/login")]
		public async Task<IActionResult> Login([FromBody] UserLoginResource loginResource) {
			var user = await _accountRepository.FindUser(loginResource.Email);
			if (user != null && user.EncryptedPassword == _encryption.Encrypt(loginResource.Password)) {
				string token = _token.GenerateToken(user.Email, user.RoleName, user.Id);
				return Ok(token);
			}

			return Unauthorized();
		}
	}
}