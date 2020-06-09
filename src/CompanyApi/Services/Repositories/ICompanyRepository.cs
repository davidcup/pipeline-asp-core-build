using CompanyApi.Contracts.Entities;
using CompanyApi.Persistence.Repositories;

namespace CompanyApi.Services.Repositories
{
	public interface ICompanyRepository : IBaseRepository<Company>
    { 
    }
}
