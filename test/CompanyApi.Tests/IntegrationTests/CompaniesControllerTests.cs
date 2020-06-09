using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using CompanyApi.Tests.Services;
using Xunit;

namespace CompanyApi.Tests.IntegrationTests
{
	public class CompaniesControllerTests : IClassFixture<TestWebApplicationFactory>
	{
		private readonly HttpClientHelper _httpClientHelper;

		public CompaniesControllerTests(TestWebApplicationFactory factory)
		{
			_httpClientHelper = new HttpClientHelper(factory.CreateClient());
		}

		[Fact]
		public async Task CanCreateAndDeleteCompanies()
		{
			var newCompany = new Company { CompanyId = 999, Name = "Test Company",  Created = DateTime.UtcNow, Modified = DateTime.UtcNow };

			var company = await _httpClientHelper.PostAsync("/api/companies/create", newCompany);
			Assert.Equal("Test Company", company.Name);

			/* Delete new Employee */
			await _httpClientHelper.DeleteAsync("/api/companies/999");
		}

		[Fact]
		public async Task CanGetAllCompanies()
		{
			var companies = await _httpClientHelper.GetAsync<List<CompanyDto>>("/api/companies/getall");
			Assert.Contains(companies, p => p.Name == "Company Two");
		}

		[Fact]
		public async Task CanGetCompany()
		{
			// The endpoint or route of the controller action.
			var company = await _httpClientHelper.GetAsync<CompanyDto>("/api/companies/3");
			Assert.Equal(3, company.CompanyId);
			Assert.Equal("Company Three", company.Name);
		}

		[Fact]
		public async Task CanUpdateCompany()
		{
			// Update first company with CompanyId = 1
            var companyToUpdate = new Company { CompanyId = 1, Name = "Test Company",  Created = DateTime.UtcNow, Modified = DateTime.UtcNow };

			// Update company
			var updatedCompany = await _httpClientHelper.PostAsync("/api/companies/update", companyToUpdate).ConfigureAwait(false);

			// Name should be equal
			Assert.Equal("Test Company", updatedCompany.Name);
		}
	}
}
