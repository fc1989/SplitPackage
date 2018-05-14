using Abp.Domain.Entities;
using Abp.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization.Roles;
using SplitPackage.Authorization.Users;
using SplitPackage.Business;
using SplitPackage.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SplitPackage.EntityFrameworkCore
{
    public class SplitPackageDbContext : AbpZeroDbContext<Tenant, Role, User, SplitPackageDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Product> Products { get; set; }
        public DbSet<Logistic> Logistics { get; set; }
        public DbSet<LogisticChannel> LogisticChannels { get; set; }
        public DbSet<NumFreight> NumFreights { get; set; }
        public DbSet<SplitRule> SplitRules { get; set; }
        public DbSet<SplitRuleProductClass> SplitRuleProductClass { get; set; }
        public DbSet<WeightFreight> WeightFreights { get; set; }
        public DbSet<TenantLogisticChannel> TenantLogisticChannel { get; set; }

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

            base.OnModelCreating(modelBuilder);
        }

        protected override void CheckAndSetMayHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId)
            {
                return;
            }

            //Only set IMayHaveTenant entities
            if (!(entityAsObj is IMayHaveTenant))
            {
                return;
            }

            var entity = entityAsObj.As<IMayHaveTenant>();

            //Don't set if it's already set
            if (entity.TenantId != null)
            {
                return;
            }

            entity.TenantId = GetCurrentTenantIdOrNull();
        }
    }
}
