using Abp.Authorization;
using Abp.Logging;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json;
using SplitPackage.Split;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Controllers
{
    [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "open")]
    public class SplitPackageController : SplitPackageControllerBase
    {
        private readonly ISplitService _SplitAppService;

        public SplitPackageController(ISplitService splitAppService)
        {
            this._SplitAppService = splitAppService;
        }

        [HttpPost, Route("Split"), AbpAuthorize]
        public async Task<ResultMessage<SplitedOrder>> Split([FromBody]SplitRequest request)
        {
            var result = await this._SplitAppService.Split(request, AbpSession.TenantId);
            return new ResultMessage<SplitedOrder>(ResultCode.Success, "success", result);
        }

        [HttpPost, Route("SplitWithExp1")]
        public async Task<ResultMessage<SplitedOrder>> SplitWithExp1([FromBody]SplitWithExpRequest1 request)
        {
            var result = await this._SplitAppService.SplitWithOrganization1(request, AbpSession.TenantId);
            return new ResultMessage<SplitedOrder>(ResultCode.Success, "success", result);
        }

        [HttpPost, Route("GetLogisticsList"), AbpAuthorize]
        public async Task<ResultMessage<List<LogisticsModel>>> GetLogisticsList()
        {
            var result = await this._SplitAppService.GetLogisticsList(AbpSession.TenantId);
            return new ResultMessage<List<LogisticsModel>>(ResultCode.Success, "success", result);
        }

        [HttpPost, Route("ProductClass"), AbpAllowAnonymous]
        public async Task<List<ProductSortSimpleDto1>> GetProductClass()
        {
            return await this._SplitAppService.GetProductClass();
        }
    }
}
