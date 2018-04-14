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
            return Tuple.Create(true, string.Empty);
        }

        public Tuple<string, SplitedOrder> Split(SplitRequest request)
        {
            var validResult = this.ValidRequire(request);
            if (!validResult.Item1)
            {
                return Tuple.Create<string, SplitedOrder>(validResult.Item2, null);
            }
            return Tuple.Create( string.Empty, this.spliter.Split(request.OrderId, request.ProList, request.TotalQuantity, request.Type));
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
            if (request.ProList.Any(o => o.Weight <= 0))
            {
                return Tuple.Create<string, SplitedOrder>("货品列表的重量必须大于0", null);
            }
            if (request.ProList.Any(o => !o.PTId.HasValue))
            {
                return Tuple.Create<string, SplitedOrder>("货品列表必须提供PTId", null);
            }
            var unDeployPTIds = request.ProList.Where(o => !this.spliter.GetProductConfig().Products.Any(oi => oi.PTId.Equals(o.PTId))).Select(o => o.PTId.Value).ToList();
            if (unDeployPTIds.Count > 0)
            {
                return Tuple.Create<string, SplitedOrder>(string.Format("未经允许的PTId:{0}", string.Join(",", unDeployPTIds)), null);
            }
            if (request.logistics == null || request.logistics.Count == 0)
            {
                return Tuple.Create<string, SplitedOrder>("需要提供物流信息", null);
            }
            var logisticsIds = request.logistics.Distinct();
            var logistics = this.spliter.GetLogisticsList().Where(o => logisticsIds.Contains(o.Name));
            if (logistics.Count() != logisticsIds.Count())
            {
                string errorStr = string.Format("指定物流Id:{0}不存在", string.Join(",", logisticsIds.Where(o => !logistics.Any(oi => oi.ID == o))));
                return Tuple.Create<string, SplitedOrder>(errorStr, null);
            }
            List<RuleEntity> rules = new List<RuleEntity>();
            foreach (var item in logistics)
            {
                Logistic l = this.spliter.GetLogisticcDic()[Logistic.GetLogisticName(item.Name, "标准型")];
                if (l == null || l.RuleSequenceDic == null)
                {
                    return Tuple.Create<string, SplitedOrder>(string.Format("物流Id:{0}下没有标准型拆单规则", item.ID),null);
                }
                rules.AddRange(l.RuleSequenceDic.Values);
            }
            LogHelper.Logger.Info("Call Spliter.SplitWithOrganization(): " + request);
            SplitedOrder result = this.spliter.SplitWithOrganization1(request.OrderId.ToString(), request.ProList, request.TotalQuantity, rules);
            return Tuple.Create(string.Empty, result);
        }

        public Tuple<string, List<LogisticsModel>> GetLogisticsList(string userName)
        {
            if (!"admin".Equals(userName))
            {
                LogHelper.Logger.Info("UserName不合法", new ArgumentException("UserName不合法"));
                return Tuple.Create<string, List<LogisticsModel>>( "用户信息认证失败", null);
            }
            LogHelper.Logger.Info("Call Spliter.GetLogisticsList(): " + "UserName=" + userName);
            return Tuple.Create( string.Empty, this.spliter.GetLogisticsList());
        }
    }
}
