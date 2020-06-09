using CompanyApi.Contracts.Entities;

namespace CompanyApi.Services.Authorization
{
	public interface IUserService
	{
		User Authenticate(string username, string password);
	}
}
