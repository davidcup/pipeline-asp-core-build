using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyApi.Contracts.Entities;
using CompanyApi.Tests.Services;
using Xunit;

namespace CompanyApi.Tests.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClientHelper _httpClientHelper;

        public UsersControllerTests(TestWebApplicationFactory factory)
        {
            _httpClientHelper = new HttpClientHelper(factory.CreateClient());
        }

        [Fact]
        public async Task CanCreateAndDeleteUsers()
        {
            var newCompany = new Company
            {
                CompanyId = 9999,
                Name = "Company TEST",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var newDepartment = new Department { DepartmentId = 9999, Name = "Department TEST"};
            var newEmployee = new Employee
            {
                EmployeeId = 9999,
                FirstName = "Sylvester",
                LastName = "Holt",
                BirthDate = new DateTime(1995, 8, 7),
                Company = newCompany,
                Department = newDepartment,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            var newUser = new User
            {
                EmployeeId = newEmployee.EmployeeId, Employee = newEmployee, Username = "testuser", Password = "test", Token = string.Empty
            };
            var user = await _httpClientHelper.PostAsync("/api/users/create", newUser);
            Assert.Equal("testuser", user.Username);

            /* Delete new User */
            await _httpClientHelper.DeleteAsync("/users/testuser");
        }
        
        [Fact]
        public async Task CanGetAllUsers()
        {
            var users = await _httpClientHelper.GetAsync<List<User>>("/api/users/getall");
            Assert.Contains(users, p => p.Username == "johnw");
        }

        [Fact]
        public async Task CanGetUser()
        {
            var user = await _httpClientHelper.GetAsync<User>("/api/users/johnw");
            Assert.Equal("johnw", user.Username);
        }

        [Fact]
        public async Task CanUpdateUser()
        {
            // Get user and change Password
            var newUser = new User
            {
                EmployeeId = 2,
                Username = "mathiasg",
                Password = "abcde12345",
                Token = string.Empty
            };

            var updatedUser = await _httpClientHelper.PostAsync("/api/users/update", newUser).ConfigureAwait(false);
            Assert.Equal("abcde12345", updatedUser.Password);
        }

        [Fact]
        public async Task CanUserAuthenticate()
        {
            var user = new User { EmployeeId = 1, Username = "johnw", Password = "test", Token = string.Empty };
            var authenticatedUser = await _httpClientHelper.PostAsync("/api/users/authenticate", user);
            Assert.NotNull(authenticatedUser);
        }
    }
}
