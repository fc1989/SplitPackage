using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Business.SplitRules.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.SplitRules
{
    public class SplitRuleAppService : AsyncCrudAppService<SplitRule, SplitRuleDto, long, PagedResultRequestDto, CreateSplitRuleDto, UpdateSplitRuleDto>, ISplitRuleAppService
    {
        public SplitRuleAppService(IRepository<SplitRule, long> repository) : base(repository)
        {

        }

        public override async Task<PagedResultDto<SplitRuleDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p => p.LogisticLineBy));

            return new PagedResultDto<SplitRuleDto>(
                totalCount,
                entities.Select(o => new SplitRuleDto()
                {
                    Id = o.Id,
                    LogisticLineId = o.LogisticLineId,
                    LogisticLineName = o.LogisticLineBy.LineName,
                    MinPackage = o.MinPackage,
                    MaxPackage = o.MaxPackage,
                    MaxWeight = o.MaxWeight,
                    MaxTax = o.MaxTax,
                    MaxPrice = o.MaxPrice,
                    IsActive = o.IsActive
                }).ToList()
            );
        }
    }
}
