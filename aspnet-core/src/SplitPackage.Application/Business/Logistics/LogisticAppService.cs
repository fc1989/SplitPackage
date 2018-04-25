using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.Logistics.Dto;
using SplitPackage.Business.ProductClasses.Dto;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Logistics
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_Logistics)]
    public class LogisticAppService : AsyncCrudAppService<Logistic, LogisticDto, long, PagedResultRequestDto, CreateLogisticDto, UpdateLogisticDto>, ILogisticAppService
    {
        public LogisticAppService(IRepository<Logistic, long> repository) : base(repository)
        {

        }

        public override async Task<LogisticDto> Create(CreateLogisticDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task<bool> Verify(string flag)
        {
            var count = await this.Repository.GetAll().Where(o => o.LogisticFlag == flag).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<object> Query(QueryRequire<long> req)
        {
            Expression<Func<Logistic, bool>> filter;
            if (!string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => o.LogisticFlag.StartsWith(req.Flag) || o.CorporationName.StartsWith(req.Flag);
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
                filter = o => o.LogisticFlag.StartsWith(req.Flag) || o.CorporationName.StartsWith(req.Flag) || req.Ids.Contains(o.Id);
            }
            return await this.Repository.GetAll().Where(filter).Take(20).Select(o => new {
                value = o.Id,
                label = string.Format("{0}[{1}]", o.CorporationName, o.LogisticFlag)
            }).ToListAsync();
        }
    }
}
