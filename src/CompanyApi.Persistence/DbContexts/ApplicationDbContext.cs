using System;
using System.Linq;
using CompanyApi.Contracts.Entities;
using CompanyApi.Contracts.Entities.Base;
using CompanyApi.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Persistence.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
	    public ApplicationDbContext(DbContextOptions options) 
		    : base(options)
        {
        }

		public DbSet<Company> Companies { get; set; }

	    public DbSet<Department> Departments { get; set; }

	    public DbSet<Employee> Employees { get; set; }

	    public DbSet<EmployeeAddress> EmployeeAddresses { get; set; }

	    public DbSet<User> Users { get; set; }

		public override int SaveChanges()
		{
			TrackChanges();
			return base.SaveChanges();
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new CompanyConfiguration(modelBuilder.Entity<Company>());
	        new EmployeeConfiguration(modelBuilder.Entity<Employee>());
			new EmployeeAddressConfiguration(modelBuilder.Entity<EmployeeAddress>());
	        new DepartmentConfiguration(modelBuilder.Entity<Department>());
	        new UserConfiguration(modelBuilder.Entity<User>());
        }

        private void TrackChanges()
        {
            var entries = ChangeTracker.Entries()
	            .Where(x => x.Entity is IBaseAuditEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
	            if (entry.State == EntityState.Added)
                {
                    ((IBaseAuditEntity)entry.Entity).Created = DateTime.UtcNow;
                }
                ((IBaseAuditEntity)entry.Entity).Modified = DateTime.UtcNow;
            }
        }
    }
}
