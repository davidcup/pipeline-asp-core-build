using System.Net;
using CompanyApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyApi.Core.Errors
{
	public class InternalServerError : ApiError
	{
		public InternalServerError()
			: base(StatusCodes.Status500InternalServerError, HttpStatusCode.InternalServerError.ToString())
		{
		}
		
		public InternalServerError(string message)
			: base(StatusCodes.Status500InternalServerError, HttpStatusCode.InternalServerError.ToString(), message)
		{
		}
	}
}
