using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CompanyApi.Contracts.Entities;
using CompanyApi.Persistence.DbContexts;
using CompanyApi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Services.Repositories
{
	public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
	{
		public EmployeeRepository(ApplicationDbContext appDbContext) : base(appDbContext)
		{
		}

		public override async Task<Employee> GetSingleAsync(Expression<Func<Employee, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			Employee employee = null;
			try
			{
				// https://www.learnentityframeworkcore.com/dbset/querying-data
				employee = await DatabaseSet
                    .Include(emp => emp.Company)
                    .Include(emp => emp.Department)
                    .Include(emp => emp.EmployeeAddress)
                    .Include(emp => emp.User)
					.AsNoTracking()
					.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
				return employee;
			}
			catch (Exception)
			{
				return employee;
			}
		}

		public override async Task<IList<Employee>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			IQueryable<Employee> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			var result = await query
                .Include(emp => emp.Company)
                .Include(emp => emp.Department)
				.Include(emp => emp.EmployeeAddress)
                .Include(emp => emp.User)
				.AsNoTracking()
				.ToListAsync(cancellationToken).ConfigureAwait(false);
			return result;
		}
	}
}
