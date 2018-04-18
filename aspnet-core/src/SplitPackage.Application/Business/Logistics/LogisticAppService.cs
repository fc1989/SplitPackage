using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.Logistics.Dto;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.Logistics
{
    public class LogisticAppService : AsyncCrudAppService<Logistic, LogisticDto, long, PagedResultRequestDto, CreateLogisticDto, UpdateLogisticDto>, ILogisticAppService
    {
        public LogisticAppService(IRepository<Logistic, long> repository) : base(repository)
        {

        }
    }
}
