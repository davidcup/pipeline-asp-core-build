using System;
using Microsoft.AspNetCore.Authentication;

namespace CompanyApi.Tests.Services
{
	public static class TestAuthenticationExtensions
	{
		public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
		{
			return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>("Test Scheme", "Test Auth", configureOptions);
		}
	}
}
