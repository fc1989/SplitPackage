using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Abp.Logging;
using SplitPackage.Cache.Dto;
using SplitPackage.SplitV1.RuleModels;
using SplitPackage.Split.SplitModels;

namespace SplitPackage.SplitV1
{
    public class Spliter
    {
        private readonly SplitConfig splitConfig;

        private readonly List<RelatedItem> logisticsRelated;

        private readonly List<Logistic> logistics;

        public Spliter(IList<LogisticCacheDto> ownLogistics, IList<IList<LogisticRelatedOptionCacheDto>> relateds)
        {
            this.logistics = ownLogistics.Select(o => new Logistic(o)).ToList();
            this.logisticsRelated = relateds.Select(o => new RelatedItem()
            {
                Logistics = o.Select(oi => oi.LogisticCode).ToList()
            }).ToList();
            this.splitConfig = new SplitConfig();
            splitConfig.Initialize(this.logistics.SelectMany(o => o.RuleSequenceDic.Select(oi => oi.Value)).ToList());
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
                var splitResults = ruleOptions.Select(rules => SplitOnce(SplitPackage.SplitV1.Common.Common.CloneProductEntityList(productList), rules, splitPrinciple)).ToList();
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
                result = this.ReturnRemainPackage(productList);
            }
            if (badProductList != null && badProductList.Any())
            {
                var subOrder = new SubOrder("-2", null, null, null, null, null, badProductList)
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
                splitedOrder.AddSubOrderRange(this.ReturnRemainPackage(restProductList));
                restProductList.Clear();
            }
            return Tuple.Create(splitedOrder, isTax, restProductList);
        }

        private Tuple<SplitedOrder, List<ProductEntity>> SplitOrderWithOrganization(string orderId, List<ProductEntity> productList, int totalQuantity, List<RuleEntity> ruleList)
        {
            // 指定物流时，此处传入的RuleEntity清单仅为该物流规则
            Debug.Assert(ruleList != null);
            var ruleOptions = SplitConfig.GetRuleEntities(ruleList, productList);
            var splitResults = ruleOptions.Select(rules => SplitOnce(SplitPackage.SplitV1.Common.Common.CloneProductEntityList(productList), rules, SplitPrinciple.LogisticsFirst)).ToList();
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
                ProductEntity pe = new ProductEntity()
                {
                    No = p.ProNo,
                    SKUNo = p.SkuNo,
                    Name = p.ProName,
                    PTId = p.PTId,
                    OrderInfo = new List<Product>()
                };
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

        private SplitedOrder ReturnRemainPackage(List<ProductEntity> remainProducts)
        {
            var result = new SplitedOrder();
            var productList = remainProducts.SelectMany(o => o.OrderInfo).ToList();
            var invalidSubOrder = new SubOrder()
            {
                Id = "-1",
                TotalWeight = productList.Sum(o=>o.Weight * o.Quantity),
                TotalPrice = productList.Sum(o=>o.ProPrice * o.Quantity),
                ProList = productList
            };
            result.AddSubOrder(invalidSubOrder);
            return result;
        }
    }
}
