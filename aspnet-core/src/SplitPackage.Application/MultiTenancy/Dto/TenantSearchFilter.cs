using Abp.Application.Services.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.MultiTenancy.Dto
{
    public class TenantSearchFilter: PageSearchFilter<Tenant>
    {
        public string Name { get; set; }
    }
}
