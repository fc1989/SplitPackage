using SplitPackage.Business.Logistics.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Logistics
{
    public interface ILogisticLogic
    {
        IQueryable<Logistic> GetQuery(int? tenantId);

        IQueryable<TenantLogisticChannel> GetTenantLogisticChannels(int? tenantId);

        IQueryable<LogisticChannel> GetLogisticChannels(int? tenantId);

        IQueryable<LogisticRelated> GetLogisticRelateds(int? tenantId);

        Task<Logistic> Create(CreateLogisticDto input, int? tenantId);

        Task<bool> Verify(int? tenantId, string logisticCode);
    }
}
