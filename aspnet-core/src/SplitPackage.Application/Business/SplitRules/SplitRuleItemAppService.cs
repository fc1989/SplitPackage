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
using System.Threading.Tasks;

namespace SplitPackage.Business.SplitRules
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_SplitRules)]
    public class SplitRuleItemAppService : AsyncCrudAppService<SplitRuleProductClass, RuleItemDto, long, SplitRuleItemFilter, CreateRuleItemDto, UpdateRuleItemDto>
    {
        private readonly IRepository<ProductClass, long> _productClassRepository;

        public SplitRuleItemAppService(IRepository<SplitRuleProductClass, long> repository, 
            IRepository<ProductClass, long> productClassRepository) :base(repository)
        {
            this._productClassRepository = productClassRepository;
        }

        protected override IQueryable<SplitRuleProductClass> CreateFilteredQuery(SplitRuleItemFilter input)
        {
            return base.CreateFilteredQuery(input).Where(o=>o.SplitRuleId == input.SplitRuleId).Include(p => p.SplitRuleBy);
        }

        public override async Task<PagedResultDto<RuleItemDto>> GetAll(SplitRuleItemFilter input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query);

            var result = entities.Select(MapToEntityDto).ToList();

            var pcSet = _productClassRepository.GetAll().Where(o => result.Select(oi => oi.PTId).Contains(o.PTId));
            result.ForEach(o =>{
                var pc = pcSet.Where(oi => oi.PTId == o.PTId);
                o.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ClassName)), o.PTId);
            });
            return new PagedResultDto<RuleItemDto>(
                totalCount, result
            );
        }
    }
}
