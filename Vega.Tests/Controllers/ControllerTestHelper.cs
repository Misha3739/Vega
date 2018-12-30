using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Vega.Tests.Controllers {
	internal static class ControllerTestHelper {
		public static Dictionary<string, object> GetBadRequestErrors(IActionResult actual) {
			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			SerializableError errorList = result?.Value as SerializableError;
			return errorList;
		}

		public static string GetBadRequestError(IActionResult actual) {
			Assert.IsInstanceOf<BadRequestObjectResult>(actual);
			BadRequestObjectResult result = actual as BadRequestObjectResult;
			return result.Value?.ToString();
		}

		public static string GetNotFoundError(IActionResult actual) {
			Assert.IsInstanceOf<NotFoundObjectResult>(actual);
			NotFoundObjectResult result = actual as NotFoundObjectResult;
			return result.Value?.ToString();
		}
	}
}
