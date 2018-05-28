using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.SplitRules
{
    public interface ISplitRuleLogic
    {
        Task<List<string>> GetIncludeSplitPTId(List<string> ptids, int? tenantId);
    }
}
