using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using SplitPackage.Split.Common;
using System.Diagnostics;
using Abp.Logging;
using SplitPackage.RuleModels;

namespace SplitPackage.Split
{
    public class SpliterV1
    {
        private readonly SplitConfig splitConfig;

        private readonly BcRuleEntity bcRuleEntity;

        private readonly List<RelatedItem> logisticsRelated;

        private readonly List<Logistic> logistics;

        public SpliterV1(List<SplitPackage.Business.Logistic> logistics, List<SplitPackage.Business.LogisticRelated> logisticRelateds)
        {
            this.logistics = logistics.Select(o=> new Logistic(o)).ToList();
            this.logisticsRelated = logisticRelateds.Select(o => new RelatedItem()
            {
                ID = o.RelatedName,
                Logistics = o.Items.Select(oi=>oi.LogisticBy.LogisticCode).ToList()
            }).ToList();
            this.splitConfig = new SplitConfig();
            splitConfig.Initialize(this.logistics.SelectMany(o=>o.RuleSequenceDic.Select(oi=>oi.Value)).ToList());
            this.bcRuleEntity = new BcRuleEntity();
            var subLevelDic = this.logistics.SelectMany(o => o.RuleSequenceDic.SelectMany(oi => oi.Value.Rule.MixRule.SelectMany(oii => oii.RuleItems.Select(oiii=>oiii.PTId)))).Distinct().ToDictionary(o => o, o => new SubLevel()
            {
                PTId = o,
                PostTaxRate = 0,
                BcTaxRate = 0
            });
            bcRuleEntity.Initialize(new BcConfig() { StepWeight = 1}, subLevelDic);
        }

        /// <summary>
        /// 默认拆单方法
        /// 1:价格最便宜
        /// 2:速度最快
        /// 3:最少拆单数
        /// </summary>
        /// <param name="productList">待拆单商品列表</param>
        /// <param name="splitType">拆单方法</param>
        public SplitedOrder Split(string orderId, List<Product> productList, int totalQuantity, int splitType)
        {
            try
            {
                var pel = ConvertToProductEntity(productList);
                var result = SplitOrder(orderId, pel.Item1, pel.Item2, totalQuantity, (SplitPrinciple)splitType).GenerateSubOrderId();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                throw;
            }
        }

        public SplitedOrder SplitWithOrganization1(string orderId, List<Product> productList, int totalQuantity, List<string> logistics)
        {
            try
            {
                var pel = ConvertToProductEntity(productList);
                List<RuleEntity> rules = new List<RuleEntity>();
                foreach (var item in logistics)
                {
                    Logistic l = this.logistics.Where(o => o.LogisticName.Equals(item)).FirstOrDefault();
                    if (l != null && l.RuleSequenceDic != null)
                    {
                        rules.AddRange(l.RuleSequenceDic.Values);
                    }
                }
                // 指定物流拆单
                var result = SplitOrderWithOrganization(orderId, pel.Item1, totalQuantity, rules);
                if (result.Item2.Count > 0)
                {
                    var secondLogistics = logisticsRelated.Where(o => o.Logistics.Any(oi => logistics.Contains(oi))).SelectMany(o => o.Logistics)
                        .Where(o => !logistics.Contains(o));
                    List<RuleEntity> secondRules = new List<RuleEntity>();
                    foreach (var item in secondLogistics)
                    {
                        Logistic l = this.logistics.Where(o=>o.LogisticName.Equals(item)).FirstOrDefault();
                        if (l != null && l.RuleSequenceDic != null)
                        {
                            secondRules.AddRange(l.RuleSequenceDic.Values);
                        }
                    }
                    var secondResult = SplitOrderWithOrganization(orderId, result.Item2, totalQuantity, secondRules);
                    result.Item1.AddSubOrderRange(secondResult.Item1);
                    if (secondResult.Item2.Count > 0)
                    {
                        // 指定物流情况下，重新调用一遍价格优先将剩余订单拆分
                        result.Item1.AddSubOrderRange(SplitOrder(orderId, secondResult.Item2, pel.Item2, totalQuantity, SplitPrinciple.PriceFirst));
                    }
                }
                result.Item1.GenerateSubOrderId();
                LogHelper.Logger.Info(string.Format("Spliter.SplitWithOrganization return:\n    {0}", result.Item1));
                return result.Item1;
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                throw;
            }
        }

        private SplitedOrder SplitOrder(string orderId, List<ProductEntity> productList, List<Product> badProductList, int totalQuantity, SplitPrinciple splitPrinciple)
        {
            Debug.Assert(splitPrinciple != SplitPrinciple.LogisticsFirst);
            var result = new SplitedOrder();
            List<List<RuleEntity>> ruleOptions = null;
            switch (splitPrinciple)
            {
                case SplitPrinciple.SpeedFirst:
                    break;

                case SplitPrinciple.QuanlityFirst:
                case SplitPrinciple.PriceFirst:
                default:
                    ruleOptions = splitConfig.GetRuleEntities(splitPrinciple, productList);
                    break;
            }

            if (ruleOptions != null && ruleOptions.Count > 0)
            {
                var splitResults = ruleOptions.Select(rules => SplitOnce(SplitPackage.Split.Common.Common.CloneProductEntityList(productList), rules, splitPrinciple)).ToList();
                var msgs = Enumerable.Repeat(string.Format("Spliter.SplitOrder({0}, {1}, {2}, {3}) alternative:", "orderId=" + orderId, "productList.Count=" + productList.Count, "totalQuantity=" + totalQuantity, "splitPrinciple=" + splitPrinciple), 1)
                    .Concat(splitResults.Select(ret => string.Format("    ({0}, {1}, {2})", ret.Item1, ret.Item2, "[" + string.Join(", ", ret.Item3) + "]")));
                LogHelper.Logger.Info(string.Join(Environment.NewLine, msgs));
                Tuple<SplitedOrder, bool, List<ProductEntity>> optimal = null;
                switch (splitPrinciple)
                {
                    case SplitPrinciple.QuanlityFirst:
                        optimal = splitResults.OrderBy(t => t.Item2 ? 0 : 1)
                            .ThenBy(t => t.Item1.OrderList.Count)
                            .ThenBy(t => t.Item1.CalculateLogisticsAndTaxCost())
                            .FirstOrDefault();
                        break;

                    case SplitPrinciple.PriceFirst:
                    case SplitPrinciple.LogisticsFirst:
                    default:
                        optimal = splitResults.OrderBy(t => t.Item2 ? 0 : 1)
                            .ThenBy(t => t.Item1.CalculateLogisticsAndTaxCost())
                            .ThenBy(t => t.Item1.OrderList.Count)
                            .FirstOrDefault();
                        break;
                }
                result = optimal != null ? optimal.Item1 : result;
            }
            else
            {
                // BC方式，计算跨境综合税
                result = bcRuleEntity.Split(productList);
            }

            if (badProductList != null && badProductList.Any())
            {
                var subOrder = new SubOrder("-2", null, null, null, null, badProductList)
                {
                    LogisticsUnitPrice = int.MaxValue,
                    LogisticsCost = int.MaxValue,
                    TaxCost = int.MaxValue,
                };
                result.AddSubOrder(subOrder);
            }

            result.OrderId = orderId;
            return result;
        }

        private Tuple<SplitedOrder, bool, List<ProductEntity>> SplitOnce(List<ProductEntity> productList, List<RuleEntity> rules, SplitPrinciple splitPrinciple)
        {
            var splitedOrder = new SplitedOrder();

            var restProductList = productList;
            for (int i = 0; i < rules.Count && restProductList.Count > 0; i++)
            {
                var tuple = rules[i].Split(restProductList, splitPrinciple, false);
                splitedOrder.AddSubOrderRange(tuple.Item1);
                restProductList = tuple.Item2;
            }

            bool isTax = false;
            if (splitPrinciple != SplitPrinciple.LogisticsFirst && restProductList.Count > 0)
            {
                isTax = true;
                splitedOrder.AddSubOrderRange(bcRuleEntity.Split(restProductList));
                restProductList.Clear();
            }
            return Tuple.Create(splitedOrder, isTax, restProductList);
        }

        private Tuple<SplitedOrder, List<ProductEntity>> SplitOrderWithOrganization(string orderId, List<ProductEntity> productList, int totalQuantity, List<RuleEntity> ruleList)
        {
            // 指定物流时，此处传入的RuleEntity清单仅为该物流规则
            Debug.Assert(ruleList != null);
            var ruleOptions = SplitConfig.GetRuleEntities(ruleList, productList);
            var splitResults = ruleOptions.Select(rules => SplitOnce(SplitPackage.Split.Common.Common.CloneProductEntityList(productList), rules, SplitPrinciple.LogisticsFirst)).ToList();
            LogHelper.Logger.Info(string.Format("SplitOrderWithOrganization({0}, {1}, {2}, {3}) alternative:", "orderId=" + orderId, "productList.Count=" + productList.Count, "totalQuantity=" + totalQuantity, "ruleList=" + (ruleList != null ? ruleList.Count.ToString() : "<null>")));
            splitResults.ForEach(ret => LogHelper.Logger.Info(string.Format("    {0}", ret)));
            var optimal = splitResults.OrderBy(t => t.Item2 ? 0 : 1).ThenBy(t => t.Item1.CalculateLogisticsAndTaxCost()).FirstOrDefault();
            var result = optimal != null ? optimal.Item1 : new SplitedOrder();
            var restProductList = optimal != null ? optimal.Item3 : productList;

            result.OrderId = orderId;
            return Tuple.Create(result, restProductList);
        }

        private Tuple<List<ProductEntity>, List<Product>> ConvertToProductEntity(List<Product> productList)
        {
            var ped = new Dictionary<string, ProductEntity>();
            List<Product> restProductList = new List<Product>();
            foreach (var p in productList)
            {
                ProductEntity pe = new ProductEntity(new ProductConfigItem()
                {
                    No = p.ProNo,
                    SKUNo = p.SkuNo,
                    PTId = p.PTId,
                    Weight = p.Weight,
                    Name = p.ProName
                });
                string ptId = p.PTId.ToString();
                if (!ped.ContainsKey(ptId))
                {
                    ProductEntity penew = pe.Clone();
                    ped.Add(ptId, penew);
                }
                p.PTId = pe.PTId;
                ped[ptId].AddProdcut(p);
            }
            return Tuple.Create(ped.Values.ToList(), restProductList);
        }
    }
}
