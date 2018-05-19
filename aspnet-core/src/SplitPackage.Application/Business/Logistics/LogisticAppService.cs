using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.Dto;
using SplitPackage.Business.Logistics.Dto;
using SplitPackage.Business.Products.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Logistics
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_Logistics)]
    public class LogisticAppService : AsyncCrudAppService<Logistic, LogisticDto, long, LogisticSearchFilter, CreateLogisticDto, UpdateLogisticDto>, ILogisticAppService
    {
        private readonly IRepository<LogisticChannel, long> _logisticChannelRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tenantLogisticChannelRepository;

        public LogisticAppService(IRepository<Logistic, long> repository,
            IRepository<LogisticChannel, long> logisticChannelRepository,
            IRepository<TenantLogisticChannel, long> tenantLogisticChannelRepository) : base(repository)
        {
           this._logisticChannelRepository = logisticChannelRepository;
           this._tenantLogisticChannelRepository = tenantLogisticChannelRepository;
        }

        protected override IQueryable<Logistic> CreateFilteredQuery(LogisticSearchFilter input)
        {
            var query = Repository.GetAll();
            if (AbpSession.TenantId.HasValue)
            {
                query = from l in query.IgnoreQueryFilters()
                        join ll in _logisticChannelRepository.GetAll().IgnoreQueryFilters() on l.Id equals ll.LogisticId into left
                        from tb1 in left.DefaultIfEmpty()
                        join tl in _tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters() on tb1.Id equals tl.LogisticChannelId into left1
                        from tb in left1.DefaultIfEmpty()
                        where !l.IsDeleted && (l.TenantId == AbpSession.TenantId.Value || tb.TenantId == AbpSession.TenantId.Value)
                        select l;
            }
            var filter = input.GenerateFilter();
            return filter == null ? query : query.Where(filter);
        }

        public override async Task<LogisticDto> Create(CreateLogisticDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            if (AbpSession.TenantId.HasValue)
            {
                entity.LogisticCode = string.Format("{0}_{1}",AbpSession.TenantId.Value,entity.LogisticCode);
            }
            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task<bool> Verify(string flag)
        {
            if (AbpSession.TenantId.HasValue)
            {
                flag = string.Format("{0}_{1}", AbpSession.TenantId.Value, flag);
            }
            var count = await this.Repository.GetAll().Where(o => o.LogisticCode == flag).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<List<OptionDto<string>>> Query(QueryRequire<long> req)
        {
            var param = Expression.Parameter(typeof(Logistic), "o");
            var variable = Expression.Constant(req);
            var judge1 = Expression.Call(Expression.Property(param, nameof(Logistic.LogisticCode)), typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Property(variable, "Flag"));
            var judge2 = Expression.Call(Expression.Property(param, nameof(Logistic.CorporationName)), typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Property(variable, "Flag"));
            Expression<Func<Logistic, bool>> filter = Expression.Lambda<Func<Logistic, bool>>(Expression.OrElse(judge1, judge2), param);

            if (!string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => o.LogisticCode.StartsWith(req.Flag) || o.CorporationName.StartsWith(req.Flag);
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
                filter = o => o.LogisticCode.StartsWith(req.Flag) || o.CorporationName.StartsWith(req.Flag) || req.Ids.Contains(o.Id);
            }
            return await this.Repository.GetAll().Where(filter).Take(20).Select(o => new OptionDto<string>
            {
                Value = o.Id.ToString(),
                Label = string.Format("{0}[{1}]", o.CorporationName, o.LogisticCode)
            }).ToListAsync();
        }
    }
}
