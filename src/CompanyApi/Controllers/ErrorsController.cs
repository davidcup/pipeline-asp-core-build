using System.Net;
using CompanyApi.Core.Errors.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
	[ApiController]
	[AllowAnonymous]
	public class ErrorsController : Controller
	{
		[Route("[controller]/unhandled")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public IActionResult UnhandledError()
		{
			ApiError error = null;
			var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			if (exceptionFeature != null)
			{
				error = new ApiError(StatusCodes.Status500InternalServerError, $"Unhandled error: {exceptionFeature.Error.Message}, path: {exceptionFeature.Path}");
			}
			return new ObjectResult(error);
		}

		[Route("[controller]/{code}")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public IActionResult Error(int code)
		{
			var parsedCode = (HttpStatusCode)code;
			var error = new ApiError(code, parsedCode.ToString());
			return new ObjectResult(error);
		}
	}
}