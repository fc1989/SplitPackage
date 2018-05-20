using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.SplitRules.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.Business.SplitRules
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_SplitRules)]
    public class SplitRuleItemAppService : AsyncCrudAppService<SplitRuleProductClass, RuleItemDto, long, SplitRuleItemFilter, CreateRuleItemDto, UpdateRuleItemDto>
    {
        public SplitRuleItemAppService(IRepository<SplitRuleProductClass, long> repository):base(repository)
        {

        }

        protected override IQueryable<SplitRuleProductClass> CreateFilteredQuery(SplitRuleItemFilter input)
        {
            return base.CreateFilteredQuery(input).Where(o=>o.SplitRuleId == input.SplitRuleId).Include(p => p.SplitRuleBy);
        }
    }
}
