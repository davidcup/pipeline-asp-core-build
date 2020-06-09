using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyApi.Contracts.Entities;
using CompanyApi.Services.Repositories;
using CompanyApi.Tests.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CompanyApi.Tests.UnitTests
{
    public class DepartmentRepositoryTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentRepositoryTests(TestWebApplicationFactory factory)
        {
            factory.CreateClient();
            _departmentRepository = factory.Server.Host.Services.GetRequiredService<IDepartmentRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            var department = new Department { DepartmentId = 999, Name = "TEST DEPARTMENT"};
            var newDepartment = await _departmentRepository.AddAsync(department).ConfigureAwait(false);

            Assert.Equal("TEST DEPARTMENT", newDepartment.Name);
        }

        [Fact]
        public async Task CanAddRange()
        {
            var departments = new List<Department>
            {
                new Department {DepartmentId = 1111, Name = "TEST1"},
                new Department {DepartmentId = 2222, Name = "TEST2"},
                new Department {DepartmentId = 3333, Name = "TEST3"}
            };
            var nrDepartments = await _departmentRepository.AddAsync(departments).ConfigureAwait(false);
            Assert.True(nrDepartments > 0);
        }

        [Fact]
        public async Task CanCount()
        {
            var nrCompanies = await _departmentRepository.CountAsync().ConfigureAwait(false);

            Assert.True(nrCompanies > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            var department = new Department { DepartmentId = 9999, Name = "TEST DEPARTMENT"};
            var newDepartment = await _departmentRepository.AddAsync(department).ConfigureAwait(false);
            var result = await _departmentRepository.DeleteAsync(newDepartment).ConfigureAwait(false);

            Assert.True(result > 0);
        }

        [Fact]
        public async Task CanGetAllByPredicate()
        {
            var companies = await _departmentRepository.GetAllAsync(cmp => cmp.Name.Equals("Development")).ConfigureAwait(false);

            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanGetSingle()
        {
            var department = await _departmentRepository.GetSingleAsync(cmp => cmp.Name.Equals("Development")).ConfigureAwait(false);

            Assert.True(department != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var companies = await _departmentRepository.GetAllAsync().ConfigureAwait(false);

            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanUpdate()
        {
            var department = new Department { DepartmentId = 1, Name = "HR Updated" };
            var updatedDepartment = await _departmentRepository.UpdateAsync(department).ConfigureAwait(false);
            
            Assert.Equal("HR Updated", updatedDepartment.Name);
        }
    }
}
