using SplitPackage.Split.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.Split
{
    public class SplitConfig
    {
        private List<RuleEntity> ruleEntityList = null;

        //private Dictionary<SplitPrinciple, List<List<RuleEntity>>> ruleEntitiesDic = new Dictionary<SplitPrinciple, List<List<RuleEntity>>>();

        public void Initialize(List<RuleEntity> ruleEntityList)
        {
            this.ruleEntityList = ruleEntityList;

            //this.ruleEntitiesDic.Add(SplitPrinciple.PriceFirst,
            //    new List<List<RuleEntity>>
            //    {
            //        ruleEntityList.OrderBy(re => re.Rule.Price).ToList(),
            //        ruleEntityList.OrderBy(re => re.Rule.Price / re.Rule.StepWeight).ToList(),
            //        ruleEntityList.OrderBy(re => re.Rule.StartingPrice / re.Rule.StartingWeight).ToList(),
            //        ruleEntityList.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.MaxQuantity) : -1).ToList(),
            //        ruleEntityList.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.MaxWeight) : -1).ToList(),
            //        ruleEntityList.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.TaxThreshold) : -1).ToList(),
            //        //ruleEntityList.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.MaxPrice) : -1).ToList(),
            //        ruleEntityList.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.LimitedQuantity) : -1) : -1).ToList(),
            //        ruleEntityList.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.LimitedWeight) : -1) : -1).ToList(),
            //        ruleEntityList.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.TaxThreshold) : -1) : -1).ToList(),
            //        //ruleEntityList.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.LimitedMaxPrice) : -1) : -1).ToList(),
            //        //ruleEntityList.OrderBy(re => re.Rule.PriceLevel).ToList(),
            //    });

            ////this.ruleEntitiesDic.Add(SplitPrinciple.SpeedFirst, );

            //this.ruleEntitiesDic.Add(SplitPrinciple.QuanlityFirst, this.ruleEntitiesDic[SplitPrinciple.PriceFirst]);

            ////this.ruleEntitiesDic.Add(SplitPrinciple.LogisticsFirst, Enumerable.Repeat(ruleEntityList.OrderBy(re => re.Rule.Price).ToList(), 1).ToList());
        }

        // 根据拆分规则取得全体Rule列表(排序后的)
        //public List<List<RuleEntity>> GetRuleEntities(SplitPrinciple splitPrinciple)
        //{
        //    return ruleEntitiesDic.ContainsKey(splitPrinciple) ? ruleEntitiesDic[splitPrinciple] : ruleEntitiesDic[SplitPrinciple.PriceFirst];
        //}

        // 根据拆分规则及商品列表取得可能的Rule列表(排序后的)(商品列表中任一商品都不支持的Rule不返回)
        public List<List<RuleEntity>> GetRuleEntities(SplitPrinciple splitPrinciple, List<ProductEntity> pel)
        {
            return GetRuleEntities(ruleEntityList, pel);
            //return GetRuleEntities(splitPrinciple)
            //    .Select(l => l.Where(re => pel.Any(pe => re.GetRuleDic().ContainsKey(pe.PTId.ToString()))).ToList())    // 只留下能支持当前商品的Rule组
            //    .Where(l => l.Count > 0)                                                                                // 空Rule就不要了
            //    .Distinct(new ListComparer<RuleEntity>())                                                               // 去除没用的Rule后就有可能有重复了，重复的去掉
            //    .ToList();
        }

        /// <summary>
        /// 指定物流拆单时
        /// </summary>
        /// <param name="ruleEntities"></param>
        /// <param name="pel"></param>
        /// <returns></returns>
        public static List<List<RuleEntity>> GetRuleEntities(List<RuleEntity> ruleEntities, List<ProductEntity> pel)
        {
            return getPermutation(ruleEntities.Where(re => pel.Any(pe => re.GetRuleDic().ContainsKey(pe.PTId.ToString()))).ToList(), pel);    // 只留下能支持当前商品的Rule组

            //var ruleEntitiesList = new List<List<RuleEntity>>
            //    {
            //        ruleEntities.OrderBy(re => re.Rule.Price).ToList(),
            //        ruleEntities.OrderBy(re => re.Rule.Price / re.Rule.StepWeight).ToList(),
            //        ruleEntities.OrderBy(re => re.Rule.StartingPrice / re.Rule.StartingWeight).ToList(),
            //        //ruleEntityList.OrderBy(re => re.Rule.PriceLevel).ToList(),
            //    };

            //return ruleEntitiesList.Select(l => l.Where(re => pel.Any(pe => re.GetRuleDic().ContainsKey(pe.PTId.ToString()))).ToList())    // 只留下能支持当前商品的Rule组
            //    .Where(l => l.Count > 0)                                                                                // 空Rule就不要了
            //    .Distinct(new ListComparer<RuleEntity>())                                                               // 去除没用的Rule后就有可能有重复了，重复的去掉
            //    .ToList();
        }

        private static List<List<RuleEntity>> getPermutation(List<RuleEntity> ruleEntities, List<ProductEntity> pel)
        {
            var ptids = pel.Select(pe => pe.PTId).ToList();
            var totalCount = pel.Sum(pe => pe.OrderInfo.Sum(p => p.Quantity));
            return new List<List<RuleEntity>>
            {
                ruleEntities.OrderBy(re => re.Rule.Price).ToList(),
                ruleEntities.OrderBy(re => re.Rule.Price / re.Rule.StepWeight).ToList(),
                ruleEntities.OrderBy(re => re.Rule.StartingPrice / re.Rule.StartingWeight).ToList(),
                ruleEntities.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.MaxQuantity) : -1).ToList(),
                ruleEntities.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.MaxWeight) : -1).ToList(),
                ruleEntities.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.TaxThreshold) : -1).ToList(),
                //ruleEntities.OrderByDescending(re => re.SingleRuleDic.Count > 0 ? re.SingleRuleDic.Max(kv => kv.Value.MaxPrice) : -1).ToList(),
                ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => MaxLimitedQuantity(kv.Value, ptids)) : -1).ToList(),
                ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => MaxLimitedWeight(kv.Value, ptids)) : -1).ToList(),
                ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => MaxTaxThreshold(kv.Value, ptids)) : -1).ToList(),
                ptids.Count == 1 ? ruleEntities.OrderByDescending(re => CompareMixRuleMinMaxQuantity(re, ptids[0], totalCount)).ToList() : new List<RuleEntity>(),
                //ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.)

                //ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.LimitedMaxPrice) : -1) : -1).ToList(),
                //ruleEntities.OrderBy(re => re.Rule.PriceLevel).ToList(),
                
                //ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.LimitedQuantity) : -1) : -1).ToList(),
                //ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.LimitedWeight) : -1) : -1).ToList(),
                //ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Max(kv => kv.Value.Count > 0 ? kv.Value.Max(pmre => pmre.TaxThreshold) : -1) : -1).ToList(),
                //ruleEntities.OrderByDescending(re => re.MixRuleDic.Count > 0 ? re.MixRuleDic.Select(kv => kv.Value.Where(pmre => IsValid(pmre, ptids)).ToList()).Max(pmres => pmres.Count > 0 ? pmres.Max(pmre => pmre.LimitedQuantity) : -1) : -1).ToList(),
            }
            //return getPermutation(ruleEntities)
                .Where(l => l != null && l.Count > 0)                    // 空Rule就不要了
                .Distinct(new ListComparer<RuleEntity>())   // 去除没用的Rule后就有可能有重复了，重复的去掉
                .ToList();
        }

        /// <summary>
        /// 特殊规则，混装规则里正好能装下的优先
        /// </summary>
        /// <param name="re"></param>
        /// <param name="ptid"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private static int CompareMixRuleMinMaxQuantity(RuleEntity re, int ptid, int totalCount)
        {
            if (!re.MixRuleDic.ContainsKey(ptid))
            {
                return -10;
            }

            var ret = 0;
            foreach (var pmre in re.MixRuleDic[ptid])
            {
                var ruleItems = pmre.GetRuleItemList(ptid);
                foreach (var ruleItem in ruleItems)
                {
                    if (ruleItem.MaxQuantity == totalCount && ruleItem.MinQuantity == totalCount)
                    {
                        return 10;
                    }
                    if (ret < 9 && ruleItem.MaxQuantity == totalCount)
                    {
                        ret = 9;
                    }
                    if (ret < 8 && ruleItem.MaxQuantity > totalCount)
                    {
                        ret = 8;
                    }
                }
            }

            return ret;
        }

        private static int MaxLimitedQuantity(List<ProductMixedRuleEntity> pmres, List<int> ptids)
        {
            var validPmres = pmres.Where(pmre => IsValid(pmre, ptids)).ToList();
            return validPmres.Count > 0 ? validPmres.Max(pmre => pmre.LimitedQuantity) : -1;
        }

        private static int MaxLimitedWeight(List<ProductMixedRuleEntity> pmres, List<int> ptids)
        {
            var validPmres = pmres.Where(pmre => IsValid(pmre, ptids)).ToList();
            return validPmres.Count > 0 ? validPmres.Max(pmre => pmre.LimitedWeight) : -1;
        }

        private static decimal MaxTaxThreshold(List<ProductMixedRuleEntity> pmres, List<int> ptids)
        {
            var validPmres = pmres.Where(pmre => IsValid(pmre, ptids)).ToList();
            return validPmres.Count > 0 ? validPmres.Max(pmre => pmre.TaxThreshold) : -1;
        }

        private static bool IsValid(ProductMixedRuleEntity pmre, List<int> ptids)
        {
            return (ptids != null && ptids.Count > 0) ? pmre.CanSupportPTId(ptids) : true;
        }

        private static IEnumerable<List<RuleEntity>> getPermutation(List<RuleEntity> source)
        {
            if (source.Count == 1)
            {
                yield return source;
            }
            else
            {
                for (int i = 0; i < source.Count; i++)
                {
                    foreach (var p in getPermutation(source.Take(i).Concat(source.Skip(i + 1)).ToList()))
                    {
                        yield return source.Skip(i).Take(1).Concat(p).ToList();
                    }
                }
            }
        }
    }
}
