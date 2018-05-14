using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.WeightFreights.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.WeightFreights
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_WeightFreights)]
    public class WeightFreightAppService : AsyncCrudAppService<WeightFreight, WeightFreightDto, long, PagedResultRequestDto, CreateWeightFreightDto, UpdateWeightFreightDto>, IWeightFreightAppService
    {
        public WeightFreightAppService(IRepository<WeightFreight, long> repository) : base(repository)
        {

        }

        protected override IQueryable<WeightFreight> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input).Include(p=>p.LogisticChannelBy);
        }

        protected override WeightFreightDto MapToEntityDto(WeightFreight entity)
        {
            var result = base.MapToEntityDto(entity);
            result.LogisticChannelName = entity.LogisticChannelBy.ChannelName;
            return result;
        }
    }
}
