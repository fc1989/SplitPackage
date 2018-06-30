using SplitPackage.Split.Dto;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.SplitV1
{
    public interface ISplitService
    {
        Task<SplitedOrder> Split(SplitRequest request, int? tenantId);
        Task<SplitedOrder> SplitWithOrganization1(SplitWithExpRequest1 request, int? tenantId);
        Task<List<LogisticsModel>> GetLogisticsList(int? tenantId);
        Task<List<ProductSortSimpleDto1>> GetProductClass();
    }
}
