using Abp.EntityFrameworkCore.Configuration;
using Abp.EntityHistory;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore.Seed;

namespace SplitPackage.EntityFrameworkCore
{
    [DependsOn(
        typeof(SplitPackageCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class SplitPackageEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<SplitPackageDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        SplitPackageDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        SplitPackageDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SplitPackageEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
