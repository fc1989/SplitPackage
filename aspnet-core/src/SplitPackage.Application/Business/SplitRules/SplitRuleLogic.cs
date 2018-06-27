using Abp.Dependency;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitPackage.Business.SplitRules
{
    public class SplitRuleLogic : ISplitRuleLogic, ITransientDependency
    {
        private readonly IRepository<Logistic, long> _logisticRepository;
        private readonly IRepository<LogisticChannel, long> _logisticChannelRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tenantLogisticChannelRepository;
        private readonly IRepository<SplitRule, long> _splitRuleRepository;
        private readonly IRepository<SplitRuleItem, long> _splitRuleProductClassRepository;

        public SplitRuleLogic(IRepository<Logistic, long> logisticRepository,
            IRepository<LogisticChannel, long> logisticChannelRepository,
            IRepository<TenantLogisticChannel, long> tenantLogisticChannelRepository,
            IRepository<SplitRule, long> splitRuleRepository,
            IRepository<SplitRuleItem, long> splitRuleProductClassRepository)
        {
            this._logisticRepository = logisticRepository;
            this._logisticChannelRepository = logisticChannelRepository;
            this._tenantLogisticChannelRepository = tenantLogisticChannelRepository;
            this._splitRuleRepository = splitRuleRepository;
            this._splitRuleProductClassRepository = splitRuleProductClassRepository;
        }

        public async Task<List<string>> GetIncludeSplitPTId(List<string> ptids, int? tenantId)
        {
            var query = (from l in this._logisticRepository.GetAll().IgnoreQueryFilters()
                         join lc in this._logisticChannelRepository.GetAll().IgnoreQueryFilters() on l.Id equals lc.LogisticId
                         join tl in this._tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters() on lc.Id equals tl.LogisticChannelId into left1
                         from tb in left1.DefaultIfEmpty()
                         join sr in this._splitRuleRepository.GetAll() on lc.Id equals sr.LogisticChannelId
                         join srp in this._splitRuleProductClassRepository.GetAll() on sr.Id equals srp.SplitRuleId
                         where !l.IsDeleted && !lc.IsDeleted && (l.TenantId == tenantId || tb.TenantId == tenantId) && ptids.Contains(srp.StintMark)
                         select srp.StintMark);
             return await query.Distinct().ToListAsync();
        }
    }
}
