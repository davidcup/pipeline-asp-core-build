using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompanyApi.Services.Filters
{
	/// <summary>
	/// Filter for returning a result if the given model to a controller does not pass validation
	/// </summary>
	public class ValidatorActionFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.ModelState.IsValid)
			{
				filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
			}
		}
 
		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
		}
	}
}
