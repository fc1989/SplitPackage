using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using SplitPackage.Authorization.Roles;
using SplitPackage.Authorization.Users;
using SplitPackage.MultiTenancy;
using Abp.Application.Editions;
using Abp.EntityHistory;
using SplitPackage.Business;
using Abp.Runtime.Session;

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
    }
}
