using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
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
using SplitPackage.Domain.Logistic;
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
        private readonly IEventBus _eventBus;

        protected virtual string GetPermissionName { get; set; }
        protected virtual string GetAllPermissionName { get; set; }
        protected virtual string CreatePermissionName { get; set; }
        protected virtual string UpdatePermissionName { get; set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public LogisticChannelAppService(IRepository<LogisticChannel, long> lcRepository,
            IRepository<WeightFreight, long> wfRepository,
            IRepository<NumFreight, long> nfRepository,
            IRepository<TenantLogisticChannel, long> tlcRepository,
            IRepository<Logistic, long> logisticRepository,
            IEventBus eventBus)
        {
            this.LocalizationSourceName = SplitPackageConsts.LocalizationSourceName;
            this._wfRepository = wfRepository;
            this._nfRepository = nfRepository;
            this._tlcRepository = tlcRepository;
            this._lcRepository = lcRepository;
            this._logisticRepository = logisticRepository;
            this.AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            this._eventBus = eventBus;
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
                     result.NumFreights = chargeRules.Select(rule => ObjectMapper.Map<NumFreightDto>(rule));
                 }
                 else if (o.Way == ChargeWay.ChargeByWeight)
                 {
                     var chargeRules = await this._wfRepository.GetAllListAsync(f => f.LogisticChannelId == o.Id);
                     result.WeightFreights = chargeRules.Select(rule => ObjectMapper.Map<WeightFreightDto>(rule));
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
                    var replaceCharge = JsonConvert.DeserializeObject<ChangeFreightRule>(customerChange.LogisticChannelChange);
                    switch (result.Way)
                    {
                        case ChargeWay.ChargeByWeight:
                            result.WeightFreights = replaceCharge.WeightChargeRules.Select(o => ObjectMapper.Map<WeightFreightDto>(o));
                            break;
                        case ChargeWay.ChargeByNum:
                            result.NumFreights = replaceCharge.NumChargeRules.Select(o => ObjectMapper.Map<NumFreightDto>(o));
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
            if (entity.WeightFreights != null)
            {
                entity.WeightFreights.ToList().ForEach(o => o.LogisticChannelBy = entity);
            }
            if (entity.NumFreights != null)
            {
                entity.NumFreights.ToList().ForEach(o => o.LogisticChannelBy = entity);
            }
            await this._lcRepository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            await this._eventBus.TriggerAsync(this.ObjectMapper.Map<CreateChannelEvent>(entity));
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
            var entity = await this._lcRepository.GetAll().Include(p=>p.WeightFreights).Include(p=>p.NumFreights).IgnoreQueryFilters()
                .AsNoTracking().SingleAsync(o => o.Id == input.Id && !o.IsDeleted);
            if (entity.TenantId == tenantId)
            {
                entity.Type = input.Type;
                if (entity.Way != input.Way)
                {
                    entity.WeightFreights.ToList().ForEach(o => this._wfRepository.Delete(o));
                    entity.NumFreights.ToList().ForEach(o => this._nfRepository.Delete(o));
                    entity.WeightFreights = new List<WeightFreight>();
                    entity.NumFreights = new List<NumFreight>();
                }
                entity.Way = input.Way;
                entity.ChannelName = input.ChannelName;
                entity.AliasName = input.AliasName;
                switch (entity.Way)
                {
                    case ChargeWay.ChargeByWeight:
                        foreach (var item in input.WeightFreights)
                        {
                            var weightRule = entity.WeightFreights.FirstOrDefault(o => o.Id == item.Id);
                            if (weightRule == null)
                            {
                                weightRule = this.ObjectMapper.Map<WeightFreight>(item);
                                weightRule.LogisticChannelBy = entity;
                                entity.WeightFreights.Add(weightRule);
                                await this._wfRepository.InsertAsync(weightRule);
                            }
                            else
                            {
                                this.ObjectMapper.Map<WeightFreightDto, WeightFreight>(item,weightRule);
                                await this._wfRepository.UpdateAsync(weightRule);
                            }
                        }
                        break;
                    case ChargeWay.ChargeByNum:
                        foreach (var item in input.NumFreights)
                        {
                            var numRule = entity.NumFreights.FirstOrDefault(o => o.Id == item.Id);
                            if (numRule == null)
                            {
                                numRule = this.ObjectMapper.Map<NumFreight>(item);
                                numRule.LogisticChannelBy = entity;
                                entity.NumFreights.Add(numRule);
                                await this._nfRepository.InsertAsync(numRule);
                            }
                            else
                            {
                                this.ObjectMapper.Map<NumFreightDto, NumFreight>(item, numRule);
                                await this._nfRepository.UpdateAsync(numRule);
                            }
                        }
                        break;
                    default:
                        break;
                }
                await this._lcRepository.UpdateAsync(entity);
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
                        var weightRuleDto = input.WeightFreights.FirstOrDefault();
                        weightChange = judgeWeightChange(weightRule, weightRuleDto);
                        break;
                    case ChargeWay.ChargeByNum:
                        var numRule = await this._nfRepository.FirstOrDefaultAsync(o => o.LogisticChannelId == entity.Id);
                        var numRuleDto = input.NumFreights.FirstOrDefault();
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
                    LogisticChannelChange = numChange || weightChange ? JsonConvert.SerializeObject(new ChangeFreightRule()
                    {
                        WeightChargeRules = weightChange ? input.WeightFreights.Select(o => ObjectMapper.Map<WeightFreight>(o)) : null,
                        NumChargeRules = numChange ? input.NumFreights.Select(o => ObjectMapper.Map<NumFreight>(o)) : null
                    }) : string.Empty
                });
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            if (entity.TenantId == tenantId)
            {
                await this._eventBus.TriggerAsync(this.ObjectMapper.Map<ModifyChannelEvent>(entity));
            }
            else
            {
                await this._eventBus.TriggerAsync(new TenantModifyImportChannelEvent() {
                    TenantId = tenantId.Value,
                    LogisticId = entity.LogisticId,
                    LogisticChannelId = entity.Id
                });
            }
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
            await this._tlcRepository.DeleteAsync(o => o.TenantId == tenantId && deleteSet.Contains(o.LogisticChannelId));
            await this._eventBus.TriggerAsync(new TenantImportChannelEvent() {
                TenantId = AbpSession.TenantId.Value,
                AddChannelIds = addSet,
                RemoveChannelIds = deleteSet
            });
            return true;
        }

        public async Task<bool> Verify(VerifyChannelDto input)
        {
            var count = await this._lcRepository.GetAll().Where(o => o.LogisticId == input.LogisticId && o.ChannelName == input.ChannelName).CountAsync();
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

        public async Task<List<Option>> GetOwn()
        {
            var tenantId = AbpSession.TenantId;
            var query = from lc in this._lcRepository.GetAll().IgnoreQueryFilters()
                        where lc.TenantId == tenantId
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

        public async Task Switch(long id, bool IsActive)
        {
            CheckPermission(UpdatePermissionName);

            var entity = await this._lcRepository.SingleAsync(o => o.Id == id);
            if (entity.IsActive == IsActive)
            {
                return;
            }
            entity.IsActive = IsActive;
            await CurrentUnitOfWork.SaveChangesAsync();
            if (IsActive)
            {
                await this._eventBus.TriggerAsync(new StartUseChannelEvent()
                {
                    TenantId = entity.TenantId,
                    LogisticId = entity.LogisticId,
                    LogisticChannelId = entity.Id
                });
            }
            else
            {
                await this._eventBus.TriggerAsync(new BanishChannelEvent()
                {
                    TenantId = entity.TenantId,
                    LogisticId = entity.LogisticId,
                    LogisticChannelId = entity.Id
                });
            }
        }
    }
}
