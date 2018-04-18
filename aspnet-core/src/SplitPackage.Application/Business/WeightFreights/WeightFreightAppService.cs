using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.WeightFreights.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.WeightFreights
{
    public class WeightFreightAppService : AsyncCrudAppService<WeightFreight, WeightFreightDto, long, PagedResultRequestDto, CreateWeightFreightDto, UpdateWeightFreightDto>, IWeightFreightAppService
    {
        public WeightFreightAppService(IRepository<WeightFreight, long> repository) : base(repository)
        {

        }
    }
}
