using System.Net;
using CompanyApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyApi.Core.Errors
{
	public class NotFoundError : ApiError
	{
		public NotFoundError()
			: base(StatusCodes.Status404NotFound, HttpStatusCode.NotFound.ToString())
		{
		}
		
		public NotFoundError(string message)
			: base(StatusCodes.Status404NotFound, HttpStatusCode.NotFound.ToString(), message)
		{
		}
	}
}
