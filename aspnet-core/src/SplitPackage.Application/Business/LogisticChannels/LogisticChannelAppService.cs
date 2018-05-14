using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.LogisticChannels.Dto;
using SplitPackage.Business.ProductClasses.Dto;
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
    public class LogisticChannelAppService : AsyncCrudAppService<LogisticChannel, LogisticChannelDto, long, LogisticChannelSearchFilter, CreateLogisticChannelDto, UpdateLogisticChannelDto>, ILogisticChannelAppService
    {
        private readonly IRepository<WeightFreight, long> _wfRepository;
        private readonly IRepository<NumFreight, long> _nfRepository;

        public LogisticChannelAppService(IRepository<WeightFreight, long> wfRepository, 
            IRepository<NumFreight, long> nfRepository, 
            IRepository<LogisticChannel, long> repository) : base(repository)
        {
            this.LocalizationSourceName = SplitPackageConsts.LocalizationSourceName;
            this._wfRepository = wfRepository;
            this._nfRepository = nfRepository;
        }

        protected override LogisticChannelDto MapToEntityDto(LogisticChannel entity)
        {
            var result = ObjectMapper.Map<LogisticChannelDto>(entity);
            result.LogisticName = entity.LogisticBy.CorporationName;
            return result;
        }

        public async override Task<LogisticChannelDto> Get(EntityDto<long> input)
        {
            CheckGetPermission();

            var entity = await Repository.GetAllIncluding(o=>o.LogisticBy,o=>o.NumFreights,o=>o.WeightFreights).FirstOrDefaultAsync(o=>o.Id == input.Id);
            var result = MapToEntityDto(entity);
            result.WeightChargeRules = entity.WeightFreights.Select(o => ObjectMapper.Map<WeightFreights.Dto.WeightFreightDto>(o));
            result.NumChargeRules = entity.NumFreights.Select(o => ObjectMapper.Map<NumFreights.Dto.NumFreightDto>(o));
            return result;
        }

        public override async Task<LogisticChannelDto> Create(CreateLogisticChannelDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            var channelId = await Repository.InsertAndGetIdAsync(entity);
            if (input.Way == ChargeWay.ChargeByNum)
            {
                var array = input.NumChargeRules.Select(o =>
                {
                    var result = ObjectMapper.Map<NumFreight>(o);
                    result.LogisticChannelId = channelId;
                    return result;
                });
                foreach (var item in array)
                {
                    await this._nfRepository.InsertAsync(item);
                }
            }
            else if (input.Way == ChargeWay.ChargeByWeight)
            {
                var array = input.NumChargeRules.Select(o =>
                {
                    var result = ObjectMapper.Map<WeightFreight>(o);
                    result.LogisticChannelId = channelId;
                    return result;
                });
                foreach (var item in array)
                {
                    await this._wfRepository.InsertAsync(item);
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public override async Task<LogisticChannelDto> Update(UpdateLogisticChannelDto input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        protected override IQueryable<LogisticChannel> CreateFilteredQuery(LogisticChannelSearchFilter input)
        {
            var query = Repository.GetAll().IgnoreQueryFilters().Include(p=>p.LogisticBy);
            var filter = input.GenerateFilter();
            return filter == null ? query : query.Where(filter);
        }

        public override async Task<PagedResultDto<LogisticChannelDto>> GetAll(LogisticChannelSearchFilter input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p => p.LogisticBy));

            return new PagedResultDto<LogisticChannelDto>(
                totalCount,
                entities.Select(MapToEntityDto).ToList()
            );
        }

        public async Task<bool> Verify(VerifyChannelDto input)
        {
            var count = await this.Repository.GetAll().IgnoreQueryFilters().Where(o => o.LogisticId == input.LogisticId && o.ChannelName == input.ChannelName).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<List<OptionDto<string>>> Query(QueryRequire<long> req)
        {
            Expression<Func<LogisticChannel, bool>> filter;
            if (!string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => o.ChannelName.StartsWith(req.Flag) && o.LogisticBy.TenantId == AbpSession.TenantId;
            }
            else if (string.IsNullOrEmpty(req.Flag) && (req.Ids != null || req.Ids.Count > 0))
            {
                filter = o => req.Ids.Contains(o.Id);
            }
            else if (string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => true;
            }
            else
            {
                filter = o => o.ChannelName.StartsWith(req.Flag) || req.Ids.Contains(o.Id);
            }
            return await this.Repository.GetAll().Where(filter).Take(20).Select(o => new OptionDto<string>
            {
                Value = o.Id.ToString(),
                Label = string.Format("{0}", o.ChannelName)
            }).ToListAsync();
        }
    }
}
