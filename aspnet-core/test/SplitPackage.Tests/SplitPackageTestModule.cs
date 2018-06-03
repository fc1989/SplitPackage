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
using Abp.Domain.Uow;
using System.Transactions;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using SplitPackage.Cache;

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

        public override void PostInitialize()
        {
            //using (var uowManager = IocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            //{
            //    using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
            //    {
            //        var context = uowManager.Object.Current.GetDbContext<SplitPackageDbContext>(MultiTenancySides.Host);
            //        new EntityFrameworkCore.Seed.Host.InitialHostDbBuilder(context).Create();
            //        new EntityFrameworkCore.Seed.Business.InitialBusinessDbBuilder(context).Create();
            //        new EntityFrameworkCore.Seed.Tenants.DefaultTenantBuilder(context, new string[] { Abp.MultiTenancy.AbpTenantBase.DefaultTenantName, "AstraeaAssistant", "Auz", "Iautao", "Phoenix" }).Create();
            //        new EntityFrameworkCore.Seed.Tenants.DefaultTenantBusinessBuilder(context).Create();
            //        uow.Complete();
            //    }
            //}
            //不规范实现
            var init = IocManager.Resolve<ManageCache>();
            init.InitCache();
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
