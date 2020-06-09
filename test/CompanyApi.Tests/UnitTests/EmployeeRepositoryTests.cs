using System;
using System.Threading.Tasks;
using CompanyApi.Contracts.Entities;
using CompanyApi.Services.Repositories;
using CompanyApi.Tests.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CompanyApi.Tests.UnitTests
{
    public class EmployeeRepositoryTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeRepositoryTests(TestWebApplicationFactory factory)
        {
            factory.CreateClient();
            _employeeRepository = factory.Server.Host.Services.GetRequiredService<IEmployeeRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            var employee = new Employee
            {
                EmployeeId = 999,
                FirstName = "Test",
                LastName = "Tester",
                BirthDate = new DateTime(2001, 12, 16),
                CompanyId = 1,
                DepartmentId = 1,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var newEmployee = await _employeeRepository.AddAsync(employee).ConfigureAwait(false);

            Assert.Equal("Tester", newEmployee.LastName);
        }

        [Fact]
        public async Task CanCount()
        {
            var nrCompanies = await _employeeRepository.CountAsync().ConfigureAwait(false);

            Assert.True(nrCompanies > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            var employee = new Employee
            {
                EmployeeId = 9999,
                FirstName = "Test",
                LastName = "Tester",
                BirthDate = new DateTime(2001, 12, 16),
                CompanyId = 1,
                DepartmentId = 1,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };            
            var newEmployee = await _employeeRepository.AddAsync(employee).ConfigureAwait(false);
            var result = await _employeeRepository.DeleteAsync(newEmployee).ConfigureAwait(false);

            Assert.True(result > 0);
        }

        [Fact]
        public async Task CanGetAllByPredicate()
        {
            var companies = await _employeeRepository.GetAllAsync(cmp => cmp.FirstName.Equals("Julia")).ConfigureAwait(false);

            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanGetSingle()
        {
            var employee = await _employeeRepository.GetSingleAsync(cmp => cmp.FirstName.Equals("Alois") && cmp.LastName.Equals("Mock")).ConfigureAwait(false);

            Assert.True(employee != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var companies = await _employeeRepository.GetAllAsync().ConfigureAwait(false);

            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanUpdate()
        {
            var employee = new Employee
            {
                EmployeeId = 3,
                FirstName = "Julia",
                LastName = "Reynolds Updated",
                BirthDate = new DateTime(1955, 12, 16),
                CompanyId = 1,
                DepartmentId = 3,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var updatedEmployee = await _employeeRepository.UpdateAsync(employee).ConfigureAwait(false);
            
            Assert.Equal("Reynolds Updated", updatedEmployee.LastName);
        }
    }
}
