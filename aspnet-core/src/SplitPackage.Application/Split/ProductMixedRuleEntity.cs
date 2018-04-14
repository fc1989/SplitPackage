using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplitPackage.Split
{
    public class ProductMixedRuleEntity : IProductRuleEntity
    {
        public int MRId { get; set; }
        public int LimitedQuantity { get; set; }
        //
        //public int LimitedQuantityA { get; set; }
        public int LimitedWeight { get; set; }
        //
        //public double LimitedUnitPrice { get; set; }
        public decimal TaxThreshold { get; set; }
        //public int TaxRate { get; set; }
        public decimal LimitedMaxPrice { get; set; }

        public List<RuleItem> RuleItems { get; set; }

        public RuleEntity Rule { get; private set; }

        private Dictionary<Tuple<int, string>, RuleItem> ruleItemLevelDic;

        private Dictionary<int, List<RuleItem>> ruleItemDic;

        public ProductMixedRuleEntity(MixRule mixRule, RuleEntity rule)
        {
            this.Rule = rule;
            this.MRId = mixRule.MRId;
            this.LimitedQuantity = mixRule.LimitedQuantity;
            this.LimitedWeight = (int)mixRule.LimitedWeight;
            this.TaxThreshold = (decimal)mixRule.TaxThreshold;
            //this.TaxRate = (int)mixRule.TaxRate;
            this.LimitedMaxPrice = (decimal)mixRule.LimitedMaxPrice;
            this.RuleItems = mixRule.RuleItems;
            this.ruleItemLevelDic = this.RuleItems.ToDictionary(r => Tuple.Create(r.PTId, r.LevelName ?? string.Empty));
            this.ruleItemDic = this.RuleItems.GroupBy(r => r.PTId).ToDictionary(g => g.Key, g => g.ToList());
        }

        public new ProductRuleType GetType()
        {
            return ProductRuleType.Mixed;
        }

        public Tuple<SplitedOrder, List<ProductEntity>> Split(List<ProductEntity> productList, bool withTax)
        {
            var result = new SplitedOrder();

            var productEntities = productList.Where(pe => ruleItemDic.ContainsKey(pe.PTId)).ToList();
            var subOrder = SplitOneSubOrder(productEntities, withTax);
            while (subOrder != null && subOrder.ProList.Count > 0)
            {
                result.AddSubOrder(subOrder);
                subOrder = SplitOneSubOrder(productEntities, withTax);
            }

            productList.RemoveAll(pe => pe.OrderInfo.Count <= 0);

            result.OrderList.ForEach(so => so.CalculateTotalPrice());
            result.OrderList.Where(so => withTax).ToList().ForEach(so => CalculateTax(so));
            return Tuple.Create(result, productList);
        }

        /// <summary>
        /// 计算税费
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public void CalculateTax(SubOrder subOrder)
        {
            subOrder.TaxCost = (subOrder.CalculateTotalPrice() > this.TaxThreshold)
                ? subOrder.ProList.Sum(p => p.CalculateTotalPrice() * (decimal)Spliter.TheSubLevelDic[p.PTId.Value].PostTaxRate)
                : 0;

            //return price <= this.TaxThreshold ? 0 : price * this.TaxRate / 100;
        }

        public List<RuleItem> GetRuleItemList(int ptid)
        {
            return ruleItemDic.ContainsKey(ptid) ? ruleItemDic[ptid] : new List<RuleItem>();
        }

        public bool CanSupportPTId(List<int> ptids)
        {
            return ptids.Any(ptid => CanSupportPTId(ptid));
        }

        public bool CanSupportPTId(int ptid)
        {
            return ruleItemDic.ContainsKey(ptid);
        }

        /// <summary>
        /// 拆一个子订单出来
        /// </summary>
        /// <param name="peDic"></param>
        /// <returns></returns>
        private SubOrder SplitOneSubOrder(List<ProductEntity> productEntities, bool withTax)
        {
            SubOrder subOrder = CreateSubOrder();
            foreach (var productEntity in productEntities)
            {
                var splitRuleProduct = SplitRuleProduct(productEntity.OrderInfo.Where(p => p.Quantity > 0).ToList());
                var restProductList = splitRuleProduct.Item2;

                foreach (var kv in splitRuleProduct.Item1)
                {
                    var products = kv.Value;
                    if (subOrder.CalculateTotalQuantity() >= this.LimitedQuantity)
                    {
                        restProductList.AddRange(products);
                        continue;
                    }

                    var ruleItem = ruleItemLevelDic[kv.Key];
                    var quantityLimit = Math.Min(ruleItem.MaxQuantity, this.LimitedQuantity - subOrder.CalculateTotalQuantity());
                    var weightLimit = this.LimitedWeight - subOrder.CalculateTotalWeight();
                    var priceLimit = (withTax ? this.LimitedMaxPrice : this.TaxThreshold) - subOrder.CalculateTotalPrice();
                    var productsTuple = SplitProducts(products, quantityLimit, weightLimit, priceLimit, ruleItem.MinQuantity);
                    subOrder.ProList.AddRange(productsTuple.Item1);
                    restProductList.AddRange(productsTuple.Item2);
                }
                productEntity.OrderInfo = restProductList;
                if (subOrder.CalculateTotalQuantity() >= this.LimitedQuantity)
                {
                    break;
                }
            }

            return subOrder.ProList.Count > 0 ? subOrder : null;
        }

        private Tuple<List<Product>, List<Product>> SplitProducts(List<Product> products, int quantityLimit, int weightLimit, decimal priceLimit, int minQuantity)
        {
            SubOrder subOrder = CreateSubOrder();
            var restProductList = new List<Product>();
            foreach (var p in products)
            {
                if (subOrder.CalculateTotalQuantity() >= quantityLimit)
                {
                    restProductList.Add(p);
                    continue;
                }

                var qLimit = quantityLimit - subOrder.CalculateTotalQuantity();
                var wLimit = weightLimit - subOrder.CalculateTotalWeight();
                var pLimit = priceLimit - subOrder.CalculateTotalPrice();
                var productTuple = SplitProduct(p, qLimit, wLimit, pLimit);
                if (productTuple.Item1.Quantity > 0)
                {
                    subOrder.ProList.Add(productTuple.Item1);
                }
                if (productTuple.Item2.Quantity > 0)
                {
                    restProductList.Add(productTuple.Item2);
                }
            }

            if (subOrder.CalculateTotalQuantity() >= minQuantity)
            {
                return Tuple.Create(subOrder.ProList, restProductList);
            }
            else
            {
                return Tuple.Create(new List<Product>(), products);
            }
        }


        private Tuple<Product, Product> SplitProduct(Product product, int quantityLimit, int weightLimit, decimal priceLimit/*, int minQuantity*/)
        {
            var splitProduct = product.Clone();
            var restProduct = product.Clone();

            var quantity = new int[] { quantityLimit, weightLimit / product.Weight, (int)(priceLimit / product.ProPrice) }.Min();
            //quantity = quantity >= minQuantity ? quantity : minQuantity;
            splitProduct.Quantity = Math.Min(product.Quantity, quantity);
            restProduct.Quantity = product.Quantity - splitProduct.Quantity;

            return Tuple.Create(splitProduct, restProduct);
        }

        private SubOrder CreateSubOrder()
        {
            var result = new SubOrder("", this.Rule.LogisticsName, this.Rule.Organization.URL, this.Rule.SubOrganizationName, this.Rule.RuleName);
            result.LogisticsUnitPrice = (decimal)this.Rule.Rule.Price;
            return result;
        }

        private Tuple<Dictionary<Tuple<int, string>, List<Product>>, List<Product>> SplitRuleProduct(List<Product> productList)
        {
            var ruleProductDic = new Dictionary<Tuple<int, string>, List<Product>>();
            var restProductList = new List<Product>();

            productList.ForEach(p =>
            {
                var ruleItemTuple = FindRuleItem(p);
                if (ruleItemTuple != null)
                {
                    if (!ruleProductDic.ContainsKey(ruleItemTuple.Item1))
                    {
                        ruleProductDic.Add(ruleItemTuple.Item1, new List<Product>());
                    }
                    ruleProductDic[ruleItemTuple.Item1].Add(p);
                }
                else
                {
                    restProductList.Add(p);
                }
            });

            return Tuple.Create(ruleProductDic, restProductList);
        }

        private Tuple<Tuple<int, string>, RuleItem> FindRuleItem(Product product)
        {
            var key = Tuple.Create(product.PTId.Value, string.Empty);
            RuleItem value;
            if (ruleItemLevelDic.TryGetValue(key, out value))
            {
                return Tuple.Create(key, value);
            }

            var level = Spliter.TheSubLevelDic[product.PTId.Value].SubLevelItems.FirstOrDefault(l => product.ProPrice >= (decimal)l.BaselineFloor && product.ProPrice <= (decimal)l.BaselineUpper);
            if (level != null)
            {
                key = Tuple.Create(product.PTId.Value, level.Name);
                if (ruleItemLevelDic.TryGetValue(key, out value))
                {
                    return Tuple.Create(key, value);
                }
            }

            return null;
        }

        //private bool CheckLevel(Product product, RuleItem ruleItem)
        //{
        //    return string.IsNullOrEmpty(ruleItem.LevelName)
        //        ? true
        //        : Spliter.SubLevelDic[product.PTId].SubLevelItems.FirstOrDefault(l => l.Name.Equals(ruleItem.LevelName) && product.ProPrice >= (decimal)l.BaselineFloor && product.ProPrice <= (decimal)l.BaselineUpper) != null;
        //}
    }
}
