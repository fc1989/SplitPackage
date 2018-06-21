using Abp.Application.Editions;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using SplitPackage.Authorization;
using SplitPackage.Authorization.Roles;
using SplitPackage.Editions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests
{
    /// <summary>
    /// 测试初始化数据是否设置正确
    /// </summary>
    public class SplitInitial_Test : SplitPackageTestBase
    {
        public SplitInitial_Test(Xunit.Abstractions.ITestOutputHelper output)
        {

        }

        protected async Task EntityValid(object obj, int? tenantId = null)
        {
            obj.ShouldNotBeNull();
            if (obj is ICreationAudited)
            {
                var cobj = (ICreationAudited)obj;
                Assert.Null(cobj.CreatorUserId);
                Assert.True(cobj.CreationTime.Year == DateTime.Now.Year);
            }
            if (obj is ISoftDelete)
            {
                var dobj = (ISoftDelete)obj;
                Assert.False(dobj.IsDeleted);
            }
            if (obj is IPassivable)
            {
                var iobj = (IPassivable)obj;
                Assert.True(iobj.IsActive);
            }
            if (obj is IMayHaveTenant)
            {
                var tobj = (IMayHaveTenant)obj;
                if (!tenantId.HasValue)
                {
                    Assert.Null(tobj.TenantId);
                }
                else
                {
                    Assert.Equal(tenantId.Value, tobj.TenantId.Value);
                }
            }
            if (obj is IMustHaveTenant)
            {
                var tobj = (IMustHaveTenant)obj;
                Assert.Equal(tenantId.Value, tobj.TenantId);
            }
            await Task.CompletedTask;
        }

        [Fact]
        public async Task EditionsInitial_test()
        {
            await UsingDbContextAsync(async context =>
            {
                Edition edition = await context.Editions.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Name == EditionManager.DefaultEditionName);
                edition.ShouldNotBeNull();
                Assert.Equal(EditionManager.DefaultEditionName, edition.DisplayName);
                await EntityValid(edition);
            });
        }

        [Fact]
        public async Task LanguageInitial_test()
        {
            await UsingDbContextAsync(async context =>
            {
                List<ApplicationLanguage> als = context.Languages.IgnoreQueryFilters().Where(e => e.TenantId == null && (e.Name == "en-US" || e.Name == "zh-Hans")).ToList();
                Assert.Equal(2, als.Count);
                var zh = als.FirstOrDefault(o => o.DisplayName == "简体中文");
                await EntityValid(zh);
                var cn = als.FirstOrDefault(o => o.DisplayName == "English");
                await EntityValid(cn);
                await Task.CompletedTask;
            });
        }

        [Fact]
        public async Task HostRoleAndUsersInitial_test()
        {
            await UsingDbContextAsync(async context =>
            {
                var adminRoleForHost = context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
                await EntityValid(adminRoleForHost);
                var rolePermissions = context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == null && p.RoleId == adminRoleForHost.Id).ToList();
                var permissions = PermissionFinder
                    .GetAllPermissions(new SplitPackageAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host))
                    .ToList();
                rolePermissions.Count.ShouldBeGreaterThanOrEqualTo(permissions.Count);
                foreach (var item in rolePermissions)
                {
                    await EntityValid(item);
                }
                var adminUserForHost = context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
                await EntityValid(adminUserForHost);
                Assert.Equal(AbpUserBase.AdminUserName, adminUserForHost.UserName);
                Assert.Equal("admin", adminUserForHost.Name);
                Assert.Equal("admin", adminUserForHost.Surname);
                Assert.Equal("admin@aspnetboilerplate.com", adminUserForHost.EmailAddress);
                Assert.True(adminUserForHost.IsEmailConfirmed);
                var userRole = await context.UserRoles.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.UserId == adminUserForHost.Id && o.RoleId == adminRoleForHost.Id);
                await EntityValid(userRole);
                await Task.CompletedTask;
            });
        }

        [Fact]
        public async Task SettingInitial_test()
        {
            await UsingDbContextAsync(async context =>
            {
                var dfa = await context.Settings.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Name == EmailSettingNames.DefaultFromAddress && s.TenantId == null && s.UserId == null);
                await EntityValid(dfa);
                Assert.Equal("admin@mydomain.com", dfa.Value);
                var dfd = await context.Settings.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Name == EmailSettingNames.DefaultFromDisplayName && s.TenantId == null && s.UserId == null);
                await EntityValid(dfd);
                Assert.Equal("mydomain.com mailer", dfd.Value);
                var ls = await context.Settings.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Name == LocalizationSettingNames.DefaultLanguage && s.TenantId == null && s.UserId == null);
                await EntityValid(ls);
                Assert.Equal("zh-Hans", ls.Value);
            });
        }

        [Fact]
        public async Task TenantInitial_test()
        {
            await UsingDbContextAsync(async context =>
            {
                var tenant = context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == AbpTenantBase.DefaultTenantName);
                await EntityValid(tenant);
                Assert.Equal(AbpTenantBase.DefaultTenantName, tenant.Name);
            });
        }

        [Fact]
        public async Task InitialBusiness_test()
        {
            await UsingDbContextAsync(async context =>
            {
                var logistic = await context.Logistics.FirstOrDefaultAsync(s => s.LogisticCode == "AOLAU EXPRESS" && s.TenantId == null);
                await EntityValid(logistic);
                Assert.Equal("http://www.aol-au.com", logistic.CorporationUrl);
                var line = await context.LogisticChannels.FirstOrDefaultAsync(s => s.TenantId == null && s.LogisticId == logistic.Id);
                await EntityValid(line);
                var wf = await context.WeightFreights.FirstOrDefaultAsync(s => s.LogisticChannelId == line.Id &&
                    s.Currency == "AUD" &&
                    s.Unit == "g" &&
                    s.StartingPrice == 4.5 &&
                    s.StartingWeight == 1000 &&
                    s.EndWeight == 10000000 &&
                    s.StepWeight == 100 &&
                    s.CostPrice == 0.45 &&
                    s.Price == 0.45
                );
                await EntityValid(wf);
                var sr = await context.SplitRules.FirstOrDefaultAsync(s => s.LogisticChannelId == line.Id &&
                s.MaxPackage == 3 &&
                s.MaxWeight == 40000 &&
                s.MaxTax == 10000 &&
                s.MaxPrice == 10000);
                await EntityValid(sr);
                var srpcs = context.SplitRuleProductClass.Where(o => o.SplitRuleId == sr.Id);
                Assert.Equal(8, srpcs.Count());
                foreach (var item in srpcs)
                {
                    await EntityValid(item);
                }
            });
        }

        [Fact]
        public async Task TenantRoleAndUserInitial_test()
        {
            await UsingDbContextAsync(async context =>
            {
                var role = context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == 1 && r.Name == StaticRoleNames.Tenants.Admin);
                await EntityValid(role, 1);
                var rolePermissions = context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == 1 && p.RoleId == role.Id).ToList();
                var permissions = PermissionFinder
                    .GetAllPermissions(new SplitPackageAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant))
                    .ToList();
                rolePermissions.Count.ShouldBeGreaterThanOrEqualTo(permissions.Count);
                foreach (var item in rolePermissions)
                {
                    await EntityValid(item,1);
                }
                var adminUserForHost = context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == 1 && u.UserName == AbpUserBase.AdminUserName);
                await EntityValid(adminUserForHost,1);
                Assert.Equal(AbpUserBase.AdminUserName, adminUserForHost.UserName);
                Assert.Equal("admin", adminUserForHost.Name);
                Assert.Equal("admin", adminUserForHost.Surname);
                Assert.Equal("admin@defaulttenant.com", adminUserForHost.EmailAddress);
                Assert.True(adminUserForHost.IsEmailConfirmed);
                var userRole = await context.UserRoles.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.TenantId == 1 && o.UserId == adminUserForHost.Id && o.RoleId == role.Id);
                await EntityValid(userRole,1);
            });
        }
    }
}
