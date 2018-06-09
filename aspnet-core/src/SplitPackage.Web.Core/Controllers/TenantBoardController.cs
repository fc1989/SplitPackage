using Abp.Authorization;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitPackage.MultiTenancy;
using SplitPackage.MultiTenancy.Dto;
using SplitPackage.Split;
using SplitPackage.Split.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Controllers
{
    [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
    [Route("api/[controller]"), AbpAuthorize]
    [ApiExplorerSettings(GroupName = "open")]
    public class TenantBoardController : SplitPackageControllerBase
    {
        private readonly ITenantService _tenantService;
        IPrincipalAccessor _principalAccessor;

        public TenantBoardController(ITenantService tenantService, IPrincipalAccessor principalAccessor)
        {
            this._tenantService = tenantService;
            this._principalAccessor = principalAccessor;
        }

        [HttpPost,Route("CreateTenant")]
        public async Task<bool> CreateTenant([FromBody]SynchronizeTenantDto input)
        {
            var otherSystem = _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "SplitPackageOtherSystemId");
            if (string.IsNullOrEmpty(otherSystem?.Value))
            {
                throw new UserFriendlyException("不存在该系统标识");
            }
            return await _tenantService.CreateTenant(input, long.Parse(otherSystem.Value));
        }
    }
}
