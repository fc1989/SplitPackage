using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using SplitPackage.Authorization.Roles;
using SplitPackage.Authorization.Users;
using SplitPackage.MultiTenancy;
using Abp.Application.Editions;
using Abp.EntityHistory;
using SplitPackage.Business;
using Abp.Runtime.Session;
using System.Linq.Expressions;
using System;
using Abp.Domain.Entities;
using System.Collections.Generic;

namespace SplitPackage.EntityFrameworkCore
{
    public class SplitPackageDbContext : AbpZeroDbContext<Tenant, Role, User, SplitPackageDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Product> Products { get; set; }
        public DbSet<Logistic> Logistics { get; set; }
        public DbSet<LogisticLine> LogisticLines { get; set; }
        public DbSet<NumFreight> NumFreights { get; set; }
        public DbSet<ProductClass> ProductClasses { get; set; }
        public DbSet<ProductProductClass> ProductProductClass { get; set; }
        public DbSet<SplitRule> SplitRules { get; set; }
        public DbSet<SplitRuleProductClass> SplitRuleProductClass { get; set; }
        public DbSet<WeightFreight> WeightFreights { get; set; }

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
            modelBuilder.Entity<SplitRuleProductClass>().HasKey(p => new { p.SplitRuleId, p.ProductClassId });

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductClasses)
                .WithOne(p => p.ProductBy)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductClass>()
                .HasMany(p => p.Products)
                .WithOne(p => p.ProductClassBy)
                .HasForeignKey(p=>p.ProductClassId);

            modelBuilder.Entity<ProductProductClass>()
                .HasOne(bc => bc.ProductBy)
                .WithMany(b => b.ProductClasses)
                .HasForeignKey(bc => bc.ProductId);

            modelBuilder.Entity<ProductProductClass>()
                .HasOne(bc => bc.ProductClassBy)
                .WithMany(c => c.Products)
                .HasForeignKey(bc => bc.ProductClassId);

            base.OnModelCreating(modelBuilder);
        }

        protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                /* This condition should normally be defined as below:
                 * !IsSoftDeleteFilterEnabled || !((ISoftDelete) e).IsDeleted
                 * But this causes a problem with EF Core (see https://github.com/aspnet/EntityFrameworkCore/issues/9502)
                 * So, we made a workaround to make it working. It works same as above.
                 */

                Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted || ((ISoftDelete)e).IsDeleted != IsSoftDeleteFilterEnabled;
                expression = expression == null ? softDeleteFilter : CombineExpressions(expression, softDeleteFilter);
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                /* This condition should normally be defined as below:
                 * !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant)e).TenantId == CurrentTenantId
                 * But this causes a problem with EF Core (see https://github.com/aspnet/EntityFrameworkCore/issues/9502)
                 * So, we made a workaround to make it working. It works same as above.
                 */
                Expression<Func<TEntity, bool>> mayHaveTenantFilter;
                List<Type> types = new List<Type> { typeof(User), typeof(Role)};
                if (types.Contains(typeof(TEntity)))
                {
                    mayHaveTenantFilter = e => ((IMayHaveTenant)e).TenantId == CurrentTenantId || (((IMayHaveTenant)e).TenantId == CurrentTenantId) == IsMayHaveTenantFilterEnabled;
                }
                else
                {
                    mayHaveTenantFilter = e => CurrentTenantId.HasValue ? ((IMayHaveTenant)e).TenantId == CurrentTenantId || ((IMayHaveTenant)e).TenantId == null : ((IMayHaveTenant)e).TenantId == null;
                }

                expression = expression == null ? mayHaveTenantFilter : CombineExpressions(expression, mayHaveTenantFilter);
            }

            if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                /* This condition should normally be defined as below:
                 * !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant)e).TenantId == CurrentTenantId
                 * But this causes a problem with EF Core (see https://github.com/aspnet/EntityFrameworkCore/issues/9502)
                 * So, we made a workaround to make it working. It works same as above.
                 */
                Expression<Func<TEntity, bool>> mustHaveTenantFilter = e => ((IMustHaveTenant)e).TenantId == CurrentTenantId || (((IMustHaveTenant)e).TenantId == CurrentTenantId) == IsMustHaveTenantFilterEnabled;
                expression = expression == null ? mustHaveTenantFilter : CombineExpressions(expression, mustHaveTenantFilter);
            }

            return expression;
        }
    }
}
