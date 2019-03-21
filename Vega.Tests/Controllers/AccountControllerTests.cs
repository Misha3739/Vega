using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vega.Controllers;
using vega.Controllers.Repositories;
using vega.Controllers.Resources;
using vega.Models;
using vega.Utility;

namespace Vega.Tests.Controllers {
	[TestFixture()]
	public class AccountControllerTests {
		private readonly Mock<IAccountRepository> _loginRepository;
		private readonly Mock<IToken> _tokenUtility;
		private readonly Mock<IEncryption> _encryption;
		private readonly AccountController _controller;

		private const string Email = "alice@gmail.com";

		public AccountControllerTests() {
			_loginRepository = new Mock<IAccountRepository>();
			_tokenUtility = new Mock<IToken>();
			_encryption = new Mock<IEncryption>();
			_controller = new AccountController(_loginRepository.Object, _tokenUtility.Object, _encryption.Object);
		}

		[TearDown]
		public void CleanUp() {
			_tokenUtility.Reset();
			_loginRepository.Reset();
			_encryption.Reset();
		}

		[Test]
		public async Task CanValidateCorrectCredentials() {
			const int userId = 1;
			string encryptedPassword = new string('*', 40);
			string token = new string('-', 40);
			var credentials = new UserLoginResource {
				Email = Email,
				Password = "newPassw0rd!"
			};

			_loginRepository.Setup(repo => repo.FindUser(credentials.Email))
				.ReturnsAsync(new User() {
					Id = userId,
					Email = Email,
					Role = Role.Administrator,
					EncryptedPassword = encryptedPassword
				});
			_tokenUtility.Setup(util => util.GenerateToken(Email, "Administrator", userId)).Returns(token);
			_encryption.Setup(enc => enc.Encrypt(credentials.Password)).Returns(encryptedPassword);

			var actual = await _controller.Login(credentials);

			_loginRepository.Verify(repo => repo.FindUser(credentials.Email), Times.Once);
			_encryption.Verify(enc => enc.Encrypt(credentials.Password), Times.Once);
			_tokenUtility.Verify(util => util.GenerateToken(credentials.Email, "Administrator", userId), Times.Once);
			Assert.IsInstanceOf<OkObjectResult>(actual);
			Assert.AreEqual(token, ControllerTestHelper.GetOkValue(actual));
		}

		[Test]
		public async Task CanValidateIfUserNotFound() {
			var credentials = new UserLoginResource {
				Email = Email,
				Password = "newPassw0rd!"
			};

			_loginRepository.Setup(repo => repo.FindUser(credentials.Email))
				.ReturnsAsync(default(User));

			var actual = await _controller.Login(credentials);
			_loginRepository.Verify(repo => repo.FindUser(credentials.Email), Times.Once);
			_tokenUtility.Verify(util => util.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
				Times.Never);
			_encryption.Verify(enc => enc.Encrypt(It.IsAny<string>()), Times.Never);
			Assert.IsInstanceOf<UnauthorizedResult>(actual);
		}

		[Test]
		public async Task CanValidateIfWrongPassword() {
			const int userId = 1;
			string encryptedPassword = new string('*', 40);
			var credentials = new UserLoginResource {
				Email = Email,
				Password = "wrongPassw0rd!"
			};

			_loginRepository.Setup(repo => repo.FindUser(credentials.Email))
				.ReturnsAsync(new User() {
					Id = userId,
					Email = Email,
					Role = Role.Administrator,
					EncryptedPassword = encryptedPassword
				});
			_encryption.Setup(enc => enc.Encrypt(credentials.Password)).Returns(new string('*', 10));

			var actual = await _controller.Login(credentials);

			_loginRepository.Verify(repo => repo.FindUser(credentials.Email), Times.Once);
			_encryption.Verify(enc => enc.Encrypt(credentials.Password), Times.Once);
			_tokenUtility.Verify(util => util.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
				Times.Never);
			Assert.IsInstanceOf<UnauthorizedResult>(actual);
		}
	}
}