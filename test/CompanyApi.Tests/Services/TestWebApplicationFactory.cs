using System;
using CompanyApi.Persistence.DbContexts;
using CompanyApi.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompanyApi.Tests.Services
{
	/// <summary>
	/// Factory for bootstrapping an application in-memory for functional end to end tests.
	/// This factory can be used to create a TestServer instance.
	/// </summary>
    public class TestWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
	    protected override IWebHostBuilder CreateWebHostBuilder()
	    {
		    return WebHost.CreateDefaultBuilder().UseStartup<TestStartup>();
	    }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
	        builder.UseContentRoot(".");
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDatabase");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var serviceScope = sp.CreateScope())
                {
	                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var scopedServices = serviceScope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<ApplicationDbContext>();
					var logger = scopedServices.GetRequiredService<ILogger<TestStartup>>();

                    // Ensure the database is created
                    appDb.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with some test data
	                    SeedData.Initialize(context);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
