using System.Net;
using CompanyApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyApi.Core.Errors
{
	public class UnauthorizedError : ApiError
	{
		public UnauthorizedError()
			: base(StatusCodes.Status401Unauthorized, HttpStatusCode.Unauthorized.ToString())
		{
		}

		public UnauthorizedError(string message)
			: base(StatusCodes.Status401Unauthorized, HttpStatusCode.Unauthorized.ToString(), message)
		{
		}
	}
}
