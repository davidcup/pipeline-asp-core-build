using System.Net;
using CompanyApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyApi.Core.Errors
{
	public class ForbiddenError : ApiError
	{
		public ForbiddenError()
			: base(StatusCodes.Status403Forbidden, HttpStatusCode.Forbidden.ToString())
		{
		}

		public ForbiddenError(string message)
			: base(StatusCodes.Status403Forbidden, HttpStatusCode.Forbidden.ToString(), message)
		{
		}
	}
}
