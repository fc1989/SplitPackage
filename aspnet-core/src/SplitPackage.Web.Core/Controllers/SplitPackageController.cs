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
    [Route("api/[controller]"), AbpAuthorize]
    [ApiExplorerSettings(GroupName = "open")]
    public class SplitPackageController : SplitPackageControllerBase
    {
        private readonly ISplitService _SplitAppService;

        public SplitPackageController(ISplitService splitAppService)
        {
            this._SplitAppService = splitAppService;
        }

        [HttpPost, Route("Split")]
        public async Task<JsonResult> Split([FromBody]SplitRequest request)
        {
            var result = await this._SplitAppService.Split(request, AbpSession.TenantId);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }

        [HttpPost, Route("SplitWithExp1")]
        public async Task<JsonResult> SplitWithExp1([FromBody]SplitWithExpRequest1 request)
        {
            var result = await this._SplitAppService.SplitWithOrganization1(request, AbpSession.TenantId);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }

        [HttpPost, Route("GetLogisticsList")]
        public async Task<JsonResult> GetLogisticsList()
        {
            var result = await this._SplitAppService.GetLogisticsList(AbpSession.TenantId);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<List<LogisticsModel>>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }
    }

    /// <summary>
    /// 妈的,action竟然AllowAnonymous特性没用
    /// </summary>
    [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
    [Route("api/[controller]"), AllowAnonymous]
    [ApiExplorerSettings(GroupName = "open")]
    public class CommonController : SplitPackageControllerBase
    {
        private readonly ISplitService _SplitAppService;

        public CommonController(ISplitService splitAppService)
        {
            this._SplitAppService = splitAppService;
        }

        [HttpPost, Route("api/SplitPackage/ProductClass")]
        public async Task<List<ProductSortSimpleDto1>> GetProductClass()
        {
            return await this._SplitAppService.GetProductClass();
        }
    }
}
