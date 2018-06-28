using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using SplitPackage.Business;
using SplitPackage.Business.SplitRules;
using SplitPackage.Business.SplitRules.Dto;
using SplitPackage.Tests.Contexts;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.Business
{
    [Collection("admin collection")]
    public class SplitRuleAppService_Admin_Tests
    {
        private readonly SplitRuleAppService _service;
        private readonly AdminCase _context;

        public SplitRuleAppService_Admin_Tests(Xunit.Abstractions.ITestOutputHelper output, AdminCase context)
        {
            this._context = context;
            this._service = context.ResolveService<SplitRuleAppService>();
        }

        [Fact]
        public async Task Add_Update_Delete_Test()
        {
            long id = await this.Create_Test();
            await this.Update_Test(id);
            await this.Switch_Stop_Test(id);
            await this.Switch_Enable_Test(id);
            await this.Delete_Test(id);
        }

        protected async Task<long> Create_Test()
        {
            LogisticChannel lc;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                lc = await this._context.ResolveService<IRepository<LogisticChannel, long>>().GetAll().Include(o => o.LogisticBy).FirstAsync();
            }
            var input = new SplitPackage.Business.SplitRules.Dto.CreateSplitRuleDto()
            {
                LogisticChannelId = lc.Id,
                RuleName = "测试添加规则",
                MaxPackage = 10,
                MaxWeight = 10,
                MaxTax = 10,
                MaxPrice = 10
            };
            var result = await this._service.Create(input);
            result.Id.ShouldBeGreaterThan(0);
            result.IsActive.ShouldBeTrue();
            result.LogisticChannelId.ShouldBe(lc.Id);
            result.LogisticName.ShouldBe(lc.LogisticBy.CorporationName);
            result.LogisticChannelName.ShouldBe(lc.ChannelName);
            result.RuleName.ShouldBe(input.RuleName);
            result.MaxPackage.ShouldBe(input.MaxPackage);
            result.MaxWeight.ShouldBe(input.MaxWeight);
            result.MaxTax.ShouldBe(input.MaxTax);
            result.MaxPrice.ShouldBe(input.MaxPrice);
            result.TenantId.ShouldBeNull();
            await this._context.UsingDbContextAsync(async context =>
            {
                var sr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == result.Id);
                sr.ShouldNotBeNull();
                sr.RuleName.ShouldBe(result.RuleName);
                sr.MaxPackage.ShouldBe(result.MaxPackage);
                sr.MaxWeight.ShouldBe(result.MaxWeight);
                sr.MaxTax.ShouldBe(result.MaxTax);
                sr.MaxPrice.ShouldBe(result.MaxPrice);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == lc.LogisticId);
                l.ShouldNotBeNull();
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == lc.Id);
                lC.ShouldNotBeNull();
                var sr = lC.SplitRules.FirstOrDefault(o => o.Id == result.Id);
                sr.ShouldNotBeNull();
                sr.Id.ShouldBe(result.Id);
                sr.RuleName.ShouldBe(result.RuleName);
                sr.MaxPackage.ShouldBe(result.MaxPackage);
                sr.MaxWeight.ShouldBe(result.MaxWeight);
                sr.MaxTax.ShouldBe(result.MaxTax);
                sr.MaxPrice.ShouldBe(result.MaxPrice);
                sr.ProductClasses.ShouldBeNull();
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == lc.LogisticId);
                if (l == null)
                    return;
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == lc.Id);
                if (lC == null)
                    return;
                var sr = lC.SplitRules.FirstOrDefault(o => o.Id == result.Id);
                sr.ShouldNotBeNull();
                sr.Id.ShouldBe(result.Id);
                sr.RuleName.ShouldBe(result.RuleName);
                sr.MaxPackage.ShouldBe(result.MaxPackage);
                sr.MaxWeight.ShouldBe(result.MaxWeight);
                sr.MaxTax.ShouldBe(result.MaxTax);
                sr.MaxPrice.ShouldBe(result.MaxPrice);
                sr.ProductClasses.ShouldBeNull();
                await Task.FromResult(1);
            });
            return result.Id;
        }

        protected async Task Update_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(o=>o.LogisticChannelBy).ThenInclude(o=>o.LogisticBy).FirstAsync(o=>o.Id == id);
            }
            sr.ShouldNotBeNull();
            var input = new UpdateSplitRuleDto()
            {
                Id = sr.Id,
                LogisticChannelId = sr.LogisticChannelId,
                RuleName = "规则命修改",
                MaxPackage = sr.MaxPackage + 10,
                MaxWeight = sr.MaxWeight + 10,
                MaxTax = sr.MaxTax + 10,
                MaxPrice = sr.MaxPrice + 10
            };
            var result = await this._service.Update(input);
            result.Id.ShouldBeGreaterThan(0);
            result.IsActive.ShouldBeTrue();
            result.LogisticChannelId.ShouldBe(sr.LogisticChannelId);
            result.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
            result.LogisticChannelName.ShouldBe(sr.LogisticChannelBy.ChannelName);
            result.RuleName.ShouldBe(input.RuleName);
            result.MaxPackage.ShouldBe(input.MaxPackage);
            result.MaxWeight.ShouldBe(input.MaxWeight);
            result.MaxTax.ShouldBe(input.MaxTax);
            result.MaxPrice.ShouldBe(input.MaxPrice);
            result.TenantId.ShouldBeNull();
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == result.Id);
                newsr.ShouldNotBeNull();
                newsr.RuleName.ShouldBe(result.RuleName);
                newsr.MaxPackage.ShouldBe(result.MaxPackage);
                newsr.MaxWeight.ShouldBe(result.MaxWeight);
                newsr.MaxTax.ShouldBe(result.MaxTax);
                newsr.MaxPrice.ShouldBe(result.MaxPrice);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == result.Id);
                sR.ShouldNotBeNull();
                sR.Id.ShouldBe(result.Id);
                sR.RuleName.ShouldBe(result.RuleName);
                sR.MaxPackage.ShouldBe(result.MaxPackage);
                sR.MaxWeight.ShouldBe(result.MaxWeight);
                sR.MaxTax.ShouldBe(result.MaxTax);
                sR.MaxPrice.ShouldBe(result.MaxPrice);
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                if (lC == null)
                    return;
                var sR = lC.SplitRules.FirstOrDefault(o => o.Id == result.Id);
                sR.ShouldNotBeNull();
                sR.Id.ShouldBe(result.Id);
                sR.RuleName.ShouldBe(result.RuleName);
                sR.MaxPackage.ShouldBe(result.MaxPackage);
                sR.MaxWeight.ShouldBe(result.MaxWeight);
                sR.MaxTax.ShouldBe(result.MaxTax);
                sR.MaxPrice.ShouldBe(result.MaxPrice);
                await Task.FromResult(1);
            });
        }

        protected async Task Switch_Stop_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            await this._service.Switch(id, false);
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == id);
                newsr.ShouldNotBeNull();
                newsr.IsActive.ShouldBeFalse();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldBeNull();
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                if (lC == null)
                    return;
                var sR = lC.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldBeNull();
                await Task.FromResult(1);
            });
        }

        protected async Task Switch_Enable_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            await this._service.Switch(id, true);
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == id);
                newsr.ShouldNotBeNull();
                newsr.IsActive.ShouldBeTrue();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldNotBeNull();
                sR.Id.ShouldBe(sr.Id);
                sR.RuleName.ShouldBe(sr.RuleName);
                sR.MaxPackage.ShouldBe(sr.MaxPackage);
                sR.MaxWeight.ShouldBe(sr.MaxWeight);
                sR.MaxTax.ShouldBe(sr.MaxTax);
                sR.MaxPrice.ShouldBe(sr.MaxPrice);
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                if (lC == null)
                    return;
                var sR = lC.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldNotBeNull();
                sR.Id.ShouldBe(sr.Id);
                sR.RuleName.ShouldBe(sr.RuleName);
                sR.MaxPackage.ShouldBe(sr.MaxPackage);
                sR.MaxWeight.ShouldBe(sr.MaxWeight);
                sR.MaxTax.ShouldBe(sr.MaxTax);
                sR.MaxPrice.ShouldBe(sr.MaxPrice);
                await Task.FromResult(1);
            });
        }

        protected async Task Delete_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            sr.ShouldNotBeNull();
            await this._service.Delete(new Abp.Application.Services.Dto.EntityDto<long>(id));
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == sr.Id);
                newsr.ShouldBeNull();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldBeNull();
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                if (lC == null)
                    return;
                var sR = lC.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldBeNull();
                await Task.FromResult(1);
            });
        }

        [Fact]
        public async Task Get()
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync();
            }
            sr.ShouldNotBeNull();
            var result = await this._service.Get(new Abp.Application.Services.Dto.EntityDto<long>(sr.Id));
            result.IsActive.ShouldBeTrue();
            result.LogisticChannelId.ShouldBe(sr.LogisticChannelId);
            result.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
            result.LogisticChannelName.ShouldBe(sr.LogisticChannelBy.ChannelName);
            result.RuleName.ShouldBe(sr.RuleName);
            result.MaxPackage.ShouldBe(sr.MaxPackage);
            result.MaxWeight.ShouldBe(sr.MaxWeight);
            result.MaxTax.ShouldBe(sr.MaxTax);
            result.MaxPrice.ShouldBe(sr.MaxPrice);
            result.TenantId.ShouldBeNull();
        }

        [Fact]
        public async Task GetAll()
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).Include(p=>p.ProductClasses).FirstAsync();
            }
            var filter = new SplitRuleSearchFilter()
            {
                LogisticName = sr.LogisticChannelBy.LogisticBy.CorporationName
            };
            var result = await this._service.GetAll(filter);
            result.Items.Count.ShouldBeGreaterThan(0);
            foreach (var item in result.Items)
            {
                item.TenantId.ShouldBeNull();
                item.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
            }
            filter.ChannelName = sr.LogisticChannelBy.ChannelName;
            result = await this._service.GetAll(filter);
            result.Items.Count.ShouldBeGreaterThan(0);
            foreach (var item in result.Items)
            {
                item.TenantId.ShouldBeNull();
                item.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
                item.LogisticChannelName.ShouldBe(sr.LogisticChannelBy.ChannelName);
                item.LogisticChannelId.ShouldBe(sr.LogisticChannelId);
            }
            filter.PTId = sr.ProductClasses.Where(o=>o.Type == RuleItemStintType.PTId).First().StintMark;
            result = await this._service.GetAll(filter);
            result.Items.Any(o => o.Id == sr.Id && o.TenantId == null).ShouldBeTrue();
        }
    }

    [Collection("Assistant collection")]
    public class SplitRuleAppService_Tenant_Tests
    {
        private readonly SplitRuleAppService _service;
        private readonly AssistantCase _context;

        public SplitRuleAppService_Tenant_Tests(Xunit.Abstractions.ITestOutputHelper output, AssistantCase context)
        {
            this._context = context;
            this._service = context.ResolveService<SplitRuleAppService>();
        }

        [Fact]
        public async Task Add_Update_Delete_Test()
        {
            long id = await this.Create_Test();
            await this.Update_Test(id);
            await this.Switch_Stop_Test(id);
            await this.Switch_Enable_Test(id);
            await this.Delete_Test(id);
        }

        protected async Task<long> Create_Test()
        {
            LogisticChannel lc;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                var tlc = await this._context.ResolveService<IRepository<TenantLogisticChannel, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p => p.LogisticChannelBy).ThenInclude(p=>p.LogisticBy).FirstAsync(o=>o.TenantId == this._context.GetTenantId());
                lc = tlc.LogisticChannelBy;
            }
            var input = new SplitPackage.Business.SplitRules.Dto.CreateSplitRuleDto()
            {
                LogisticChannelId = lc.Id,
                RuleName = "测试添加规则",
                MaxPackage = 10,
                MaxWeight = 10,
                MaxTax = 10,
                MaxPrice = 10
            };
            var result = await this._service.Create(input);
            result.Id.ShouldBeGreaterThan(0);
            result.IsActive.ShouldBeTrue();
            result.LogisticChannelId.ShouldBe(lc.Id);
            result.LogisticName.ShouldBe(lc.LogisticBy.CorporationName);
            result.LogisticChannelName.ShouldBe(lc.ChannelName);
            result.RuleName.ShouldBe(input.RuleName);
            result.MaxPackage.ShouldBe(input.MaxPackage);
            result.MaxWeight.ShouldBe(input.MaxWeight);
            result.MaxTax.ShouldBe(input.MaxTax);
            result.MaxPrice.ShouldBe(input.MaxPrice);
            result.TenantId.ShouldNotBeNull();
            await this._context.UsingDbContextAsync(async context =>
            {
                var sr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == result.Id);
                sr.ShouldNotBeNull();
                sr.RuleName.ShouldBe(result.RuleName);
                sr.MaxPackage.ShouldBe(result.MaxPackage);
                sr.MaxWeight.ShouldBe(result.MaxWeight);
                sr.MaxTax.ShouldBe(result.MaxTax);
                sr.MaxPrice.ShouldBe(result.MaxPrice);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == lc.LogisticId);
                l.ShouldNotBeNull();
                var lC = l.LogisticChannels.FirstOrDefault(o => o.Id == lc.Id);
                lC.ShouldNotBeNull();
                var sr = lC.SplitRules.FirstOrDefault(o => o.Id == result.Id);
                sr.ShouldNotBeNull();
                sr.Id.ShouldBe(result.Id);
                sr.RuleName.ShouldBe(result.RuleName);
                sr.MaxPackage.ShouldBe(result.MaxPackage);
                sr.MaxWeight.ShouldBe(result.MaxWeight);
                sr.MaxTax.ShouldBe(result.MaxTax);
                sr.MaxPrice.ShouldBe(result.MaxPrice);
                sr.ProductClasses.ShouldBeNull();
                await Task.FromResult(1);
            });
            return result.Id;
        }

        protected async Task Update_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().IgnoreQueryFilters()
                    .Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            sr.ShouldNotBeNull();
            var input = new UpdateSplitRuleDto()
            {
                Id = sr.Id,
                LogisticChannelId = sr.LogisticChannelId,
                RuleName = "规则命修改",
                MaxPackage = sr.MaxPackage + 10,
                MaxWeight = sr.MaxWeight + 10,
                MaxTax = sr.MaxTax + 10,
                MaxPrice = sr.MaxPrice + 10
            };
            var result = await this._service.Update(input);
            result.Id.ShouldBeGreaterThan(0);
            result.IsActive.ShouldBeTrue();
            result.LogisticChannelId.ShouldBe(sr.LogisticChannelId);
            result.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
            result.LogisticChannelName.ShouldBe(sr.LogisticChannelBy.ChannelName);
            result.RuleName.ShouldBe(input.RuleName);
            result.MaxPackage.ShouldBe(input.MaxPackage);
            result.MaxWeight.ShouldBe(input.MaxWeight);
            result.MaxTax.ShouldBe(input.MaxTax);
            result.MaxPrice.ShouldBe(input.MaxPrice);
            result.TenantId.ShouldNotBeNull();
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == result.Id);
                newsr.ShouldNotBeNull();
                newsr.RuleName.ShouldBe(result.RuleName);
                newsr.MaxPackage.ShouldBe(result.MaxPackage);
                newsr.MaxWeight.ShouldBe(result.MaxWeight);
                newsr.MaxTax.ShouldBe(result.MaxTax);
                newsr.MaxPrice.ShouldBe(result.MaxPrice);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == result.Id);
                sR.ShouldNotBeNull();
                sR.Id.ShouldBe(result.Id);
                sR.RuleName.ShouldBe(result.RuleName);
                sR.MaxPackage.ShouldBe(result.MaxPackage);
                sR.MaxWeight.ShouldBe(result.MaxWeight);
                sR.MaxTax.ShouldBe(result.MaxTax);
                sR.MaxPrice.ShouldBe(result.MaxPrice);
                await Task.FromResult(1);
            });
        }

        protected async Task Switch_Stop_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().IgnoreQueryFilters()
                    .Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            await this._service.Switch(id, false);
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == id);
                newsr.ShouldNotBeNull();
                newsr.IsActive.ShouldBeFalse();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldBeNull();
                await Task.FromResult(1);
            });
        }

        protected async Task Switch_Enable_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().IgnoreQueryFilters()
                    .Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            await this._service.Switch(id, true);
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == id);
                newsr.ShouldNotBeNull();
                newsr.IsActive.ShouldBeTrue();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldNotBeNull();
                sR.Id.ShouldBe(sr.Id);
                sR.RuleName.ShouldBe(sr.RuleName);
                sR.MaxPackage.ShouldBe(sr.MaxPackage);
                sR.MaxWeight.ShouldBe(sr.MaxWeight);
                sR.MaxTax.ShouldBe(sr.MaxTax);
                sR.MaxPrice.ShouldBe(sr.MaxPrice);
                await Task.FromResult(1);
            });
        }

        protected async Task Delete_Test(long id)
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().IgnoreQueryFilters()
                    .Include(o => o.LogisticChannelBy).ThenInclude(o => o.LogisticBy).FirstAsync(o => o.Id == id);
            }
            sr.ShouldNotBeNull();
            await this._service.Delete(new Abp.Application.Services.Dto.EntityDto<long>(id));
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRules.FirstOrDefaultAsync(u => u.Id == sr.Id);
                newsr.ShouldBeNull();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldBeNull();
                await Task.FromResult(1);
            });
        }

        [Fact]
        public async Task Get()
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                var tlc = await this._context.ResolveService<IRepository<TenantLogisticChannel, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p=>p.LogisticChannelBy).ThenInclude(p=>p.SplitRules)
                    .Include(p=>p.LogisticChannelBy).ThenInclude(p=>p.LogisticBy)
                    .FirstAsync(o=>o.TenantId == this._context.GetTenantId());
                sr = tlc.LogisticChannelBy.SplitRules.First();
            }
            sr.ShouldNotBeNull();
            var result = await this._service.Get(new Abp.Application.Services.Dto.EntityDto<long>(sr.Id));
            result.IsActive.ShouldBeTrue();
            result.LogisticChannelId.ShouldBe(sr.LogisticChannelId);
            result.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
            result.LogisticChannelName.ShouldBe(sr.LogisticChannelBy.ChannelName);
            result.RuleName.ShouldBe(sr.RuleName);
            result.MaxPackage.ShouldBe(sr.MaxPackage);
            result.MaxWeight.ShouldBe(sr.MaxWeight);
            result.MaxTax.ShouldBe(sr.MaxTax);
            result.MaxPrice.ShouldBe(sr.MaxPrice);
            result.TenantId.ShouldBeNull();
        }

        [Fact]
        public async Task GetAll()
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                var tlc = await this._context.ResolveService<IRepository<TenantLogisticChannel, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.SplitRules).ThenInclude(p=>p.ProductClasses)
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy)
                    .FirstAsync(o => o.TenantId == this._context.GetTenantId());
                sr = tlc.LogisticChannelBy.SplitRules.First();
            }
            var filter = new SplitRuleSearchFilter()
            {
                LogisticName = sr.LogisticChannelBy.LogisticBy.CorporationName
            };
            var result = await this._service.GetAll(filter);
            result.Items.Count.ShouldBeGreaterThan(0);
            foreach (var item in result.Items)
            {
                (item.TenantId == null || item.TenantId == this._context.GetTenantId() ? true : false).ShouldBeTrue();
                item.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
            }
            filter.ChannelName = sr.LogisticChannelBy.ChannelName;
            result = await this._service.GetAll(filter);
            result.Items.Count.ShouldBeGreaterThan(0);
            foreach (var item in result.Items)
            {
                (item.TenantId == null || item.TenantId == this._context.GetTenantId() ? true : false).ShouldBeTrue();
                item.LogisticName.ShouldBe(sr.LogisticChannelBy.LogisticBy.CorporationName);
                item.LogisticChannelName.ShouldBe(sr.LogisticChannelBy.ChannelName);
                item.LogisticChannelId.ShouldBe(sr.LogisticChannelId);
            }
            filter.PTId = sr.ProductClasses.Where(o => o.Type == RuleItemStintType.PTId).First().StintMark;
            result = await this._service.GetAll(filter);
            result.Items.Any(o => o.Id == sr.Id && (o.TenantId == null || o.TenantId == this._context.GetTenantId())).ShouldBeTrue();
        }
    }
}
