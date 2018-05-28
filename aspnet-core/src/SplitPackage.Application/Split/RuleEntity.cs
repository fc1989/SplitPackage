using Abp.Logging;
using SplitPackage.Split.Common;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplitPackage.Split
{
    public class RuleEntity
    {
        public string Key { get; private set; }

        /// <summary>
        /// 拆包规则，最小粒度
        /// </summary>
        public PackageRule Rule { get; private set; }
        public int RuleId { get; private set; }
        /// <summary>
        /// SubBusinessName
        /// </summary>
        public string RuleName { get; private set; }
        public bool HasMixRule { get; private set; }

        /// <summary>
        /// 所属物流
        /// </summary>
        public SplitPackageConfig Organization { get; private set; }
        /// <summary>
        /// 所属物流ID
        /// </summary>
        public string LogisticsId { get; private set; }
        public string LogisticsName { get; private set; }

        /// <summary>
        /// 在所属物流的子机构实例
        /// </summary>
        public Organization SubOrganization { get; private set; }
        /// <summary>
        /// 在所属物流的子机构的名称
        /// </summary>
        public string SubOrganizationName { get; private set; }

        /// <summary>
        /// Key：商品IDPTID   Value：IProductRuleEntity集合
        /// </summary> 
        private Dictionary<string, List<IProductRuleEntity>> pRuleDic = new Dictionary<string, List<IProductRuleEntity>>();

        public Dictionary<string, ProductSingleRuleEntity> SingleRuleDic = new Dictionary<string, ProductSingleRuleEntity>();
        public Dictionary<string, List<ProductMixedRuleEntity>> MixRuleDic = new Dictionary<string, List<ProductMixedRuleEntity>>();

        public RuleEntity(SplitPackage.Business.LogisticChannel logisticChannel)
        {
            this.LogisticsName = logisticChannel.LogisticBy.CorporationName;
            this.RuleName = logisticChannel.ChannelName;
            this.Rule = new PackageRule()
            {
                Id = (int)logisticChannel.Id,
                SubBusinessName = logisticChannel.ChannelName,
                StartingPrice = logisticChannel.WeightFreights.First().StartingPrice,
                StartingWeight = logisticChannel.WeightFreights.First().StartingWeight,
                Price = logisticChannel.WeightFreights.First().Price,
                StepWeight = (int)logisticChannel.WeightFreights.First().StepWeight,
                MixRule = logisticChannel.SplitRules.Select(o=>new MixRule() {
                    MRId = (int)o.Id,
                    LimitedQuantity = o.MaxPackage,
                    LimitedWeight = o.MaxWeight,
                    TaxThreshold = o.MaxTax,
                    LimitedMaxPrice = o.MaxPrice,
                    RuleItems = o.ProductClasses.Select(oi=>new RuleItem() {
                        PTId = oi.PTId,
                        MinQuantity = oi.MinNum,
                        MaxQuantity = oi.MaxNum
                    }).ToList()
                }).OrderBy(o=>o.MRId).ToList()
            };
            this.Organization = new SplitPackageConfig()
            {
                OrganizationId = logisticChannel.LogisticBy.LogisticCode,
                OrganizationName = logisticChannel.LogisticBy.CorporationName,
                URL = logisticChannel.LogisticBy.CorporationUrl,
            };
            this.InitSplitRule();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="organization"></param>
        /// <param name="subOrganization"></param>
        public RuleEntity(PackageRule rule, SplitPackageConfig organization, Organization subOrganization)
        {
            if ((rule == null) || (organization == null) || (subOrganization == null))
            {
                throw new ArgumentNullException("非法参数");
            }
            this.Rule = rule;
            this.RuleId = rule.Id;
            this.RuleName = rule.SubBusinessName;
            this.HasMixRule = (rule.MixRule.Count > 0);

            this.Organization = organization;
            this.LogisticsId = Organization.OrganizationId;
            this.LogisticsName = Organization.OrganizationName;

            this.SubOrganization = subOrganization;
            this.SubOrganizationName = SubOrganization.GradeName;

            this.Key = string.Format("{0}${1}${2}", this.LogisticsId, this.SubOrganizationName, this.RuleId);

            this.InitSplitRule();
        }

        public Dictionary<string, List<IProductRuleEntity>> GetRuleDic()
        {
            return this.pRuleDic;
        }

        /// <summary>
        /// 初始化商品拆单规则，构建以商品ID为Key，拆单规则对象为Value的字典
        /// 其中专线的Value为单个对象，
        ///     混装的Value为集合，对应一类商品可以和多类不同商品混装的情况
        /// </summary>
        private void InitSplitRule()
        {
            if (this.Rule == null)
            {
                return;
            }

            //Dictionary<int, ProductSingleRuleEntity> singleRuleDic = new Dictionary<int, ProductSingleRuleEntity>();
            //Dictionary<int, List<ProductMixedRuleEntity>> mixRuleDic = new Dictionary<int, List<ProductMixedRuleEntity>>();
            //this.pRuleDic = new Dictionary<string, List<IProductRuleEntity>>();

            if (this.Rule.SinglePackRule != null)
            {
                foreach (var singleRule in this.Rule.SinglePackRule)
                {
                    if (SingleRuleDic.ContainsKey(singleRule.PTId))
                    {
                        throw new ArgumentException(string.Format("配置文件异常。 PTId:[{1}] in Rule[{0}].SinglePackRules ", this.Key, singleRule.PTId));
                    }

                    var psre = new ProductSingleRuleEntity(singleRule, this);
                    SingleRuleDic.Add(singleRule.PTId, psre);

                    // 将商品规则添加到商品规则字典中
                    if (pRuleDic.ContainsKey(singleRule.PTId.ToString()))
                    {
                        pRuleDic[singleRule.PTId.ToString()].Add(psre);
                    }
                    else
                    {
                        var ipsrelist = new List<IProductRuleEntity>();
                        ipsrelist.Add(psre);
                        pRuleDic.Add(singleRule.PTId.ToString(), ipsrelist);
                    }
                }
            }

            if (this.Rule.MixRule != null)
            {
                foreach (var mixRule in this.Rule.MixRule)
                {
                    var pmre = new ProductMixedRuleEntity(mixRule, this);
                    foreach (var ruleItem in mixRule.RuleItems)
                    {
                        //循环混装规则中的商品ID（PTId）
                        if (MixRuleDic.ContainsKey(ruleItem.PTId))
                        {
                            MixRuleDic[ruleItem.PTId].Add(pmre);
                        }
                        else
                        {
                            var pmreList = new List<ProductMixedRuleEntity>();
                            pmreList.Add(pmre);
                            MixRuleDic.Add(ruleItem.PTId, pmreList);
                        }

                        // 将商品规则添加到商品规则字典中
                        if (pRuleDic.ContainsKey(ruleItem.PTId.ToString()))
                        {
                            pRuleDic[ruleItem.PTId.ToString()].Add(pmre);
                        }
                        else
                        {
                            var ipmrelist = new List<IProductRuleEntity>();
                            ipmrelist.Add(pmre);
                            pRuleDic.Add(ruleItem.PTId.ToString(), ipmrelist);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public decimal CalculateFreight(int weight)
        {
            decimal result = (decimal)this.Rule.StartingPrice;
            if (this.Rule.StartingWeight < weight)
            {
                if ((this.Rule.StepWeight == 0))
                {
                    throw new ArgumentException(String.Format("Config is Invalid. @[{0}] : Rule.StepWeight is 0.", this.LogisticsName, this.RuleName));
                }

                long rem = 0;
                long div = Math.DivRem((long)(weight - this.Rule.StartingWeight), (long)this.Rule.StepWeight, out rem);
                result += (((rem > 0) ? div + 1 : div) * (decimal)this.Rule.Price);
            }

            return result;
        }

        // TODO(liuxin)(算法升级)：把所有可能全返回，外层在此基础上再跑，最终把所有可能全集中到一起判断最优解
        public Tuple<SplitedOrder, List<ProductEntity>> Split(List<ProductEntity> productList, /*int totalQuantity, */SplitPrinciple splitPrinciple, bool withTax)
        {
            var result = Tuple.Create(new SplitedOrder(), productList);
            if ((productList == null) || (productList.Count <= 0) || splitPrinciple == SplitPrinciple.SpeedFirst)
            {
                // 无单可拆，直接返回
                return result;
            }

            var productListCount = productList.Count;
            var resultGroups = getEntities(productList).Select(l => Split(SplitPackage.Split.Common.Common.CloneProductEntityList(productList), l, withTax)).ToList();
            var msgs = Enumerable.Repeat(string.Format("RuleEntity.Split({0}, {1}, {2}) alternative:", "productList.Count=" + productListCount, "splitPrinciple=" + splitPrinciple, "withTax=" + withTax), 1)
                .Concat(resultGroups.Select(ret => string.Format("    ({0}, {1})", ret.Item1, "[" + string.Join(", ", ret.Item2) + "]")));
            LogHelper.Logger.Info(string.Join(Environment.NewLine, msgs));
            var resultGroup = resultGroups.GroupBy(t => t.Item2.Count).OrderBy(g => g.Key).FirstOrDefault();
            if (resultGroup != null)
            {
                switch (splitPrinciple)
                {
                    case SplitPrinciple.QuanlityFirst:
                        result = resultGroup.OrderBy(t => t.Item1.OrderList.Count).FirstOrDefault() ?? result;
                        break;

                    case SplitPrinciple.PriceFirst:
                    case SplitPrinciple.LogisticsFirst:
                    default:
                        result = resultGroup.OrderBy(t => t.Item1.CalculateLogisticsCost()).FirstOrDefault() ?? result;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 根据给予的组合规则进行拆单操作
        /// </summary>
        /// <param name="productList">订单商品明细</param>
        /// <param name="rules">按层面整合的符合规则</param>
        /// <param name="withTax">是否收税</param>
        /// <returns>可拆解的订单,未能拆解的订单</returns>
        private Tuple<SplitedOrder, List<ProductEntity>> Split(List<ProductEntity> productList, List<List<IProductRuleEntity>> rules, bool withTax)
        {
            var result = new SplitedOrder();
            var restProductList = productList;
            for (int i = 0; i < rules.Count && restProductList.Count > 0; i++)
            {
                var l = rules[i];
                for (int j = 0; j < l.Count && restProductList.Count > 0; j++)
                {
                    var r = l[j];
                    while (restProductList.Count > 0)
                    {
                        var so = r.Split(restProductList, withTax);
                        result.AddSubOrderRange(so.Item1);
                        if (so.Item1.OrderList.Count == 0 || so.Item2.Count <= 0)
                        {
                            // 拆不出来，此规则不适用 || 全拆完了
                            break;
                        }
                        restProductList = so.Item2;
                    }
                }
            }
            result.OrderList.ForEach(o => o.LogisticsCost = this.CalculateFreight(o.CalculateTotalWeight()));
            return Tuple.Create(result, restProductList);
        }

        // 取得所有可能的排列
        private List<List<List<IProductRuleEntity>>> getEntities(List<ProductEntity> productList)
        {
            var allPTIds = new HashSet<string>(productList.Select(pe => pe.PTId));
            var singleRulePermutation = getPermutation(getSingleRuleEntityCombinations(allPTIds, productList)).ToList();
            var mixedRulePermutation = getPermutation(getMixedRuleEntityCombinations(allPTIds, productList)).ToList();

            // 先单后混或先混后单，为减少计算量不做单混随意组合
            if (!singleRulePermutation.Any())
            {
                return mixedRulePermutation;
            }
            if (!mixedRulePermutation.Any())
            {
                return singleRulePermutation;
            }
            return singleRulePermutation.SelectMany(s => mixedRulePermutation.Select(m => s.Concat(m).ToList()).ToList())
                    .Concat(mixedRulePermutation.SelectMany(m => singleRulePermutation.Select(s => m.Concat(s).ToList()))).ToList();
        }

        // 取得单装组合
        private List<List<IProductRuleEntity>> getSingleRuleEntityCombinations(HashSet<string> allPTIds, List<ProductEntity> productList)
        {
            var ruleEntities = SingleRuleDic.Where(kv => allPTIds.Contains(kv.Key)).Select(kv => kv.Value).ToList();
            return new[]
            {
                ruleEntities.OrderByDescending(psre => psre.MaxQuantity).Cast<IProductRuleEntity>().ToList(),         // 按件数，件数多的优先
                ruleEntities.OrderByDescending(psre => psre.MaxWeight).Cast<IProductRuleEntity>().ToList(),           // 按重量，重量大的优先
            }.Where(l => l.Count > 0).Distinct(new ListComparer<IProductRuleEntity>()).ToList();
        }

        // 取得混装组合
        private List<List<IProductRuleEntity>> getMixedRuleEntityCombinations(HashSet<string> allPTIds, List<ProductEntity> productList)
        {
            var ruleEntities = MixRuleDic.Where(kv => allPTIds.Contains(kv.Key)).SelectMany(kv => kv.Value).ToList();
            return new[]
            {
                ruleEntities.OrderByDescending(pmre => pmre.LimitedQuantity).Cast<IProductRuleEntity>().ToList(),         // 按件数，件数多的优先
                ruleEntities.OrderByDescending(pmre => pmre.LimitedWeight).Cast<IProductRuleEntity>().ToList(),           // 按重量，重量大的优先
                ruleEntities.OrderByDescending(pmre => pmre.RuleItems.Where(o=>allPTIds.Contains(o.PTId)).Count()).Cast<IProductRuleEntity>().ToList(),// 按混装匹配都接近的优先
                //ruleEntities.OrderByDescending(pmre => pmre.RuleItems.Where(mrie => allPTIds.Contains(mrie.PTId)).Count()).Cast<IProductRuleEntity>().ToList(), // 按混装种类，当前商品种类装得多的优先，一种混装就能把所有商品全装完的最优先
            }.Where(l => l.Count > 0).Distinct(new ListComparer<IProductRuleEntity>()).ToList();
        }

        // 全排列
        private IEnumerable<List<List<IProductRuleEntity>>> getPermutation(List<List<IProductRuleEntity>> source)
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

    /// <summary>
    /// 拆单原则
    /// </summary>
    public enum SplitPrinciple
    {
        /// <summary>
        /// 价格优先原则，低价最优
        /// </summary>
        PriceFirst = 1,
        /// <summary>
        /// 速度优先原则，快速最优
        /// </summary>
        SpeedFirst = 2,
        /// <summary>
        /// 包裹数量优先原则，量少最优
        /// </summary>
        QuanlityFirst = 3,
        /// <summary>
        /// 物流优先原则，指定物流
        /// </summary>
        LogisticsFirst = 4,
    }
}