using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.LogisticLines.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.LogisticLines
{
    public class LogisticLineAppService : AsyncCrudAppService<LogisticLine, LogisticLineDto, long, PagedResultRequestDto, CreateLogisticLineDto, UpdateLogisticLineDto>, ILogisticLineAppService
    {
        public LogisticLineAppService(IRepository<LogisticLine, long> repository) : base(repository)
        {

        }
    }
}
