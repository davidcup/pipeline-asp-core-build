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
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        // https://www.learnentityframeworkcore.com/dbset/querying-data
        public override async Task<Department> GetSingleAsync(Expression<Func<Department, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            Department company = null;
            try
            {
                company = await DatabaseSet
                    .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                    .Include(cmp => cmp.Employees).ThenInclude(emp => emp.Department)
                    .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
                return company;
            }
            catch (Exception)
            {
                return company;
            }
        }

        public override async Task<IList<Department>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Department> query = DatabaseSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            var result = await query
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.Department)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
