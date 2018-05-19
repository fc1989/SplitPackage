using Abp.Dependency;
using Abp.Logging;
using SplitPackage.Split.Dto;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.Split
{
    public class SplitService : ISplitService, ISingletonDependency
    {
        protected Spliter spliter { get; set; }

        public void Initialize(string folderPath)
        {
            spliter = new Spliter();
            this.spliter.Initialize(folderPath);
            Spliter.TheSubLevelDic = this.spliter.SubLevelDic;
        }

        protected Tuple<bool, string> ValidRequire<T>(T request) where T : BaseRequest
        {
            //非空验证
            if (request == null)
            {
                LogHelper.Logger.Info("Request model is null", new ArgumentException("SRequest model is null"));
                return Tuple.Create(false, "Request model is null");
            }
            //身份信息认证
            if (!"admin".Equals(request.UserName))
            {
                LogHelper.Logger.Info("UserName不合法", new ArgumentException("UserName不合法"));
                return Tuple.Create(false, "UserName不合法");
            }
            // 待拆分商品清单有效性验证
            if ((request.ProList == null) || (request.ProList.Count <= 0))
            {
                LogHelper.Logger.Info("Product list is null", new ArgumentException("Product list is null"));
                return Tuple.Create(false, "Product list is null");
            }
            //商品价格
            if (request.ProList.Any(o => o.ProPrice <= 0))
            {
                return Tuple.Create(false, "商品价格必须大于0");
            }
            //商品重量
            if (request.ProList.Any(o => o.Weight <= 0))
            {
                return Tuple.Create(false, "商品重量必须大于0");
            }
            //商品数量
            if (request.ProList.Any(o => o.Quantity <= 0))
            {
                return Tuple.Create(false, "商品数量必须大于0");
            }
            //sku或ptid
            if (request is SplitWithExpRequest1)
            {
                if (request.ProList.Any(o => !o.PTId.HasValue))
                {
                    return Tuple.Create(false, "缺少PTId");
                }
                var unDeployPTIds = request.ProList.Where(o => !this.spliter.GetProductConfig().Products.Any(oi => oi.PTId.Equals(o.PTId))).Select(o => o.PTId.Value).ToList();
                if (unDeployPTIds.Count > 0)
                {
                    return Tuple.Create(false, string.Format("不存在PTId:{0}", string.Join(",", unDeployPTIds.Distinct())));
                }
            }
            else
            {
                if (request.ProList.Any(o => string.IsNullOrEmpty(o.SkuNo)))
                {
                    return Tuple.Create(false, "缺少SkuNo");
                }
                var unDeploySkuNoes = request.ProList.Where(o => !this.spliter.GetProductConfig().Products.Any(oi => oi.SKUNo.Trim().Equals(o.SkuNo))).Select(o => o.SkuNo).ToList();
                if (unDeploySkuNoes.Count > 0)
                {
                    return Tuple.Create(false, string.Format("不存在SkuNo:{0}", string.Join(",", unDeploySkuNoes.Distinct())));
                }
            }
            //指定物流判断
            if (request is SplitWithExpRequest)
            {
                SplitWithExpRequest swr = (request as SplitWithExpRequest);
                string key = Logistic.GetLogisticName(swr.LogisticsName, swr.GradeName);
                if (key.Equals(Logistic.GetLogisticName(string.Empty, string.Empty)))
                {
                    return Tuple.Create(false, "请提供指定物流商");
                }
                if (!this.spliter.GetLogisticcDic().ContainsKey(key))
                {
                    return Tuple.Create(false, string.Format("不存在{0}的规则", key));
                }
            }
            else if (request is SplitWithExpRequest1)
            {
                List<string> requestLogistics = (request as SplitWithExpRequest1).logistics;
                if (requestLogistics == null || requestLogistics.Count == 0)
                {
                    return Tuple.Create(false, "请提供指定物流商");
                }
                var logisticsIds = requestLogistics.Distinct();
                var unDeploylogisticsIds = logisticsIds.Where(o => !this.spliter.GetLogisticsList().Any(oi => oi.Name.Trim().Equals(o))).ToList();
                if (unDeploylogisticsIds.Count > 0)
                {
                    return Tuple.Create(false, string.Format("指定物流商:{0}不存在", string.Join(",", unDeploylogisticsIds)));
                }
            }
            return Tuple.Create(true, string.Empty);
        }

        public Tuple<string, SplitedOrder> Split(SplitRequest request)
        {
            var validResult = this.ValidRequire(request);
            if (!validResult.Item1)
            {
                return Tuple.Create<string, SplitedOrder>(validResult.Item2, null);
            }
            return Tuple.Create(string.Empty, this.spliter.Split(request.OrderId, request.ProList, request.TotalQuantity, request.Type));
        }

        public Tuple<string, SplitedOrder> SplitWithOrganization(SplitWithExpRequest request)
        {
            var validResult = this.ValidRequire(request);
            if (!validResult.Item1)
            {
                return Tuple.Create<string, SplitedOrder>(validResult.Item2, null);
            }
            return Tuple.Create(string.Empty, this.spliter.SplitWithOrganization(request.OrderId, request.ProList, request.TotalQuantity, request.LogisticsName, request.GradeName));
        }

        public Tuple<string, SplitedOrder> SplitWithOrganization1(SplitWithExpRequest1 request)
        {
            var validResult = this.ValidRequire(request);
            if (!validResult.Item1)
            {
                return Tuple.Create<string, SplitedOrder>(validResult.Item2, null);
            }
            LogHelper.Logger.Info("Call Spliter.SplitWithOrganization(): " + request);
            SplitedOrder result = this.spliter.SplitWithOrganization1(request.OrderId.ToString(), request.ProList, request.TotalQuantity, request.logistics);
            return Tuple.Create(string.Empty, result);
        }

        public Tuple<string, List<LogisticsModel>> GetLogisticsList(string userName)
        {
            if (!"admin".Equals(userName))
            {
                LogHelper.Logger.Info("UserName不合法", new ArgumentException("UserName不合法"));
                return Tuple.Create<string, List<LogisticsModel>>("用户信息认证失败", null);
            }
            LogHelper.Logger.Info("Call Spliter.GetLogisticsList(): " + "UserName=" + userName);
            return Tuple.Create(string.Empty, this.spliter.GetLogisticsList());
        }
    }
}
