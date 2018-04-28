using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.LogisticLines.Dto;
using SplitPackage.Business.ProductClasses.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.LogisticLines
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_LogisticLines)]
    public class LogisticLineAppService : AsyncCrudAppService<LogisticLine, LogisticLineDto, long, PagedResultRequestDto, CreateLogisticLineDto, UpdateLogisticLineDto>, ILogisticLineAppService
    {
        public LogisticLineAppService(IRepository<LogisticLine, long> repository) : base(repository)
        {

        }

        public override async Task<LogisticLineDto> Create(CreateLogisticLineDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public override async Task<PagedResultDto<LogisticLineDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p => p.LogisticBy));

            return new PagedResultDto<LogisticLineDto>(
                totalCount,
                entities.Select(o => new LogisticLineDto() {
                    Id = o.Id,
                    LineName = o.LineName,
                    LineCode = o.LineCode,
                    LogisticId = o.LogisticId,
                    LogisticName = o.LogisticBy.CorporationName,
                    IsActive = o.IsActive
                }).ToList()
            );
        }

        public async Task<bool> Verify(long logisticId, string code)
        {
            var count = await this.Repository.GetAll().Where(o => o.LineCode == code && o.LogisticId == logisticId).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<List<OptionDto>> Query(QueryRequire<long> req)
        {
            Expression<Func<LogisticLine, bool>> filter;
            if (!string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => o.LineCode.StartsWith(req.Flag) || o.LineName.StartsWith(req.Flag) && o.LogisticBy.TenantId == AbpSession.TenantId;
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
                filter = o => o.LineCode.StartsWith(req.Flag) || o.LineName.StartsWith(req.Flag) || req.Ids.Contains(o.Id);
            }
            return await this.Repository.GetAll().Where(filter).Take(20).Select(o => new OptionDto
            {
                Value = o.Id.ToString(),
                Label = string.Format("{0}[{1}]", o.LineName, o.LineCode)
            }).ToListAsync();
        }
    }
}
