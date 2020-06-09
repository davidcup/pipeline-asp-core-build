using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using CompanyApi.Tests.Services;
using Xunit;

namespace CompanyApi.Tests.IntegrationTests
{
	public class EmployeesControllerTests : IClassFixture<TestWebApplicationFactory>
	{
		private readonly HttpClientHelper _httpClientHelper;

		public EmployeesControllerTests(TestWebApplicationFactory factory)
		{
			_httpClientHelper = new HttpClientHelper(factory.CreateClient());
		}

		[Fact]
		public async Task CanCreateAndDeleteEmployee()
		{
			var newCompany = new Company
			{
				CompanyId = 999,
				Name = "Company TEST",
				Created = DateTime.UtcNow,
				Modified = DateTime.UtcNow
			};

			var newDepartment = new Department { DepartmentId = 999, Name = "Department TEST"};

			/* Create Employee */
			var newEmployee = new Employee
			{
				EmployeeId = 999,
				FirstName = "Sylvester",
				LastName = "Holt",
				BirthDate = new DateTime(1995, 8, 7),
				Company = newCompany,
				Department = newDepartment,
				Created = DateTime.UtcNow,
				Modified = DateTime.UtcNow
			};

			var employee = await _httpClientHelper.PostAsync("/api/employees/create", newEmployee);
			Assert.Equal("Sylvester", employee.FirstName);
			Assert.Equal("Holt", employee.LastName);

			/* Delete new Employee */
			await _httpClientHelper.DeleteAsync("/api/employees/999");
		}

		[Fact]
		public async Task CanGetAllEmployees()
		{
			var employees = await _httpClientHelper.GetAsync<List<EmployeeDto>>("/api/employees/getall");
			Assert.Contains(employees, p => p.FirstName == "Julia");
		}

		[Fact]
		public async Task CanGetEmployee()
		{
			// The endpoint or route of the controller action.
			var employee = await _httpClientHelper.GetAsync<EmployeeDto>("/api/employees/3");
			Assert.Equal(3, employee.EmployeeId);
			Assert.Equal("Julia", employee.FirstName);
		}

		[Fact]
		public async Task CanUpdateEmployee()
		{
			// Get first employee
            /* Create Employee */
            var newEmployee = new Employee
            {
                EmployeeId = 1,
                FirstName = "Johnny",
                LastName = "Holt",
                BirthDate = new DateTime(1995, 8, 7),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

			// Update employee
			var updatedEmployee = await _httpClientHelper.PostAsync("/api/employees/update", newEmployee).ConfigureAwait(false);

			// First name should be a new one
			Assert.Equal("Johnny", updatedEmployee.FirstName);
		}
	}
}
