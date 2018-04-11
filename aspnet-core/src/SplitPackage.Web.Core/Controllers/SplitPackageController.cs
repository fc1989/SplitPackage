using Abp.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SplitPackage.Models;
using SplitPackage.Split;
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
        private readonly ISplitAppService _SplitAppService;

        public SplitPackageController(ISplitAppService splitAppService)
        {
            this._SplitAppService = splitAppService;
        }

        protected T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            return serializer.Deserialize<T>(new JsonTextReader(sr));
        }

        protected Tuple<bool, ResultMessage<String>, T> ValidRequire<T>(string jsonStr) where T : BaseRequest
        {
            T request = null;
            try
            {
                request = DeserializeJsonToObject<T>(jsonStr);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                return Tuple.Create(false, new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], ex.ToString()), default(T));
            }
            //非空验证
            if (request == null)
            {
                LogHelper.Logger.Info("SplitRequest model is null", new ArgumentException("SplitRequest model is null"));
                return Tuple.Create(false, new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], "SplitRequest model is null"), default(T));
            }
            //身份信息认证
            if (!"admin".Equals(request.UserName))
            {
                LogHelper.Logger.Info("UserName不合法", new ArgumentException("UserName不合法"));
                return Tuple.Create(false, new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], "UserName不合法"), default(T));
            }
            // 待拆分商品清单有效性验证
            if ((request.ProList == null) || (request.ProList.Count <= 0))
            {
                LogHelper.Logger.Info("Product list is null", new ArgumentException("Product list is null"));
                return Tuple.Create(false, new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], "Product list is null"), default(T));
            }
            return Tuple.Create<bool, ResultMessage<String>, T>(true, null, request);
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
        public ActionResult Split()
        {
            LogHelper.Logger.Info("Call Split()");
            //获取表单数据
            string jsonStr = Request.Form["jsonStr"];
            var validResult = ValidRequire<SplitRequest>(jsonStr);
            if (!validResult.Item1)
            {
                return Ok(validResult.Item2);
            }
            SplitRequest request = validResult.Item3;
            LogHelper.Logger.Info("Call Spliter.Split(): " + request);
            SplitedOrder result = this._SplitAppService.Split(request.OrderId, request.ProList, request.TotalQuantity, request.Type);
            return Ok(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result));
        }

        /// <summary>
        /// 指定物流拆包
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SplitWithExp"), AllowAnonymous]
        public ActionResult SplitWithExp()
        {
            LogHelper.Logger.Info("Call SplitWithExp()");

            //获取表单数据
            string jsonStr = Request.Form["jsonStr"];
            var validResult = ValidRequire<SplitWithExpRequest>(jsonStr);
            if (!validResult.Item1)
            {
                return Ok(validResult.Item2);
            }
            SplitWithExpRequest request = validResult.Item3;
            LogHelper.Logger.Info("Call Spliter.SplitWithOrganization(): " + request);
            SplitedOrder result = this._SplitAppService.SplitWithOrganization(request.OrderId.ToString(), request.ProList, request.TotalQuantity, request.LogisticsName, request.GradeName);
            return Ok(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result));
        }

        [HttpPost, Route("SplitWithExp1"), AllowAnonymous]
        public ActionResult SplitWithExp1()
        {
            LogHelper.Logger.Info(string.Format("Call {0}", nameof(SplitWithExp1)));
            string jsonStr = Request.Form["jsonStr"];
            var validResult = ValidRequire<SplitWithExpRequest1>(jsonStr);
            if (!validResult.Item1)
            {
                return Ok(validResult.Item2);
            }
            SplitWithExpRequest1 request = validResult.Item3;
            if (request.ProList.Any(o => o.Weight <= 0))
            {
                return Ok(new ResultMessage<String>((int)ResultCode.KeyIsNotExist, ResultConfig.Configs[ResultCode.Success], "货品列表的重量必须大于0"));
            }
            if (request.ProList.Any(o => !o.PTId.HasValue))
            {
                return Ok(new ResultMessage<String>((int)ResultCode.KeyIsNotExist, ResultConfig.Configs[ResultCode.Success], "货品列表必须提供PTId"));
            }
            if (request.logistics == null || request.logistics.Count == 0)
            {
                return Ok(new ResultMessage<String>((int)ResultCode.KeyIsNotExist, ResultConfig.Configs[ResultCode.Success], "需要提供物流信息"));
            }
            var logisticsIds = request.logistics.Distinct();
            var logistics = this._SplitAppService.GetLogisticsList().Where(o => logisticsIds.Contains(o.Name));
            if (logistics.Count() != logisticsIds.Count())
            {
                string errorStr = string.Format("指定物流Id:{0}不存在", string.Join(",", logisticsIds.Where(o => !logistics.Any(oi => oi.ID == o))));
                return Ok(new ResultMessage<String>((int)ResultCode.KeyIsNotExist, ResultConfig.Configs[ResultCode.Success], errorStr));
            }
            List<RuleEntity> rules = new List<RuleEntity>();
            foreach (var item in logistics)
            {
                Logistic l = this._SplitAppService.GetLogisticcDic()[Logistic.GetLogisticName(item.Name, "标准型")];
                if (l == null || l.RuleSequenceDic == null)
                {
                    return Ok(new ResultMessage<String>((int)ResultCode.KeyIsNotExist, ResultConfig.Configs[ResultCode.Success], string.Format("物流Id:{0}下没有标准型拆单规则", item.ID)));
                }
                rules.AddRange(l.RuleSequenceDic.Values);
            }
            LogHelper.Logger.Info("Call Spliter.SplitWithOrganization(): " + request);
            SplitedOrder result = this._SplitAppService.SplitWithOrganization1(request.OrderId.ToString(), request.ProList, request.TotalQuantity, rules);
            return Ok(new ResultMessage<SplitedOrder>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], result));
        }
        #endregion

        #region LogisticsInfo 物流信息查询接口
        /// <summary>
        /// 物流信息查询接口
        /// </summary>
        /// <param name="userName">认证信息（用户名）</param>
        /// <returns></returns>
        [HttpPost, Route("GetLogisticsList"), AllowAnonymous]
        public ActionResult GetLogisticsList()
        {
            LogHelper.Logger.Info("Call GetLogisticsList()");
            string userName = Request.Form["userName"];

            //模拟验证
            if (!"admin".Equals(userName))
            {
                LogHelper.Logger.Info("UserName不合法", new ArgumentException("UserName不合法"));
                return Ok(new ResultMessage<String>((int)ResultCode.KeyIsNull, ResultConfig.Configs[ResultCode.Success], "用户信息认证失败"));
            }

            LogHelper.Logger.Info("Call Spliter.GetLogisticsList(): " + "UserName=" + userName);
            List<LogisticsModel> list = this._SplitAppService.GetLogisticsList();
            return Ok(new ResultMessage<List<LogisticsModel>>((int)ResultCode.Success, ResultConfig.Configs[ResultCode.Success], list));
        }
        #endregion
    }
}
