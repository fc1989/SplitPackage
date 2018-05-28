using Abp.Logging;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SplitPackage.Split;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Controllers
{
    [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
    [Route("api/[controller]")]
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
}
