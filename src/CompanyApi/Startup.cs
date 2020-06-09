using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Contracts.Converters;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using CompanyApi.Core.Auth;
using CompanyApi.Extensions;
using CompanyApi.Persistence.DbContexts;
using CompanyApi.Services;
using CompanyApi.Services.Authorization;
using CompanyApi.Services.Filters;
using CompanyApi.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace CompanyApi
{
    public class Startup
    {
        //private readonly IHostingEnvironment _env;
        private const string API_NAME = "Company Web API";

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure JWT authentication
            ConfigureAuth(services);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqLiteConnectionString")));

            // Add Sqlite Health check
            // https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
            services.AddHealthChecks().AddSqlite(Configuration.GetConnectionString("SqLiteHealthConnectionString"), // Connection string
                "select name from sqlite_master where type='table'", // Health Query 
                tags: new[] { "sqlite" }); // Tags

            // Add Serilog logging to file
            var logFile = Path.Combine(Environment.ContentRootPath, "_log.txt");
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"))
                    .AddSerilog(new LoggerConfiguration().WriteTo.File(logFile).CreateLogger())
                    .AddConsole();
#if DEBUG
                builder.AddDebug();
#endif
            });

            // DI - Register services
            RegisterServices(services);

            // Add the whole configuration object here
            services.AddSingleton(Configuration);

            services.AddMvc(options =>
                {
                    // Filter for returning a result if the given model to a controller does not pass validation
                    options.Filters.Add(typeof(ValidatorActionFilter));
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                });

            // Add API Versioning
            // The default version is 1.1
            // And we're going to read the version number from the media type
            // Incoming requests should have a accept header like this: Accept: application/json;v=1.1
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 1); // Specify the default api version
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new MediaTypeApiVersionReader(); // read the version number from the accept header
            });

            // Configure Swagger support
            services.ConfigureSwagger(API_NAME);

            // Configure CORS
            services.AddCorsPolicy("EnableCORS");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enables full buffering of response bodies
            app.UseResponseBuffering();

            // Configure Database context
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                SeedData.Initialize(context);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.1/swagger.json", $"{API_NAME} v1.1");
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", $"{API_NAME} v1.0");
                });
            }
            else
            {
                // Handles exceptions and generates a custom response body in Errors Controller
                app.UseExceptionHandler("/errors/unhandled");

                // Handles non-success status codes with empty body in Errors Controller
                app.UseStatusCodePagesWithReExecute("/errors/{0}");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Use Health check
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = r => r.Tags.Contains("sqlite")
            });

            app.UseStaticFiles();
            app.UseCors("EnableCORS");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }

        protected virtual void ConfigureAuth(IServiceCollection services)
        {
            // Configure AuthSettings            
            var authSettings = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(authSettings);

            var key = Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]);
            var signingKey = new SymmetricSecurityKey(key);
            var jwtIssuerOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtIssuerOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtIssuerOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.ClaimsIssuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)];
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
                opt.TokenValidationParameters = tokenValidationParameters;
            });
        }
        
        protected virtual void RegisterServices(IServiceCollection services)
        {
            // Services
            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<IJwtFactory, JwtFactory>();
            services.AddScoped<IUserService, UserService>();

            //*********************************************************************************
            // Registering multiple implementations of the same interface IRepository<TEntity>
            //*********************************************************************************
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Entity to Dto Converters
            services.AddTransient<IConverter<Company, CompanyDto>, CompanyToDtoCoverter>();
            services.AddTransient<IConverter<IList<Company>, IList<CompanyDto>>, CompanyToDtoCoverter>();
            services.AddTransient<IConverter<Department, DepartmentDto>, DepartmentToDtoCoverter>();
            services.AddTransient<IConverter<IList<Department>, IList<DepartmentDto>>, DepartmentToDtoCoverter>();
            services.AddTransient<IConverter<Employee, EmployeeDto>, EmployeeToDtoCoverter>();
            services.AddTransient<IConverter<IList<Employee>, IList<EmployeeDto>>, EmployeeToDtoCoverter>();
        }
    }
}
