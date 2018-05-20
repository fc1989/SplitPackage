using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq;
using Abp.Linq.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SplitPackage.Authorization;
using SplitPackage.Business.Dto;
using SplitPackage.Business.LogisticChannels.Dto;
using SplitPackage.Business.NumFreights.Dto;
using SplitPackage.Business.WeightFreights.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.LogisticChannels
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_LogisticChannels)]
    public class LogisticChannelAppService : ApplicationService
    {
        private readonly IRepository<LogisticChannel, long> _lcRepository;
        private readonly IRepository<WeightFreight, long> _wfRepository;
        private readonly IRepository<NumFreight, long> _nfRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tlcRepository;
        private readonly IRepository<Logistic, long> _logisticRepository;

        protected virtual string GetPermissionName { get; set; }
        protected virtual string GetAllPermissionName { get; set; }
        protected virtual string CreatePermissionName { get; set; }
        protected virtual string UpdatePermissionName { get; set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public LogisticChannelAppService(IRepository<LogisticChannel, long> lcRepository,
            IRepository<WeightFreight, long> wfRepository,
            IRepository<NumFreight, long> nfRepository,
            IRepository<TenantLogisticChannel, long> tlcRepository,
            IRepository<Logistic, long> logisticRepository)
        {
            this.LocalizationSourceName = SplitPackageConsts.LocalizationSourceName;
            this._wfRepository = wfRepository;
            this._nfRepository = nfRepository;
            this._tlcRepository = tlcRepository;
            this._lcRepository = lcRepository;
            this._logisticRepository = logisticRepository;
            this.AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        protected virtual void CheckPermission(string permissionName)
        {
            if (!string.IsNullOrEmpty(permissionName))
            {
                this.PermissionChecker.Authorize(permissionName);
            }
        }

        public async Task<LogisticChannelDto> Get(EntityDto<long> input)
        {
            CheckPermission(GetPermissionName);

            var tenantId = AbpSession.TenantId;
            var entity = await this._lcRepository.GetAll().IgnoreQueryFilters().Include(p=>p.LogisticBy).FirstOrDefaultAsync(o => o.Id == input.Id && !o.IsDeleted);
            if (entity == null)
            {
                return null;
            }
            var result = ObjectMapper.Map<LogisticChannelDto>(entity);
            result.LogisticName = entity.LogisticBy.CorporationName;
            Func<LogisticChannelDto, Task> setChargeAction = async o =>
             {
                 if (o.Way == ChargeWay.ChargeByNum)
                 {
                     var chargeRules = await this._nfRepository.GetAllListAsync(f => f.LogisticChannelId == o.Id);
                     result.NumChargeRules = chargeRules.Select(rule => ObjectMapper.Map<NumFreightDto>(rule));
                 }
                 else if (o.Way == ChargeWay.ChargeByWeight)
                 {
                     var chargeRules = await this._wfRepository.GetAllListAsync(f => f.LogisticChannelId == o.Id);
                     result.WeightChargeRules = chargeRules.Select(rule => ObjectMapper.Map<WeightFreightDto>(rule));
                 }
             };
            if (entity.TenantId != tenantId)
            {
                var customerChange = await this._tlcRepository.FirstOrDefaultAsync(o => o.TenantId == tenantId && o.LogisticChannelId == input.Id);
                if (!string.IsNullOrEmpty(customerChange.AliasName))
                {
                    result.AliasName = customerChange.AliasName;
                }
                if (customerChange.Way.HasValue)
                {
                    result.Way = customerChange.Way.Value;
                }
                if (!string.IsNullOrEmpty(customerChange.LogisticChannelChange))
                {
                    var replaceCharge = JsonConvert.DeserializeObject<ChangeInformation>(customerChange.LogisticChannelChange);
                    switch (result.Way)
                    {
                        case ChargeWay.ChargeByWeight:
                            result.WeightChargeRules = replaceCharge.WeightChargeRules.Select(o => ObjectMapper.Map<WeightFreightDto>(o));
                            break;
                        case ChargeWay.ChargeByNum:
                            result.NumChargeRules = replaceCharge.NumChargeRules.Select(o => ObjectMapper.Map<NumFreightDto>(o));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    await setChargeAction(result);
                }
            }
            else
            {
                await setChargeAction(result);
            }
            return result;
        }

        public async Task<LogisticChannelDto> Create(CreateLogisticChannelDto input)
        {
            CheckPermission(CreatePermissionName);

            var entity = ObjectMapper.Map<LogisticChannel>(input);
            entity.TenantId = AbpSession.TenantId;
            var channelId = await _lcRepository.InsertAndGetIdAsync(entity);
            if (input.Way == ChargeWay.ChargeByNum)
            {
                var array = input.NumChargeRules.Select(o =>
                {
                    var result = ObjectMapper.Map<NumFreight>(o);
                    result.LogisticChannelId = channelId;
                    result.IsActive = true;
                    return result;
                });
                foreach (var item in array)
                {
                    await this._nfRepository.InsertAsync(item);
                }
            }
            else if (input.Way == ChargeWay.ChargeByWeight)
            {
                var array = input.WeightChargeRules.Select(o =>
                {
                    var result = ObjectMapper.Map<WeightFreight>(o);
                    result.LogisticChannelId = channelId;
                    result.IsActive = true;
                    return result;
                });
                foreach (var item in array)
                {
                    await this._wfRepository.InsertAsync(item);
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<LogisticChannelDto>(entity);
        }

        Func<WeightFreight, WeightFreightDto, bool> judgeWeightChange = (wf, wfdto) =>
        {
            bool result = false;
            if (wf == null && wfdto != null)
                return true;
            if (wf.Currency != wfdto.Currency)
                result = true;
            if (wf.Unit != wfdto.Currency)
                result = true;
            if (wf.StartingWeight != wfdto.StartingWeight)
                result = true;
            if (wf.EndWeight != wfdto.EndWeight)
                result = true;
            if (wf.StartingPrice != wfdto.StartingPrice)
                result = true;
            if (wf.StepWeight != wfdto.StepWeight)
                result = true;
            if (wf.CostPrice != wfdto.CostPrice)
                result = true;
            if (wf.Price != wfdto.Price)
                result = true;
            return result;
        };

        Func<NumFreight, NumFreightDto, bool> judgeNumChange = (nf, nfdto) =>
        {
            bool result = false;
            if (nf == null && nfdto != null)
                return true;
            if (nf.Currency != nfdto.Currency)
                result = true;
            if (nf.Unit != nfdto.Unit)
                result = true;
            if (nf.SplitNum != nfdto.SplitNum)
                result = true;
            if (nf.FirstPrice != nfdto.FirstPrice)
                result = true;
            if (nf.CarryOnPrice != nfdto.CarryOnPrice)
                result = true;
            return result;
        };

        public async Task<LogisticChannelDto> Update(UpdateLogisticChannelDto input)
        {
            CheckPermission(UpdatePermissionName);

            var tenantId = AbpSession.TenantId;
            var entity = await this._lcRepository.GetAll().IgnoreQueryFilters().SingleAsync(o => o.Id == input.Id && !o.IsDeleted);
            if (entity.TenantId == tenantId)
            {
                ObjectMapper.Map(input, entity);
                switch (entity.Way)
                {
                    case ChargeWay.ChargeByWeight:
                        await this._wfRepository.DeleteAsync(o => o.LogisticChannelId == entity.Id);
                        var addWeightFreight = input.WeightChargeRules.Select(o => ObjectMapper.Map<WeightFreight>(o));
                        foreach (var item in addWeightFreight)
                        {
                            item.LogisticChannelId = entity.Id;
                            item.Id = 0;
                            await this._wfRepository.InsertAsync(item);
                        }
                        break;
                    case ChargeWay.ChargeByNum:
                        await this._nfRepository.DeleteAsync(o => o.LogisticChannelId == entity.Id);
                        var addNumFreight = input.NumChargeRules.Select(o => ObjectMapper.Map<NumFreight>(o));
                        foreach (var item in addNumFreight)
                        {
                            item.LogisticChannelId = entity.Id;
                            item.Id = 0;
                            await this._nfRepository.InsertAsync(item);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                await this._tlcRepository.DeleteAsync(o => o.LogisticChannelId == entity.Id && o.TenantId == tenantId);
                bool weightChange = false;
                bool numChange = false;
                switch (input.Way)
                {
                    case ChargeWay.ChargeByWeight:
                        var weightRule = await this._wfRepository.FirstOrDefaultAsync(o => o.LogisticChannelId == entity.Id);
                        var weightRuleDto = input.WeightChargeRules.FirstOrDefault();
                        weightChange = judgeWeightChange(weightRule, weightRuleDto);
                        break;
                    case ChargeWay.ChargeByNum:
                        var numRule = await this._nfRepository.FirstOrDefaultAsync(o => o.LogisticChannelId == entity.Id);
                        var numRuleDto = input.NumChargeRules.FirstOrDefault();
                        numChange = judgeNumChange(numRule, numRuleDto);
                        break;
                    default:
                        break;
                }
                await this._tlcRepository.InsertAsync(new TenantLogisticChannel()
                {
                    TenantId = tenantId.Value,
                    LogisticChannelId = entity.Id,
                    AliasName = string.Equals(entity.AliasName, input.AliasName) ? string.Empty : input.AliasName,
                    Way = entity.Way.Equals(input.Way) ? null : (ChargeWay?)input.Way,
                    LogisticChannelChange = numChange || weightChange ? JsonConvert.SerializeObject(new ChangeInformation()
                    {
                        WeightChargeRules = weightChange ? input.WeightChargeRules.Select(o => ObjectMapper.Map<WeightFreight>(o)) : null,
                        NumChargeRules = numChange ? input.NumChargeRules.Select(o => ObjectMapper.Map<NumFreight>(o)) : null
                    }) : string.Empty
                });
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<LogisticChannelDto>(entity);
        }

        public async Task<PagedResultDto<LogisticChannelDto>> GetAll(LogisticChannelSearchFilter input)
        {
            CheckPermission(GetAllPermissionName);

            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                var query = this._lcRepository.GetAll().Where(input.GenerateFilter());
                var totalCount = await this.AsyncQueryableExecuter.CountAsync(query);
                query = query.PageBy(input);
                var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p => p.LogisticBy));
                return new PagedResultDto<LogisticChannelDto>(
                    totalCount,
                    entities.Select(ObjectMapper.Map<LogisticChannelDto>).ToList()
                );
            }
            else
            {
                var query = from l in this._logisticRepository.GetAll().IgnoreQueryFilters()
                            join lc in this._lcRepository.GetAll().IgnoreQueryFilters() on l.Id equals lc.LogisticId
                            join tlc in this._tlcRepository.GetAll() on lc.Id equals tlc.LogisticChannelId into tlc1
                            from tlcleft in tlc1.DefaultIfEmpty()
                            where (lc.TenantId == tenantId.Value || tlcleft.TenantId == tenantId.Value) && lc.LogisticId == input.LogisticId
                            select new LogisticChannelDto()
                            {
                                Id = lc.Id,
                                ChannelName = lc.ChannelName,
                                LogisticId = lc.LogisticId,
                                LogisticName = l.CorporationName,
                                AliasName = tlcleft.AliasName == null ? lc.AliasName : tlcleft.AliasName,
                                Type = lc.Type,
                                Way = tlcleft.Way == null ? lc.Way : tlcleft.Way.Value,
                                IsActive = lc.IsActive,
                                TenantId = lc.TenantId
                            };
                var totalCount = await this.AsyncQueryableExecuter.CountAsync(query);
                query = query.PageBy(input);
                var entities = await AsyncQueryableExecuter.ToListAsync(query);
                return new PagedResultDto<LogisticChannelDto>(
                    totalCount,
                    entities
                );
            }
        }

        public async Task<bool> CustomerImport(List<long> importIds)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                return false;
            }
            var imported = this._tlcRepository.GetAll().Where(o => o.TenantId == tenantId.Value).Select(o=>o.LogisticChannelId).ToList();
            var addSet = importIds.Where(o => !imported.Contains(o)).ToList();
            foreach (var item in addSet)
            {
                await this._tlcRepository.InsertAsync(new TenantLogisticChannel() {
                    TenantId = tenantId.Value,
                    LogisticChannelId = item
                });
            }
            var deleteSet = imported.Where(o => !importIds.Contains(o)).ToList();
            foreach (var item in deleteSet)
            {
                await this._tlcRepository.DeleteAsync(o => o.TenantId == tenantId && deleteSet.Contains(o.LogisticChannelId));
            }
            return true;
        }

        public async Task<bool> Verify(VerifyChannelDto input)
        {
            var count = await this._lcRepository.GetAll().IgnoreQueryFilters().Where(o => o.LogisticId == input.LogisticId && !o.IsDeleted && o.ChannelName == input.ChannelName).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<ImportStateDto> GetImportState()
        {
            ImportStateDto result = null;
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                return await Task.FromResult<ImportStateDto>(result);
            }
            result = new ImportStateDto()
            {
                SystemLogisticChannel = this._lcRepository.GetAll().Include(p => p.LogisticBy).IgnoreQueryFilters()
                    .Where(o => !o.IsDeleted && !o.TenantId.HasValue)
                    .Select(o => ObjectMapper.Map<LogisticChannelDto>(o)).ToList(),
                ImportLogisticChannel = this._tlcRepository.GetAllList(o => o.TenantId == tenantId).Select(o => o.LogisticChannelId).ToList()
            };
            return await Task.FromResult(result);
        }

        public async Task<List<Option>> GetOptional()
        {
            var tenantId = AbpSession.TenantId;
            var query = from lc in this._lcRepository.GetAll().IgnoreQueryFilters()
                        join tlc in this._tlcRepository.GetAll() on lc.Id equals tlc.LogisticChannelId into tlc1
                        from tlcleft in tlc1.DefaultIfEmpty()
                        where lc.TenantId == tenantId || tlcleft.TenantId == tenantId
                        select lc;
            query.Include(p => p.LogisticBy);
            return await Task.FromResult(query.GroupBy(o => o.LogisticBy).Select(o => new Option()
            {
                Value = o.Key.Id.ToString(),
                label = o.Key.CorporationName,
                Children = o.Select(oi => new Option()
                {
                    Value = oi.Id.ToString(),
                    label = oi.ChannelName,
                }).ToList()
            }).ToList());
        }
    }
}
