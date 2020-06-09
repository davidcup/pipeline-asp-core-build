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
	public class UserRepository : BaseRepository<User>, IUserRepository
	{
		public UserRepository(ApplicationDbContext appDbContext) : base(appDbContext)
		{
		}

        public override async Task<IList<User>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<User> query = DatabaseSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query
                .Include(usr => usr.Employee)
                .ThenInclude(emp => emp.Company)
                .ThenInclude(cmpemp => cmpemp.Employees)
                .ToListAsync(cancellationToken);
        }

        // https://www.learnentityframeworkcore.com/dbset/querying-data
        public override async Task<User> GetSingleAsync(Expression<Func<User, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			User user = null;
			try
			{
				user = await DatabaseSet
                    .Include(cmp => cmp.Employee).ThenInclude(emp => emp.EmployeeAddress)
                    .Include(cmp => cmp.Employee).ThenInclude(emp => emp.Department)
					.AsNoTracking()
					.SingleOrDefaultAsync(predicate, cancellationToken);
				return user;
			}
			catch (Exception)
			{
				return user;
			}
		}
	}
}
