using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompanyApi.Services.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace CompanyApi.Extensions
{
	public static class ServiceExtensions
	{
		public static void AddCorsPolicy(this IServiceCollection serviceCollection, string corsPolicyName)
		{
			serviceCollection.AddCors(options =>
			{
				options.AddPolicy(corsPolicyName,
					builder => builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials());
			});
		}

		public static void ConfigureSwagger(this IServiceCollection serviceCollection, string apiName, bool includeXmlDocumentation = true)
		{
			serviceCollection.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1.0", new Info
				{
					Title = $"{apiName} v1.0",
					Version = "v1.0",
					Description = "Deprecated Web API",
					TermsOfService = string.Empty,
					Contact = new Contact
					{
						Name = "Matjaz Bravc",
						Email = "matjaz.bravc@gmail.com",
						Url = "https://matjazbravc.github.io/"
					},
					License = new License
					{
						Name = "Use under LICENSE...",
						Url = "https://example.com/license"
					}
				});
				options.SwaggerDoc("v1.1", new Info
				{
					Title = $"{apiName} v1.1",
					Version = "v1.1",
					Description = "Default Web API",
					TermsOfService = string.Empty,
					Contact = new Contact
					{
						Name = "Matjaz Bravc",
						Email = "matjaz.bravc@gmail.com",
						Url = "https://matjazbravc.github.io/"
					},
					License = new License
					{
						Name = "Use under LICENSE...",
						Url = "https://example.com/license"
					}
				});
				options.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
					Name = "Authorization",
					In = "header",
					Type = "apiKey"
				});
				options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
				{
					{
						"Bearer", new string[] { }
					}
				});
				options.DocInclusionPredicate((docName, apiDesc) =>
				{
					var actionApiVersionModel = apiDesc.ActionDescriptor.GetApiVersionModel();
					// Would mean this action is unversioned and should be included everywhere
					if (actionApiVersionModel == null)
					{
						return true;
					}
					return actionApiVersionModel.DeclaredApiVersions.Any() ? actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName) : actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
				});
				options.OperationFilter<ApiVersionOperationFilter>();
				if (includeXmlDocumentation)
				{
					var xmlDocFile = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "CompanyWebApiDemo.xml");
					if (File.Exists(xmlDocFile))
					{
						options.IncludeXmlComments(xmlDocFile);
					}
				}
				options.DescribeAllEnumsAsStrings();
				options.DescribeAllParametersInCamelCase();
				options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});
		}
	}
}
