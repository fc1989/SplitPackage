using Abp.Logging;
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

namespace SplitPackage.Controllers
{
    [Route("api/[controller]")]
    public class SplitPackageController : SplitPackageControllerBase
    {
        private readonly ISplitService _SplitAppService;

        public SplitPackageController(ISplitService splitAppService)
        {
            this._SplitAppService = splitAppService;
        }

        protected T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            return serializer.Deserialize<T>(new JsonTextReader(sr));
        }

        #region Split 拆包接口
        /// <summary>
        /// 拆包
        /// </summary>
        /// <param name="userName">认证信息（用户名）</param>
        /// <param name="orderId">订单号</param>
        /// <param name="proListJsonStr">货品列表[货号、SKUNo、名称、数量、商品单价]</param>
        /// <param name="type">拆单方式 1:最便宜拆单 2:最快 3.最少拆包数</type>
        /// <returns></returns>
        [HttpPost, Route("Split"), AllowAnonymous]
        public JsonResult Split()
        {
            LogHelper.Logger.Info("Call Split()");
            //获取表单数据
            string jsonStr = Request.Form["jsonStr"];
            SplitRequest request;
            try
            {
                request = DeserializeJsonToObject<SplitRequest>(jsonStr);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], ex.ToString()));
            }
            LogHelper.Logger.Info("Call Spliter.Split(): " + request);
            var result = this._SplitAppService.Split(request);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }

        /// <summary>
        /// 指定物流拆包
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SplitWithExp"), AllowAnonymous]
        public JsonResult SplitWithExp()
        {
            LogHelper.Logger.Info("Call SplitWithExp()");
            //获取表单数据
            string jsonStr = Request.Form["jsonStr"];
            SplitWithExpRequest request;
            try
            {
                request = DeserializeJsonToObject<SplitWithExpRequest>(jsonStr);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], ex.ToString()));
            }
            LogHelper.Logger.Info("Call Spliter.Split(): " + request);
            var result = this._SplitAppService.SplitWithOrganization(request);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }

        [HttpPost, Route("SplitWithExp1"), AllowAnonymous]
        public JsonResult SplitWithExp1()
        {
            LogHelper.Logger.Info(string.Format("Call {0}", nameof(SplitWithExp1)));
            //获取表单数据
            string jsonStr = Request.Form["jsonStr"];
            SplitWithExpRequest1 request;
            try
            {
                request = DeserializeJsonToObject<SplitWithExpRequest1>(jsonStr);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], ex.ToString()));
            }
            LogHelper.Logger.Info("Call Spliter.Split(): " + request);
            var result = this._SplitAppService.SplitWithOrganization1(request);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }
        #endregion

        #region LogisticsInfo 物流信息查询接口
        /// <summary>
        /// 物流信息查询接口
        /// </summary>
        /// <param name="userName">认证信息（用户名）</param>
        /// <returns></returns>
        [HttpPost, Route("GetLogisticsList"), AllowAnonymous]
        public JsonResult GetLogisticsList()
        {
            LogHelper.Logger.Info("Call GetLogisticsList()");
            //获取表单数据
            string userName = Request.Form["userName"];
            var result = this._SplitAppService.GetLogisticsList(userName);
            if (!string.IsNullOrEmpty(result.Item1))
            {
                return new JsonResult(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], result.Item1));
            }
            return new JsonResult(new ResultMessage<List<LogisticsModel>>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result.Item2));
        }
        #endregion
    }
}
