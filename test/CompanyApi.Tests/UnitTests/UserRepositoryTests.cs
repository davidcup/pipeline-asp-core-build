using System.Threading.Tasks;
using CompanyApi.Contracts.Entities;
using CompanyApi.Services.Repositories;
using CompanyApi.Tests.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CompanyApi.Tests.UnitTests
{
    public class UserRepositoryTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests(TestWebApplicationFactory factory)
        {
            factory.CreateClient();
            _userRepository = factory.Server.Host.Services.GetRequiredService<IUserRepository>();
        }

        [Fact]
        public async Task CanAdd()
        {
            var user = new User { EmployeeId = 999, Username = "tester", Password = "test", Token = string.Empty };
            var newUser = await _userRepository.AddAsync(user).ConfigureAwait(false);

            Assert.Equal("tester", newUser.Username);
        }

        [Fact]
        public async Task CanCount()
        {
            var nrCompanies = await _userRepository.CountAsync().ConfigureAwait(false);

            Assert.True(nrCompanies > 0);
        }

        [Fact]
        public async Task CanDelete()
        {
            var user = new User { EmployeeId = 9999, Username = "tester", Password = "test", Token = string.Empty };
            var newUser = await _userRepository.AddAsync(user).ConfigureAwait(false);
            var result = await _userRepository.DeleteAsync(newUser).ConfigureAwait(false);

            Assert.True(result > 0);
        }

        [Fact]
        public async Task CanGetAllByPredicate()
        {
            var companies = await _userRepository.GetAllAsync(cmp => cmp.Username.Equals("johnw")).ConfigureAwait(false);

            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanGetSingle()
        {
            var user = await _userRepository.GetSingleAsync(cmp => cmp.Username.Equals("johnw")).ConfigureAwait(false);

            Assert.True(user != null);
        }

        [Fact]
        public async Task CanGetAll()
        {
            var companies = await _userRepository.GetAllAsync().ConfigureAwait(false);

            Assert.True(companies.Any());
        }

        [Fact]
        public async Task CanUpdate()
        {
            var user = new User { EmployeeId = 1, Username = "johnw", Password = "sfd$%fsaDgw4564", Token = string.Empty };
            var updatedUser = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
            
            Assert.Equal("sfd$%fsaDgw4564", updatedUser.Password);
        }
    }
}
