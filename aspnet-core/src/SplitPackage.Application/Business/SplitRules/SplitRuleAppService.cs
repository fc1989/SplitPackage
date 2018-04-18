using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.SplitRules.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.SplitRules
{
    public class SplitRuleAppService : AsyncCrudAppService<SplitRule, SplitRuleDto, long, PagedResultRequestDto, CreateSplitRuleDto, UpdateSplitRuleDto>, ISplitRuleAppService
    {
        public SplitRuleAppService(IRepository<SplitRule, long> repository) : base(repository)
        {

        }
    }
}
