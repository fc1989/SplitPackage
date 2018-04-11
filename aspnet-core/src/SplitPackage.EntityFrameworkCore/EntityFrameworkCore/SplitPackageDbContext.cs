using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using SplitPackage.Authorization.Roles;
using SplitPackage.Authorization.Users;
using SplitPackage.MultiTenancy;
using Abp.Application.Editions;
using Abp.EntityHistory;

namespace SplitPackage.EntityFrameworkCore
{
    public class SplitPackageDbContext : AbpZeroDbContext<Tenant, Role, User, SplitPackageDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public SplitPackageDbContext(DbContextOptions<SplitPackageDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>("");
            modelBuilder.Entity<Tenant>().HasOne(p => p.CreatorUser).WithOne().IsRequired(false).HasForeignKey<Tenant>("CreatorUserId");
            modelBuilder.Entity<Tenant>().HasOne(p => p.DeleterUser).WithOne().IsRequired(false).HasForeignKey<Tenant>("DeleterUserId");
            modelBuilder.Entity<Tenant>().HasOne(p => p.LastModifierUser).WithOne().IsRequired(false).HasForeignKey<Tenant>("LastModifierUserId");
        }
    }
}
