using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyApi.Tests.Services
{
	/// <summary>
	/// Mimicking Startup class.
	/// </summary>
	public class TestStartup : Startup
	{
		public TestStartup(IConfiguration configuration, IHostingEnvironment environment) 
			: base(configuration, environment)
		{
		}

		// Bypassing Authentication in base Startup class to be able to test Controller's methods with [Authorize] attribute
		protected override void ConfigureAuth(IServiceCollection services)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = "Test Scheme"; // has to match scheme in TestAuthenticationExtensions
				options.DefaultChallengeScheme = "Test Scheme";
			}).AddTestAuth(o => { });
		}

        protected override void RegisterServices(IServiceCollection services)
        {
            base.RegisterServices(services);
            // Add additional service registrations needed in tests
            //services.AddTransient<IDummy, Dummy>();
        }
	}
}
