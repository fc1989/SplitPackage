using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using SplitPackage.Business;
using SplitPackage.Business.SplitRules;
using SplitPackage.Tests.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Linq;
using SplitPackage.Business.SplitRules.Dto;

namespace SplitPackage.Tests.Business
{
    [Collection("admin collection")]
    public class SplitRuleItemAppService_Admin_Tests
    {
        private readonly SplitRuleItemAppService _service;
        private readonly AdminCase _context;

        public SplitRuleItemAppService_Admin_Tests(Xunit.Abstractions.ITestOutputHelper output, AdminCase context)
        {
            this._context = context;
            this._service = context.ResolveService<SplitRuleItemAppService>();
        }

        [Fact]
        public async Task Add_Update_Delete_Test()
        {
            long id = await this.Create_Test();
            await this.Update_Test(id);
            await this.Delete_Test(id);
        }

        protected async Task<long> Create_Test()
        {
            SplitRule sr;
            ProductClass pc;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll().Include(p=>p.LogisticChannelBy).ThenInclude(p=>p.LogisticBy).FirstAsync();
                pc = await this._context.ResolveService<IRepository<ProductClass, long>>().GetAll().FirstAsync();
            }
            var input = new SplitPackage.Business.SplitRules.Dto.CreateRuleItemDto()
            {
                SplitRuleId = sr.Id,
                StintMark = pc.PTId,
                MaxNum = 10,
                MinNum = 1,
                Type = RuleItemStintType.PTId
            };
            var result = await this._service.Create(input);
            result.Id.ShouldBeGreaterThan(0);
            result.RuleName.ShouldBe(sr.RuleName);
            result.StintMark.ShouldBe(input.StintMark);
            result.ProductClass.ShouldBe(string.Format("{0}({1})",pc.ClassName,pc.PTId));
            result.MaxNum.ShouldBe(input.MaxNum);
            result.MinNum.ShouldBe(input.MinNum);
            result.TenantId.ShouldBeNull();
            result.Type.ShouldBe(input.Type);
            await this._context.UsingDbContextAsync(async context =>
            {
                var srp = await context.SplitRuleProductClass.FirstOrDefaultAsync(u => u.Id == result.Id);
                srp.ShouldNotBeNull();
                srp.StintMark.ShouldBe(input.StintMark);
                srp.MaxNum.ShouldBe(input.MaxNum);
                srp.MinNum.ShouldBe(input.MinNum);
                srp.TenantId.ShouldBeNull();
                srp.Type.ShouldBe(input.Type);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldNotBeNull();
                var srp = sR.ProductClasses.FirstOrDefault(o => o.Id == result.Id);
                srp.ShouldNotBeNull();
                srp.StintMark.ShouldBe(input.StintMark);
                srp.MaxNum.ShouldBe(input.MaxNum);
                srp.MinNum.ShouldBe(input.MinNum);
                srp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                if (lc == null)
                    return;
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldNotBeNull();
                var srp = sR.ProductClasses.FirstOrDefault(o=>o.Id == result.Id);
                srp.ShouldNotBeNull();
                srp.StintMark.ShouldBe(input.StintMark);
                srp.MaxNum.ShouldBe(input.MaxNum);
                srp.MinNum.ShouldBe(input.MinNum);
                srp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
            return result.Id;
        }

        protected async Task Update_Test(long id)
        {
            SplitRuleItem srp;
            ProductClass pc;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                srp = await this._context.ResolveService<IRepository<SplitRuleItem, long>>().GetAll()
                    .Include(p=>p.SplitRuleBy).ThenInclude(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o=>o.Id == id);
                pc = await this._context.ResolveService<IRepository<ProductClass, long>>().GetAll().Skip(1).FirstAsync();
            }
            var input = new SplitPackage.Business.SplitRules.Dto.UpdateRuleItemDto()
            {
                Id = id,
                StintMark = pc.PTId,
                MaxNum = srp.MaxNum + 10,
                MinNum = 1,
                Type = RuleItemStintType.PTId
            };
            var result = await this._service.Update(input);
            result.Id.ShouldBeGreaterThan(0);
            result.RuleName.ShouldBe(srp.SplitRuleBy.RuleName);
            result.StintMark.ShouldBe(input.StintMark);
            result.ProductClass.ShouldBe(string.Format("{0}({1})", pc.ClassName, pc.PTId));
            result.MaxNum.ShouldBe(input.MaxNum);
            result.MinNum.ShouldBe(input.MinNum);
            result.TenantId.ShouldBeNull();
            result.Type.ShouldBe(input.Type);
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsrp = await context.SplitRuleProductClass.FirstOrDefaultAsync(u => u.Id == result.Id);
                newsrp.ShouldNotBeNull();
                newsrp.StintMark.ShouldBe(input.StintMark);
                newsrp.MaxNum.ShouldBe(input.MaxNum);
                newsrp.MinNum.ShouldBe(input.MinNum);
                newsrp.TenantId.ShouldBeNull();
                newsrp.Type.ShouldBe(input.Type);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == srp.SplitRuleId);
                sR.ShouldNotBeNull();
                var newsrp = sR.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldNotBeNull();
                newsrp.StintMark.ShouldBe(input.StintMark);
                newsrp.MaxNum.ShouldBe(input.MaxNum);
                newsrp.MinNum.ShouldBe(input.MinNum);
                newsrp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                if (lc == null)
                    return;
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == srp.SplitRuleId);
                sR.ShouldNotBeNull();
                var newsrp = sR.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldNotBeNull();
                newsrp.StintMark.ShouldBe(input.StintMark);
                newsrp.MaxNum.ShouldBe(input.MaxNum);
                newsrp.MinNum.ShouldBe(input.MinNum);
                newsrp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
        }

        protected async Task Delete_Test(long id)
        {
            SplitRuleItem srp = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                srp = await this._context.ResolveService<IRepository<SplitRuleItem, long>>().GetAll()
                    .Include(p=>p.SplitRuleBy).ThenInclude(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o => o.Id == id);
            }
            srp.ShouldNotBeNull();
            await this._service.Delete(new Abp.Application.Services.Dto.EntityDto<long>(id));
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRuleProductClass.FirstOrDefaultAsync(u => u.Id == srp.Id);
                newsr.ShouldBeNull();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sr = lc.SplitRules.FirstOrDefault(o => o.Id == srp.SplitRuleId);
                sr.ShouldNotBeNull();
                var newsrp = sr.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldBeNull();
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                if (lc == null)
                    return;
                var sr = lc.SplitRules.FirstOrDefault(o => o.Id == srp.Id);
                if (sr == null)
                    return;
                var newsrp = sr.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldBeNull();
                await Task.FromResult(1);
            });
        }

        [Fact]
        public async Task Get()
        {
            SplitRuleItem srp = null;
            ProductClass pc = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                srp = await this._context.ResolveService<IRepository<SplitRuleItem, long>>().GetAll()
                    .Include(p=>p.SplitRuleBy).FirstAsync();
                pc = await this._context.ResolveService<IRepository<ProductClass, long>>().GetAll()
                    .FirstAsync(o => o.PTId.Equals(srp.StintMark));
            }
            srp.ShouldNotBeNull();
            var result = await this._service.Get(new Abp.Application.Services.Dto.EntityDto<long>(srp.Id));
            result.RuleName.ShouldBe(srp.SplitRuleBy.RuleName);
            result.StintMark.ShouldBe(srp.StintMark);
            result.ProductClass.ShouldBe(string.Format("{0}({1})",pc.ClassName,pc.PTId));
            result.MaxNum.ShouldBe(srp.MaxNum);
            result.MinNum.ShouldBe(srp.MinNum);
            result.TenantId.ShouldBeNull();
            result.Type.ShouldBe(srp.Type);
        }

        [Fact]
        public async Task GetAll()
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                sr = await this._context.ResolveService<IRepository<SplitRule, long>>().GetAll()
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).Include(p => p.ProductClasses).FirstAsync();
            }
            var filter = new SplitRuleItemFilter()
            {
                SplitRuleId = sr.Id
            };
            var result = await this._service.GetAll(filter);
            result.Items.Count.ShouldBeGreaterThan(0);
            foreach (var item in result.Items)
            {
                item.TenantId.ShouldBeNull();
                item.RuleName.ShouldBe(sr.RuleName);
                item.ProductClass.ShouldNotBeNullOrEmpty();
            }
        }
    }

    [Collection("Assistant collection")]
    public class SplitRuleItemAppService_Tenant_Tests
    {
        private readonly SplitRuleItemAppService _service;
        private readonly AssistantCase _context;

        public SplitRuleItemAppService_Tenant_Tests(Xunit.Abstractions.ITestOutputHelper output, AssistantCase context)
        {
            this._context = context;
            this._service = context.ResolveService<SplitRuleItemAppService>();
        }

        [Fact]
        public async Task Add_Update_Delete_Test()
        {
            long id = await this.Create_Test();
            await this.Update_Test(id);
            await this.Delete_Test(id);
        }

        protected async Task<long> Create_Test()
        {
            SplitRule sr;
            ProductClass pc;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                var tlc = await this._context.ResolveService<IRepository<TenantLogisticChannel, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p=>p.LogisticChannelBy).ThenInclude(p=>p.SplitRules).ThenInclude(p=>p.ProductClasses)
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o => o.TenantId == this._context.GetTenantId());
                sr = tlc.LogisticChannelBy.SplitRules.First();
                pc = await this._context.ResolveService<IRepository<ProductClass, long>>().GetAll().FirstAsync();
            }
            var input = new SplitPackage.Business.SplitRules.Dto.CreateRuleItemDto()
            {
                SplitRuleId = sr.Id,
                StintMark = pc.PTId,
                MaxNum = 10,
                MinNum = 1,
                Type = RuleItemStintType.PTId
            };
            var result = await this._service.Create(input);
            result.Id.ShouldBeGreaterThan(0);
            result.RuleName.ShouldBe(sr.RuleName);
            result.StintMark.ShouldBe(input.StintMark);
            result.ProductClass.ShouldBe(string.Format("{0}({1})", pc.ClassName, pc.PTId));
            result.MaxNum.ShouldBe(input.MaxNum);
            result.MinNum.ShouldBe(input.MinNum);
            result.TenantId.ShouldNotBeNull();
            result.Type.ShouldBe(input.Type);
            await this._context.UsingDbContextAsync(async context =>
            {
                var srp = await context.SplitRuleProductClass.FirstOrDefaultAsync(u => u.Id == result.Id);
                srp.ShouldNotBeNull();
                srp.StintMark.ShouldBe(input.StintMark);
                srp.MaxNum.ShouldBe(input.MaxNum);
                srp.MinNum.ShouldBe(input.MinNum);
                srp.TenantId.ShouldNotBeNull();
                srp.Type.ShouldBe(input.Type);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == sr.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == sr.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == sr.Id);
                sR.ShouldNotBeNull();
                var srp = sR.ProductClasses.FirstOrDefault(o => o.Id == result.Id);
                srp.ShouldNotBeNull();
                srp.StintMark.ShouldBe(input.StintMark);
                srp.MaxNum.ShouldBe(input.MaxNum);
                srp.MinNum.ShouldBe(input.MinNum);
                srp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
            return result.Id;
        }

        protected async Task Update_Test(long id)
        {
            SplitRuleItem srp;
            ProductClass pc;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                srp = await this._context.ResolveService<IRepository<SplitRuleItem, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p => p.SplitRuleBy).ThenInclude(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o => o.Id == id);
                pc = await this._context.ResolveService<IRepository<ProductClass, long>>().GetAll().Skip(1).FirstAsync();
            }
            var input = new SplitPackage.Business.SplitRules.Dto.UpdateRuleItemDto()
            {
                Id = id,
                StintMark = pc.PTId,
                MaxNum = srp.MaxNum + 10,
                MinNum = 1,
                Type = RuleItemStintType.PTId
            };
            var result = await this._service.Update(input);
            result.Id.ShouldBeGreaterThan(0);
            result.RuleName.ShouldBe(srp.SplitRuleBy.RuleName);
            result.StintMark.ShouldBe(input.StintMark);
            result.ProductClass.ShouldBe(string.Format("{0}({1})", pc.ClassName, pc.PTId));
            result.MaxNum.ShouldBe(input.MaxNum);
            result.MinNum.ShouldBe(input.MinNum);
            result.TenantId.ShouldNotBeNull();
            result.Type.ShouldBe(input.Type);
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsrp = await context.SplitRuleProductClass.FirstOrDefaultAsync(u => u.Id == result.Id);
                newsrp.ShouldNotBeNull();
                newsrp.StintMark.ShouldBe(input.StintMark);
                newsrp.MaxNum.ShouldBe(input.MaxNum);
                newsrp.MinNum.ShouldBe(input.MinNum);
                newsrp.TenantId.ShouldNotBeNull();
                newsrp.Type.ShouldBe(input.Type);
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == srp.SplitRuleId);
                sR.ShouldNotBeNull();
                var newsrp = sR.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldNotBeNull();
                newsrp.StintMark.ShouldBe(input.StintMark);
                newsrp.MaxNum.ShouldBe(input.MaxNum);
                newsrp.MinNum.ShouldBe(input.MinNum);
                newsrp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                if (lc == null)
                    return;
                var sR = lc.SplitRules.FirstOrDefault(o => o.Id == srp.SplitRuleId);
                sR.ShouldNotBeNull();
                var newsrp = sR.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldNotBeNull();
                newsrp.StintMark.ShouldBe(input.StintMark);
                newsrp.MaxNum.ShouldBe(input.MaxNum);
                newsrp.MinNum.ShouldBe(input.MinNum);
                newsrp.Type.ShouldBe(input.Type);
                await Task.FromResult(1);
            });
        }

        protected async Task Delete_Test(long id)
        {
            SplitRuleItem srp = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                srp = await this._context.ResolveService<IRepository<SplitRuleItem, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p => p.SplitRuleBy).ThenInclude(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o => o.Id == id);
            }
            srp.ShouldNotBeNull();
            await this._service.Delete(new Abp.Application.Services.Dto.EntityDto<long>(id));
            await this._context.UsingDbContextAsync(async context =>
            {
                var newsr = await context.SplitRuleProductClass.FirstOrDefaultAsync(u => u.Id == srp.Id);
                newsr.ShouldBeNull();
            });
            await this._context.UsingCurrentSettingCacheAsync(async setting => {
                setting.ShouldNotBeNull();
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                l.ShouldNotBeNull();
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                lc.ShouldNotBeNull();
                var sr = lc.SplitRules.FirstOrDefault(o => o.Id == srp.SplitRuleId);
                sr.ShouldNotBeNull();
                var newsrp = sr.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldBeNull();
                await Task.FromResult(1);
            });
            await this._context.UsingTenantSettingCacheAsync(async setting => {
                if (setting == null)
                    return;
                var l = setting.OwnLogistics.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelBy.LogisticId);
                if (l == null)
                    return;
                var lc = l.LogisticChannels.FirstOrDefault(o => o.Id == srp.SplitRuleBy.LogisticChannelId);
                if (lc == null)
                    return;
                var sr = lc.SplitRules.FirstOrDefault(o => o.Id == srp.Id);
                if (sr == null)
                    return;
                var newsrp = sr.ProductClasses.FirstOrDefault(o => o.Id == srp.Id);
                newsrp.ShouldBeNull();
                await Task.FromResult(1);
            });
        }

        [Fact]
        public async Task Get()
        {
            SplitRuleItem srp = null;
            ProductClass pc = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                var tlc = await this._context.ResolveService<IRepository<TenantLogisticChannel, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.SplitRules).ThenInclude(p => p.ProductClasses)
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o => o.TenantId == this._context.GetTenantId());
                srp = tlc.LogisticChannelBy.SplitRules.First().ProductClasses.First();
                pc = await this._context.ResolveService<IRepository<ProductClass, long>>().GetAll()
                    .FirstAsync(o => o.PTId.Equals(srp.StintMark));
            }
            srp.ShouldNotBeNull();
            var result = await this._service.Get(new Abp.Application.Services.Dto.EntityDto<long>(srp.Id));
            result.RuleName.ShouldBe(srp.SplitRuleBy.RuleName);
            result.StintMark.ShouldBe(srp.StintMark);
            result.ProductClass.ShouldBe(string.Format("{0}({1})", pc.ClassName, pc.PTId));
            result.MaxNum.ShouldBe(srp.MaxNum);
            result.MinNum.ShouldBe(srp.MinNum);
            result.Type.ShouldBe(srp.Type);
            result.TenantId.ShouldBe(srp.TenantId);
        }

        [Fact]
        public async Task GetAll()
        {
            SplitRule sr = null;
            using (var unitOfWork = this._context.ResolveService<IUnitOfWorkManager>().Begin())
            {
                var tlc = await this._context.ResolveService<IRepository<TenantLogisticChannel, long>>().GetAll().IgnoreQueryFilters()
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.SplitRules).ThenInclude(p => p.ProductClasses)
                    .Include(p => p.LogisticChannelBy).ThenInclude(p => p.LogisticBy).FirstAsync(o => o.TenantId == this._context.GetTenantId());
                sr = tlc.LogisticChannelBy.SplitRules.First();
            }
            var filter = new SplitRuleItemFilter()
            {
                SplitRuleId = sr.Id
            };
            var result = await this._service.GetAll(filter);
            result.Items.Count.ShouldBeGreaterThan(0);
            foreach (var item in result.Items)
            {
                item.RuleName.ShouldBe(sr.RuleName);
                item.ProductClass.ShouldNotBeNullOrEmpty();
            }
        }
    }
}
