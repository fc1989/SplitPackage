using SplitPackage.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.MultiTenancy
{
    public interface ITenantService
    {
        Task<bool> CreateTenant(SynchronizeTenantDto dto, long otherSystemId);
    }
}
