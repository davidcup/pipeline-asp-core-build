using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CompanyApi.Services.Filters
{
	public class ApiVersionOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			var actionApiVersionModel = context.ApiDescription.ActionDescriptor.GetApiVersionModel();
			if (actionApiVersionModel == null)
			{
				return;
			}

			if (actionApiVersionModel.DeclaredApiVersions.Any())
			{
				operation.Produces = operation.Produces
					.SelectMany(p => actionApiVersionModel.DeclaredApiVersions
						.Select(version => $"{p};v={version.ToString()}")).ToList();
			}
			else
			{
				operation.Produces = operation.Produces
					.SelectMany(p => actionApiVersionModel.ImplementedApiVersions.OrderByDescending(v => v)
						.Select(version => $"{p};v={version.ToString()}")).ToList();
			}
		}
	}
}
