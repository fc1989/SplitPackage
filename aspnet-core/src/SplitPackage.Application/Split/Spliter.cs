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
    public class Spliter
    {
        /// <summary>
        /// 拆单规则配置集合
        /// </summary>
        private List<SplitPackageConfig> ruleConfigs = null;
        /// <summary>
        /// 商品配置集合
        /// </summary>
        private ProductConfig productConfig = null;

        /// <summary>
        /// 物流公司实例集合，含拆单方法
        /// Key：物流ID + GradeName
        /// Value：物流实例
        /// </summary>
        private Dictionary<string, Logistic> logisticcDic = null;

        /// <summary>
        /// 物流公司子业务集合
        /// Key：RuleID
        /// Value：RuleSequence对象
        /// </summary>
        private Dictionary<string, RuleEntity> ruleSequenceDic = null;
        private List<RuleEntity> ruleEntityList = null;

        /// <summary>
        /// 物流列表（不包括海关），查询物流清单接口用
        /// </summary>
        private List<LogisticsModel> logisticsList = null;

        public static Dictionary<string, SubLevel> TheSubLevelDic { get; set; }

        public Dictionary<string, SubLevel> SubLevelDic { get; private set; }

        private SplitConfig splitConfig = new SplitConfig();

        private BcRuleEntity bcRuleEntity = new BcRuleEntity();

        public List<SplitPackageConfig> GetRuleConfigs()
        {
            return ruleConfigs;
        }
        public ProductConfig GetProductConfig()
        {
            return productConfig;
        }

        // Key:产品SKU NO
        private Dictionary<string, ProductEntity> prodDic = new Dictionary<string, ProductEntity>();

        private List<RelatedItem> logisticsRelated;

        #region 初始化
        /// <summary>
        /// 初始化（加载配置文件）
        /// </summary>
        /// <param name="folderPath">配置文件所在路径</param>
        public void Initialize(string folderPath)
        {
            try
            {
                ruleSequenceDic = new Dictionary<string, RuleEntity>();
                ruleEntityList = new List<RuleEntity>();
                logisticcDic = new Dictionary<string, Logistic>();
                logisticsList = new List<LogisticsModel>();

                // 加载产品配置文件，并初始化
                productConfig = this.LoadProductConfig(Path.Combine(folderPath, "Product.xml"));
                if ((productConfig == null) || (productConfig.Products == null) || (productConfig.Products.Count <= 0))
                {
                    throw new ArgumentException("配置文件Product.xml有误。");
                }
                SubLevelDic = productConfig.ProductClass.SubLevels.ToDictionary(sl => sl.PTId);
                foreach (var p in productConfig.Products)
                {
                    ProductEntity pe = new ProductEntity(p);
                    if (this.prodDic.ContainsKey(p.SKUNo.Trim().ToLower()))
                    {
                        throw new ArgumentException(string.Format("SKUNO[{0}]重复配置", pe.SKUNo));
                    }
                    this.prodDic.Add(p.SKUNo.Trim().ToLower(), pe);
                }

                // 加载规则配置文件，并初始化
                ruleConfigs = this.LoadRules(Path.Combine(folderPath, "Rules"));
                foreach (SplitPackageConfig config in ruleConfigs)
                {
                    var organizations = config.SubOrganizations;
                    if ((organizations == null) || (organizations.Count < 0))
                    {
                        continue;
                    }
                    LogisticsModel logisiticsModel = new LogisticsModel()
                    {
                        ID = config.OrganizationId,
                        Name = config.OrganizationName,
                        Rule = config.RuleDiscription,
                        URL = config.URL,
                        LogoURL = config.LogoURL
                    };
                    if (logisticsList.Any(o => o.ID == logisiticsModel.ID))
                    {
                        throw new ArgumentException(string.Format("Logistics ID:{0}重复配置", logisiticsModel.ID));
                    }
                    else
                    {
                        // 海关的配置不需要，代码不处理，不在配置文件目录中放海关的规则
                        if (!Regex.IsMatch(config.OrganizationId, @"^(\-\d+|0)$"))
                        {
                            logisticsList.Add(logisiticsModel);
                        }
                    }

                    foreach (var organization in organizations)
                    {
                        logisiticsModel.GradeList.Add(organization.GradeName);
                        var rules = organization.Rules;
                        if ((rules == null) || (rules.Count < 0))
                        {
                            continue;
                        }

                        Logistic logistics = new Logistic(organization, config);
                        if (logisticcDic.ContainsKey(logistics.LogisticName))
                        {
                            throw new ArgumentException(String.Format("Logistics Name is already Added. LogisticName:[{0}]", logistics.LogisticName));
                        }
                        logisticcDic.Add(logistics.LogisticName, logistics);

                        foreach (PackageRule rule in rules)
                        {
                            if (rule == null)
                            {
                                continue;
                            }

                            RuleEntity ruleSeq = new RuleEntity(rule, config, organization);
                            ruleSequenceDic.Add(ruleSeq.Key, ruleSeq);
                            ruleEntityList.Add(ruleSeq);
                            logistics.AddRuleSequenceDic(ruleSeq);
                        }
                    }
                }

                logisticsRelated = this.LoadLogisticsRelated(Path.Combine(folderPath, "LogisticsRelated.xml"));

                CheckLevelConfig();
                splitConfig.Initialize(ruleEntityList);
                bcRuleEntity.Initialize(productConfig.ProductClass.BcConfig, SubLevelDic);

                var msgs = Enumerable.Repeat("Spliter Initialized.", 1)
                    .Concat(ruleEntityList.Select(re => string.Format("    ({0}, {1}, {2})", re.LogisticsName, re.SubOrganizationName, re.RuleName)));
                LogHelper.Logger.Info(string.Join(Environment.NewLine, msgs));
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 加载拆包规则配置文件
        /// </summary>
        /// <param name="folder">拆包规则文件存放路径</param>
        /// <returns></returns>
        public List<SplitPackageConfig> LoadRules(string folder)
        {
            LogHelper.Logger.Info(String.Format("Loading RulesFolder @[{0}]", folder));
            List<SplitPackageConfig> rules = new List<SplitPackageConfig>();
            string rulesFolder = folder;
            if (!Directory.Exists(folder))
            {
                throw new ArgumentException(string.Format("not exist folder path [{0}]", rulesFolder));
            }

            // 加载所有规则（海关、物流）
            string[] files = Directory.GetFiles(rulesFolder, "*.xml", SearchOption.AllDirectories);
            if ((files == null) || (files.Length <= 0))
            {
                throw new ArgumentException(string.Format("Can not find any Rule file in folder [{0}]", rulesFolder));
            }
            foreach (string filePath in files)
            {
                LogHelper.Logger.Info(String.Format("Loading Rule File @[{0}]", filePath));
                //string fileName = Path.GetFileName(filePath);
                SplitPackageConfig rule = XmlHelper.LoadXmlFile<SplitPackageConfig>(filePath);
                if (rule == null)
                {
                    throw new ArgumentException(string.Format("Rule file[{0}] is incorrect.", filePath));
                }
                rules.Add(rule);
            }

            return rules;
        }

        /// <summary>
        /// 加载商品配置
        /// </summary>
        /// <param name="fileName">商品配置文件路径</param>
        /// <returns></returns>
        public ProductConfig LoadProductConfig(string fileName)
        {
            LogHelper.Logger.Info(String.Format("Loading ProductConfig @[{0}]", fileName));
            if (!File.Exists(fileName))
            {
                throw new ArgumentException(string.Format("not exist file path [{0}]", fileName));
            }

            ProductConfig config = XmlHelper.LoadXmlFile<ProductConfig>(fileName);
            if ((config != null) && (config.Products != null))
            {
                LogHelper.Logger.Info(String.Format("Loading ProductConfig Completed. Loaded Products Count is [{0}]", config.Products.Count.ToString()));
            }
            return config;
        }

        public List<RelatedItem> LoadLogisticsRelated(string fileName)
        {
            LogHelper.Logger.Info(String.Format("Loading LogisticsRelated @[{0}]", fileName));
            if (!File.Exists(fileName))
            {
                return new List<RelatedItem>();
            }

            var relates = XmlHelper.LoadXmlFile<List<RelatedItem>>(fileName);
            if (relates != null && relates.Count > 0)
            {
                LogHelper.Logger.Info(String.Format("Loading LogisticsRelated Completed. Loaded Products Count is [{0}]", relates.Count));
            }
            return relates;
        }
        #endregion

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
                LogHelper.Logger.Info(string.Format("Spliter.Split return:\n    {0}", result));
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Info(ex.Message, ex);
                throw;
            }
        }

        public SplitedOrder SplitWithOrganization1(string orderId, List<Product> productList, int totalQuantity, List<RuleEntity> relst)
        {
            try
            {
                var pel = ConvertToProductEntity(productList, false);
                // 指定物流拆单
                var result = SplitOrderWithOrganization(orderId, pel.Item1, totalQuantity, relst);
                if (result.Item2.Count > 0)
                {
                    // 指定物流情况下，重新调用一遍价格优先将剩余订单拆分
                    result.Item1.AddSubOrderRange(SplitOrder(orderId, result.Item2, pel.Item2, totalQuantity, SplitPrinciple.PriceFirst));
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

        public SplitedOrder SplitWithOrganization1(string orderId, List<Product> productList, int totalQuantity, List<string> logistics)
        {
            try
            {
                var pel = ConvertToProductEntity(productList, false);
                List<RuleEntity> rules = new List<RuleEntity>();
                foreach (var item in logistics)
                {
                    Logistic l = this.GetLogisticcDic()[Logistic.GetLogisticName(item, "标准型")];
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
                        Logistic l = this.GetLogisticcDic()[Logistic.GetLogisticName(item, "标准型")];
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

        /// <summary>
        /// 按照指定物流拆单
        /// </summary>
        /// <param name="productNoList">待拆单货号列表</param>
        /// <param name="OrganizationId">物流ID</param>
        public SplitedOrder SplitWithOrganization(string orderId, List<Product> productList, int totalQuantity, string logisticsName, string gradeName)
        {
            try
            {
                string key = Logistic.GetLogisticName(logisticsName, gradeName);
                if (!this.logisticcDic.ContainsKey(key))
                {
                    LogHelper.Logger.Error(string.Format("指定物流(logisticsName[{0}], gradeName[{1}])不存在：", logisticsName, gradeName));
                    return this.Split(orderId, productList, totalQuantity, (int)SplitPrinciple.PriceFirst);
                }

                var logistics = this.logisticcDic[key];
                if (logistics.RuleSequenceDic == null)
                {
                    LogHelper.Logger.Error(string.Format("物流(logisticsName[{0}], gradeName[{1}])的规则配置为NULL。", logistics.LogisticName, gradeName));
                    return this.Split(orderId, productList, totalQuantity, (int)SplitPrinciple.PriceFirst);
                }

                // 读取该指定物流的规则清单
                var relst = logistics.RuleSequenceDic.Values.ToList();
                var pel = ConvertToProductEntity(productList);
                // 指定物流拆单
                var result = SplitOrderWithOrganization(orderId, pel.Item1, totalQuantity, relst);
                if (result.Item2.Count > 0)
                {
                    // 指定物流情况下，重新调用一遍价格优先将剩余订单拆分
                    result.Item1.AddSubOrderRange(SplitOrder(orderId, result.Item2, pel.Item2, totalQuantity, SplitPrinciple.PriceFirst));
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
                    // BC
                    break;

                //case SplitPrinciple.LogisticsFirst:
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
                // 没拆完，按含税方式拆一遍
                //for (int i = 0; i < rules.Count && restProductList.Count > 0; i++)
                //{
                //    var tuple = rules[i].Split(restProductList, splitPrinciple, true);
                //    isTax |= tuple.Item1.OrderList.Count > 0;
                //    splitedOrder.AddSubOrderRange(tuple.Item1);
                //    restProductList = tuple.Item2;
                //}

                //if (restProductList.Count > 0)
                {
                    // 还有剩余就按BC拆
                    isTax = true;
                    //var subOrder = new SubOrder("-1", null, null, null, null, restProductList.SelectMany(pe => pe.OrderInfo).ToList())
                    //{
                    //    LogisticsUnitPrice = decimal.MaxValue,
                    //    LogisticsCost = decimal.MaxValue,
                    //    TaxCost = decimal.MaxValue,
                    //};
                    //splitedOrder.AddSubOrder(subOrder);
                    splitedOrder.AddSubOrderRange(bcRuleEntity.Split(restProductList));
                    restProductList.Clear();
                }
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

        private Tuple<SplitedOrder, List<ProductEntity>> SplitOnceWithOrganization(List<ProductEntity> productList, List<RuleEntity> rules, SplitPrinciple splitPrinciple)
        {
            var splitedOrder = new SplitedOrder();

            var restProductList = productList;
            for (int i = 0; i < rules.Count && restProductList.Count > 0; i++)
            {
                var tuple = rules[i].Split(restProductList, splitPrinciple, false);
                splitedOrder.AddSubOrderRange(tuple.Item1);
                restProductList = tuple.Item2;
            }

            return Tuple.Create(splitedOrder, restProductList);
        }

        private Tuple<List<ProductEntity>, List<Product>> ConvertToProductEntity(List<Product> productList, bool getPTId = true)
        {
            var ped = new Dictionary<string, ProductEntity>();
            List<Product> restProductList = new List<Product>();
            // 将Product转换为ProductEntity
            foreach (var p in productList)
            {
                ProductEntity pe;
                string ptId;
                if (getPTId)
                {
                    if (!prodDic.ContainsKey(p.SkuNo.Trim().ToLower()))
                    {
                        // 未知商品，放到一个特定的子订单中
                        restProductList.Add(p);
                        LogHelper.Logger.Error(string.Format("无法识别的SkuNo[{0}]", p.SkuNo));
                        continue;
                    }
                    pe = prodDic[p.SkuNo.Trim().ToLower()];
                    ptId = pe.PTId.ToString();
                }
                else
                {
                    pe = new ProductEntity(new ProductConfigItem()
                    {
                        No = p.ProNo,
                        SKUNo = p.SkuNo,
                        PTId = p.PTId,
                        Weight = p.Weight,
                        Name = p.ProName
                    });
                    ptId = p.PTId.ToString();
                }

                // 商品归类，相同PTId商品合并
                if (!ped.ContainsKey(ptId))
                {
                    ProductEntity penew = pe.Clone();
                    ped.Add(ptId, penew);
                }

                p.PTId = pe.PTId;
                if (p.Weight == 0)
                {
                    LogHelper.Logger.Info(string.Format("Product's weight is zero: {0}, set to [{1}]", p, (int)pe.Weight), new ArgumentException("Product.Weight"));
                    p.Weight = (int)pe.Weight;
                }
                ped[ptId].AddProdcut(p);
            }
            return Tuple.Create(ped.Values.ToList(), restProductList);
        }

        private void CheckLevelConfig()
        {
            var subLevelDic = SubLevelDic;
            var dupSingleRulePTIdList = new List<Tuple<string, string, string, string>>();
            var invalidSingleRulePTIdList = new List<Tuple<string, string, string, string>>();
            var invalidSingleRuleLevelList = new List<Tuple<string, string, string, string, string>>();
            var dupMixedRulePTIdLevelList = new List<Tuple<string, string, string, int, string, string>>();
            var invalidMixedRulePTIdList = new List<Tuple<string, string, string, int, string>>();
            var invalidMixedRuleLevelList = new List<Tuple<string, string, string, int, string, string>>();
            ruleConfigs.ForEach(config => config.SubOrganizations.ForEach(org => org.Rules.ForEach(pr =>
            {
                dupSingleRulePTIdList.AddRange(pr.SinglePackRule.GroupBy(sr => sr.PTId).Where(g => g.Count() > 1).Select(g => g.Key).Distinct().Select(ptid => Tuple.Create(config.OrganizationName, org.GradeName, string.Format("{0}({1})", pr.Id, pr.SubBusinessName), ptid)));
                invalidSingleRulePTIdList.AddRange(pr.SinglePackRule.Where(sr => !subLevelDic.ContainsKey(sr.PTId)).Select(sr => Tuple.Create(config.OrganizationName, org.GradeName, string.Format("{0}({1})", pr.Id, pr.SubBusinessName), sr.PTId)));
                invalidSingleRuleLevelList.AddRange(
                    pr.SinglePackRule.Where(sr => subLevelDic.ContainsKey(sr.PTId))
                        .SelectMany(sr => sr.Levels.Where(pl => !subLevelDic[sr.PTId].SubLevelItems.Any(l => l.Name.Equals(pl.Name))).Select(pl => Tuple.Create(config.OrganizationName, org.GradeName, string.Format("{0}({1})", pr.Id, pr.SubBusinessName), sr.PTId, pl.Name))));

                pr.MixRule.ForEach(mr =>
                {
                    dupMixedRulePTIdLevelList.AddRange(mr.RuleItems.GroupBy(ri => Tuple.Create(ri.PTId, ri.LevelName ?? string.Empty)).Where(g => g.Count() > 1).Select(g => g.Key).Distinct().Select(ptidLevel => Tuple.Create(config.OrganizationName, org.GradeName, string.Format("{0}({1})", pr.Id, pr.SubBusinessName), mr.MRId, ptidLevel.Item1, ptidLevel.Item2)));
                    invalidMixedRulePTIdList.AddRange(mr.RuleItems.Where(ri => !subLevelDic.ContainsKey(ri.PTId)).Select(ri => Tuple.Create(config.OrganizationName, org.GradeName, string.Format("{0}({1})", pr.Id, pr.SubBusinessName), mr.MRId, ri.PTId)));
                    invalidMixedRuleLevelList.AddRange(
                        mr.RuleItems.Where(ri => !string.IsNullOrEmpty(ri.LevelName) && subLevelDic.ContainsKey(ri.PTId) && !subLevelDic[ri.PTId].SubLevelItems.Any(l => l.Name.Equals(ri.LevelName)))
                            .Select(ri => Tuple.Create(config.OrganizationName, org.GradeName, string.Format("{0}({1})", pr.Id, pr.SubBusinessName), mr.MRId, ri.PTId, ri.LevelName)));
                });
            })));

            if (dupSingleRulePTIdList.Count > 0 || invalidSingleRulePTIdList.Count > 0 || invalidSingleRuleLevelList.Count > 0
                || dupMixedRulePTIdLevelList.Count > 0 || invalidMixedRulePTIdList.Count > 0 || invalidMixedRuleLevelList.Count > 0)
            {
                var msgs = new[] {
                    (dupSingleRulePTIdList.Count > 0) ? "单独装Rule的PTId重复：[" + string.Join(", ", dupSingleRulePTIdList) + "]" : null,
                    (invalidSingleRulePTIdList.Count > 0) ? "单独装Rule无效的PTId:[" + string.Join(", ", invalidSingleRulePTIdList) + "]" : null,
                    (invalidSingleRuleLevelList.Count > 0) ? "单独装Rule无效的LevelName:[" + string.Join(", ", invalidSingleRuleLevelList) + "]" : null,
                    (dupMixedRulePTIdLevelList.Count > 0) ? "混装Rule的Level重复：[" + string.Join(", ", dupMixedRulePTIdLevelList) + "]" : null,
                    (invalidMixedRulePTIdList.Count > 0) ? "混装Rule无效的PTId:[" + string.Join(", ", invalidMixedRulePTIdList) + "]" : null,
                    (invalidMixedRuleLevelList.Count > 0) ? "混装Rule无效的LevelName:[" + string.Join(", ", invalidMixedRuleLevelList) + "]" : null,
                };
                throw new ArgumentException(string.Join("\n\n", msgs.Where(msg => !string.IsNullOrEmpty(msg))));
            }
        }

        //private SplitedOrder SplitOrder(string orderId, List<Product> productList, int totalQuantity, RuleEntity rs)
        //{
        //    if (rs == null)
        //    {
        //        return null;
        //    }

        //    var result = new SplitedOrder();
        //    result.OrderId = orderId;
        //    result.OrderList = new List<SubOrder>();

        //    int subOrderIndex = 0;

        //    if (totalQuantity <= 3)
        //    {
        //        string subOrderId = string.Format("{0}{1:00}", orderId, ++subOrderIndex);
        //        SubOrder so = new SubOrder(orderId, rs.LogisticsName, rs.Organization.URL, rs.SubOrganizationName, rs.Rule.SubBusinessName, productList);
        //        so.LogisticsUnitPrice = rs.Rule.Price;
        //        so.LogisticsCost = rs.CalculateFreight(so.TotalWeight);
        //        result.OrderList.Add(so);
        //    }
        //    else
        //    {
        //        // 将各商品依次取出1个的规则，将全部商品以3个为一组进行分组
        //        int cnt = 0;
        //        int subOrderNum = (int)(totalQuantity / 3) + (((totalQuantity % 3) > 0) ? 1 : 0);
        //        int j = 0;
        //        int totalCnt = totalQuantity;
        //        for (int i = 0; i < subOrderNum; i++)
        //        {
        //            int[] pQuanlitys = new int[productList.Count];
        //            int loopCnt = subOrderNum * productList.Count;
        //            while ((cnt < 3) && (totalCnt > 0))
        //            {
        //                SplitModels.Product p = productList[j % productList.Count];
        //                if (p.Quantity > 0)
        //                {
        //                    pQuanlitys[j % productList.Count] += p.Remove(1);
        //                    cnt++;
        //                    totalCnt--;
        //                }
        //                j++;
        //            }

        //            // 一组商品达到上限总数为3个的条件，生成一个子订单
        //            List<SplitModels.Product> package = new List<SplitModels.Product>();
        //            for (int k = 0; k < pQuanlitys.Length; k++)
        //            {
        //                if (pQuanlitys[k] > 0)
        //                {
        //                    var rp = productList[k].Clone();
        //                    rp.Quantity = pQuanlitys[k];
        //                    package.Add(rp);
        //                }
        //            }

        //            string subOrderId = string.Format("{0}{1:00}", orderId, ++subOrderIndex);
        //            SubOrder so = new SubOrder(subOrderId, rs.LogisticsName, rs.Organization.URL, rs.SubOrganizationName, rs.Rule.SubBusinessName, package);
        //            so.LogisticsUnitPrice = rs.Rule.Price;
        //            so.LogisticsCost = rs.CalculateFreight(so.TotalWeight);

        //            result.OrderList.Add(so);

        //            cnt = 0;
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// 获取所有物流公司信息清单
        /// </summary>
        /// <returns>物流公司清单</returns>
        public List<LogisticsModel> GetLogisticsList()
        {
            return logisticsList;
        }

        public Dictionary<string, Logistic> GetLogisticcDic()
        {
            return logisticcDic;
        }
    }
}
