using System.Net;
using CompanyApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyApi.Core.Errors
{
	public class BadRequestError : ApiError
	{
		public BadRequestError()
			: base(StatusCodes.Status400BadRequest, HttpStatusCode.BadRequest.ToString())
		{
		}

		public BadRequestError(string message)
			: base(StatusCodes.Status400BadRequest, HttpStatusCode.BadRequest.ToString(), message)
		{
		}
	}
}
