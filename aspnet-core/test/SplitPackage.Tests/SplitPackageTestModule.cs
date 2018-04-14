using System;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Configuration.Startup;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using SplitPackage.Tests.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace SplitPackage.Tests
{
    [DependsOn(
        typeof(SplitPackageApplicationModule),
        typeof(SplitPackageEntityFrameworkModule),
        typeof(AbpTestBaseModule)
        )]
    public class SplitPackageTestModule : AbpModule
    {
        public SplitPackageTestModule(SplitPackageEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;
        }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            Configuration.UnitOfWork.IsTransactional = false;

            // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<AbpZeroDbMigrator<SplitPackageDbContext>>();

            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);

            //全局筛选器,并发请求异常
            //替换成串行测试方式,不会导致该异常
            AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue9825", true);
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(IocManager);
        }

        private void RegisterFakeService<TService>() where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );
        }
    }
}
