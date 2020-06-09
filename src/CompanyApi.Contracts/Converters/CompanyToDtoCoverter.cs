using System.Collections.Generic;
using System.Linq;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using Microsoft.Extensions.Logging;

namespace CompanyApi.Contracts.Converters
{
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
	public class CompanyToDtoCoverter : IConverter<Company, CompanyDto>, IConverter<IList<Company>, IList<CompanyDto>>
	{
		private readonly ILogger<CompanyToDtoCoverter> _logger;

		public CompanyToDtoCoverter(ILogger<CompanyToDtoCoverter> logger)
		{
			_logger = logger;
		}

		public CompanyDto Convert(Company company)
		{
			_logger.LogDebug("Convert");
			var companyDto = new CompanyDto
			{
				CompanyId = company.CompanyId,
				Name = company.Name
			};

			foreach (var employee in company.Employees)
            {
                var address = employee.EmployeeAddress == null ? "N/A" : employee.EmployeeAddress.Address;
                var department = employee.Department == null ? "N/A" : employee.Department.Name;
                var username = employee.User == null ? "N/A" : employee.User.Username;
                var employeeDto = $"{employee.FirstName} {employee.LastName}, Address: {address}, Department: {department}, Username: {username}";
                companyDto.Employees.Add(employeeDto);
			}
			return companyDto;
		}

		public IList<CompanyDto> Convert(IList<Company> companies)
		{
			_logger.LogDebug("ConvertList");
			return companies.Select(cmp =>
			{
				var companyDto = Convert(cmp);
				return companyDto;
			}).ToList();
		}
	}
}
