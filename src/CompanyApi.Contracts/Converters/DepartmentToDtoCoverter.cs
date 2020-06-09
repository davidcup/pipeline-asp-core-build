using System.Collections.Generic;
using System.Linq;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using Microsoft.Extensions.Logging;

namespace CompanyApi.Contracts.Converters
{
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
	public class DepartmentToDtoCoverter : IConverter<Department, DepartmentDto>, IConverter<IList<Department>, IList<DepartmentDto>>
	{
		private readonly ILogger<DepartmentToDtoCoverter> _logger;

		public DepartmentToDtoCoverter(ILogger<DepartmentToDtoCoverter> logger)
		{
			_logger = logger;
		}

		public DepartmentDto Convert(Department department)
		{
			_logger.LogDebug("Convert");
			var departmentDto = new DepartmentDto
			{
				DepartmentId = department.DepartmentId,
				Name = department.Name
			};

			foreach (var employee in department.Employees)
            {
                var addressStr = employee.EmployeeAddress == null ? "N/A" : employee.EmployeeAddress.Address;
                var departmentStr = employee.Department == null ? "N/A" : employee.Department.Name;
                var username = employee.User == null ? "N/A" : employee.User.Username;
                var employeeDto = $"{employee.FirstName} {employee.LastName}, Address: {addressStr}, Department: {departmentStr}, Username: {username}";
                departmentDto.Employees.Add(employeeDto);
			}
			return departmentDto;
		}

		public IList<DepartmentDto> Convert(IList<Department> companies)
		{
			_logger.LogDebug("ConvertList");
			return companies.Select(cmp =>
			{
				var departmentDto = Convert(cmp);
				return departmentDto;
			}).ToList();
		}
	}
}
