using Abp.Authorization;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using SplitPackage.SplitV1;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using System.Collections.Generic;
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

        [HttpPost, Route("SplitWithExp1"), AbpAuthorize]
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
        public async Task<ResultMessage<List<ProductSortSimpleDto1>>> GetProductClass()
        {
            var result = await this._SplitAppService.GetProductClass(); ;
            return new ResultMessage<List<ProductSortSimpleDto1>>(ResultCode.Success,"success",result);
        }
    }
}
