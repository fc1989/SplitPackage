using Abp;
using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SplitPackage.Authorization.Users;
using SplitPackage.Cache;
using SplitPackage.Cache.Dto;
using SplitPackage.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore.Seed.Business;
using SplitPackage.EntityFrameworkCore.Seed.Host;
using SplitPackage.EntityFrameworkCore.Seed.Tenants;
using SplitPackage.MultiTenancy;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SplitPackage.Tests
{
    public abstract class BaseCaseContext : AbpAspNetCoreIntegratedTestBase<Startup>, IDisposable
    {
        public BaseCaseContext()
        {
            void NormalizeDbContext(SplitPackageDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                //context.SuppressAutoSetTenantId = true;
            }

            // Seed initial data for host
            AbpSession.TenantId = null;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new InitialHostDbBuilder(context).Create();
                new InitialBusinessDbBuilder(context).Create();
                new DefaultTenantBuilder(context, new string[] { AbpTenantBase.DefaultTenantName, "AstraeaAssistant", "Auz", "Iautao", "Phoenix" }).Create();
                new DefaultTenantBusinessBuilder(context).Create();
                this.InitData(context);
            });

            // Seed initial data for default Product
            AbpSession.TenantId = 1;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
            });

            var init = IocManager.Resolve<ManageCache>();
            init.InitCache();

            LoginAsHostAdmin();
        }

        public abstract void InitData(SplitPackageDbContext context);

        #region UsingDbContext

        public IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = AbpSession.TenantId;
            AbpSession.TenantId = tenantId;
            return new DisposeAction(() => AbpSession.TenantId = previousTenantId);
        }

        public void UsingDbContext(Action<SplitPackageDbContext> action)
        {
            UsingDbContext(AbpSession.TenantId, action);
        }

        public Task UsingDbContextAsync(Func<SplitPackageDbContext, Task> action)
        {
            return UsingDbContextAsync(AbpSession.TenantId, action);
        }

        public T UsingDbContext<T>(Func<SplitPackageDbContext, T> func)
        {
            return UsingDbContext(AbpSession.TenantId, func);
        }

        public Task<T> UsingDbContextAsync<T>(Func<SplitPackageDbContext, Task<T>> func)
        {
            return UsingDbContextAsync(AbpSession.TenantId, func);
        }

        public void UsingDbContext(int? tenantId, Action<SplitPackageDbContext> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = IocManager.Resolve<SplitPackageDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        public async Task UsingDbContextAsync(int? tenantId, Func<SplitPackageDbContext, Task> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = IocManager.Resolve<SplitPackageDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        public T UsingDbContext<T>(int? tenantId, Func<SplitPackageDbContext, T> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = IocManager.Resolve<SplitPackageDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        public async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<SplitPackageDbContext, Task<T>> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = IocManager.Resolve<SplitPackageDbContext>())
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

        protected void LoginAsHost(string userName)
        {
            AbpSession.TenantId = null;

            var user =
                UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            AbpSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            var tenant = UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            AbpSession.TenantId = tenant.Id;

            var user =
                UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            AbpSession.UserId = user.Id;
        }
        #endregion

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        public async Task<User> GetCurrentUserAsync()
        {
            var userId = AbpSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = AbpSession.GetTenantId();
            return await UsingDbContext(context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }

        #region cache
        public async Task UsingCurrentSettingCacheAsync(Func<SplitPackageSettingCache, Task> action)
        {
            var mc = IocManager.Resolve<ManageCache>();
            var settingCache = await mc.GetSplitPackageSettingAsync(AbpSession.TenantId);
            await action(settingCache);
        }

        public async Task UsingTenantSettingCacheAsync(Func<SplitPackageSettingCache, Task> action)
        {
            var mc = IocManager.Resolve<ManageCache>();
            using (var unitOfWork = IocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                var tenants = await IocManager.Resolve<IRepository<Tenant>>().GetAll().ToListAsync();
                foreach (var item in tenants)
                {
                    var settingCache = await mc.GetSplitPackageSettingAsync(item.Id);
                    await action(settingCache);
                }
            }
        }
        #endregion

        public int? GetTenantId()
        {
            return this.AbpSession.TenantId;
        }

        public T ResolveService<T>()
        {
            return this.Resolve<T>();
        }

        public virtual void Dispose()
        {

        }
    }

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