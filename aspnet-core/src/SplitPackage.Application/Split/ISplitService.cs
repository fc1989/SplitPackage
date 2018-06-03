using SplitPackage.Split.Dto;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Split
{
    public interface ISplitService
    {
        Task<Tuple<string, SplitedOrder>> Split(SplitRequest request, int? tenantId);
        Task<Tuple<string, SplitedOrder>> SplitWithOrganization1(SplitWithExpRequest1 request, int? tenantId);
        Task<Tuple<string, List<LogisticsModel>>> GetLogisticsList(int? tenantId);
        Task<List<ProductSortDto>> GetProductClass();
    }
}
