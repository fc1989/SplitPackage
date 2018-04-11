using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp;
using Abp.Authorization.Users;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.TestBase;
using SplitPackage.Authorization.Users;
using SplitPackage.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore.Seed.Host;
using SplitPackage.EntityFrameworkCore.Seed.Tenants;
using SplitPackage.MultiTenancy;
using Abp.AspNetCore.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Abp.AspNetCore;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Abp.Domain.Uow;
using Xunit;
using Abp.TestBase.Runtime.Session;
using Microsoft.AspNetCore.TestHost;
using Abp.Dependency;

namespace SplitPackage.Tests
{
    [Collection("SplitPackage collection")]
    public abstract class SplitPackageTestBase
    {
        protected TestContext context;

        public SplitPackageTestBase(TestContext testContext)
        {
            context = testContext;
            void NormalizeDbContext(SplitPackageDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                context.SuppressAutoSetTenantId = true;
            }

            // Seed initial data for host
            this.context.TestSession.TenantId = null;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new InitialHostDbBuilder(context).Create();
                new DefaultTenantBuilder(context).Create();
            });

            // Seed initial data for default tenant
            this.context.TestSession.TenantId = 1;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new TenantRoleAndUserBuilder(context, 1).Create();
            });

            LoginAsDefaultTenantAdmin();
        }

        #region UsingDbContext

        protected IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = this.context.TestSession.TenantId;
            this.context.TestSession.TenantId = tenantId;
            return new DisposeAction(() => this.context.TestSession.TenantId = previousTenantId);
        }

        protected void UsingDbContext(Action<SplitPackageDbContext> action)
        {
            UsingDbContext(this.context.TestSession.TenantId, action);
        }

        protected Task UsingDbContextAsync(Func<SplitPackageDbContext, Task> action)
        {
            return UsingDbContextAsync(this.context.TestSession.TenantId, action);
        }

        protected T UsingDbContext<T>(Func<SplitPackageDbContext, T> func)
        {
            return UsingDbContext(this.context.TestSession.TenantId, func);
        }

        protected Task<T> UsingDbContextAsync<T>(Func<SplitPackageDbContext, Task<T>> func)
        {
            return UsingDbContextAsync(this.context.TestSession.TenantId, func);
        }

        protected void UsingDbContext(int? tenantId, Action<SplitPackageDbContext> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = this.context.TestIocManager.Resolve<SplitPackageDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<SplitPackageDbContext, Task> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = this.context.TestIocManager.Resolve<SplitPackageDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected T UsingDbContext<T>(int? tenantId, Func<SplitPackageDbContext, T> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = this.context.TestIocManager.Resolve<SplitPackageDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<SplitPackageDbContext, Task<T>> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = this.context.TestIocManager.Resolve<SplitPackageDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync();
                }
            }

            return result;
        }

        #endregion

        #region Login

        protected void LoginAsHostAdmin()
        {
            LoginAsHost(AbpUserBase.AdminUserName);
        }

        protected void LoginAsDefaultTenantAdmin()
        {
            LoginAsTenant(AbpTenantBase.DefaultTenantName, AbpUserBase.AdminUserName);
        }

        protected void LoginAsHost(string userName)
        {
            this.context.TestSession.TenantId = null;

            var user =
                UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == this.context.TestSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            this.context.TestSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            var tenant = UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            this.context.TestSession.TenantId = tenant.Id;

            var user =
                UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == this.context.TestSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            this.context.TestSession.UserId = user.Id;
        }

        #endregion

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = this.context.TestSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = this.context.TestSession.GetTenantId();
            return await UsingDbContext(context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }

        protected T Resolve<T>()
        {
            return this.context.TestIocManager.Resolve<T>();
        }
    }

    public class TestContext : AbpAspNetCoreIntegratedTestBase<TestContext.Startup>, IDisposable
    {
        public TestContext()
        {

        }

        public void Dispose()
        {

        }

        public TestAbpSession TestSession { get { return this.AbpSession; } }

        public TestServer TestServer { get { return this.Server; } }

        public IServiceProvider TestServiceProvider { get { return this.ServiceProvider; } }

        public IIocManager TestIocManager { get { return this.IocManager; } }

        public class Startup
        {
            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                return services.AddAbp<SplitPackageTestModule>(options =>
                {
                    options.SetupTest();
                });
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseAbp();

                app.UseUnitOfWork();

                app.Use(async (context, func) =>
                {
                    await context.Response.WriteAsync(
                        context.RequestServices.GetRequiredService<IUnitOfWorkManager>().Current == null
                            ? "null"
                            : "not-null"
                    );
                });
            }
        }
    }

    [CollectionDefinition("SplitPackage collection")]
    public class SplitPackageCollection : ICollectionFixture<TestContext>
    {

    }
}