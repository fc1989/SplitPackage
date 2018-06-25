using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplitPackage.Split
{
    public class ProductSingleRuleEntity : IProductRuleEntity
    {

        // 货号/条码
        public string PTId { get; set; }

        public String TypeName { get; set; }
        /// <summary>
        /// 商品类别ID
        /// </summary>
        public int ClassId { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        public String ClassName { get; set; }

        /// <summary>
        /// 品牌白名单，仅此名单中包含的品牌可运，为空则表示无品牌限制
        /// 多个品牌用|分割
        /// </summary>
        public String WhiteBrands { get; set; }
        /// <summary>
        /// 品牌黑名单，此名单中品牌属于禁运品牌，为空则表示无品牌限制
        /// 多个品牌用|分割
        /// </summary>
        public String BlackBrands { get; set; }

        /// <summary>
        /// 最大数量
        /// </summary>
        public int MaxQuantity { get; set; }

        /// <summary>
        /// 最大重量
        /// </summary>
        public int MaxWeight { get; set; }

        public double MinUnitPrice { get; set; }

        public decimal TaxThreshold { get; set; }

        //public int TaxRate { get; set; }

        /// <summary>
        /// 超过退运
        /// </summary>
        public decimal MaxPrice { get; set; }
        public String SingleOnly { get; set; }
        public List<Spec> Specs { get; set; }
        public List<ProductLevel> Levels { get; set; }

        public string Key { get; private set; }

        public RuleEntity Rule { get; private set; }

        /// <summary>
        /// 父节点，拆单规则
        /// </summary>
        /// <param name="rule"></param>
        public ProductSingleRuleEntity(SingleRule singleRule, RuleEntity rule)
        {
            this.Rule = rule;
            this.Key = string.Format("{0}${1}", rule.Key, this.PTId);

            this.PTId = singleRule.PTId;
            this.BlackBrands = singleRule.BlackBrands;
            //this.ClassId = Int32.Parse(singleRule.ClassId);
            //this.ClassName = singleRule.ClassName;
            this.Levels = singleRule.Levels;
            this.MaxPrice = (decimal)singleRule.MaxPrice;
            this.MaxQuantity = singleRule.MaxQuantity;
            this.MaxWeight = (int)singleRule.MaxWeight;
            this.MinUnitPrice = singleRule.MinUnitPrice;
            //this.SingleOnly = singleRule.SingleOnly;
            //this.Specs = productType.Specs;
            //this.TaxRate = (int)singleRule.TaxRate;
            this.TaxThreshold = (decimal)singleRule.TaxThreshold;
            this.TypeName = singleRule.TypeName;
            this.WhiteBrands = singleRule.WhiteBrands;
        }

        public new ProductRuleType GetType()
        {
            return ProductRuleType.Single;
        }

        // 拆包，把当前Rule能装的包全拆
        public Tuple<SplitedOrder, List<ProductEntity>> Split(List<ProductEntity> productList, bool withTax)
        {
            var result = new SplitedOrder();

            var index = productList.FindIndex(pe => pe.PTId == this.PTId);
            if (index >= 0)
            {
                var pe = productList[index];
                var restProducts = new List<Product>();
                var subOrder = CreateSubOrder();
                for (int j = 0; j < pe.OrderInfo.Count; j++)
                {
                    var restProduct = pe.OrderInfo[j];
                    while (restProduct.Quantity > 0)
                    {
                        var pl = FindSubLevel(restProduct);
                        var quantityLimit = (pl != null ? pl.MaxQuantity : this.MaxQuantity) - subOrder.CalculateTotalQuantity();
                        var weightLimit = (pl != null ? pl.MaxWeight : this.MaxWeight) - subOrder.CalculateTotalWeight();
                        var priceLimit = (withTax ? this.MaxPrice : (pl != null ? (decimal)pl.MaxPrice : this.TaxThreshold)) - subOrder.CalculateTotalPrice();
                        var productTuple = SplitProduct(restProduct, quantityLimit, weightLimit, priceLimit);
                        if (productTuple.Item1.Quantity <= 0)    // 一个都分配不出去，要么是当前Rule不满足，要么是当前SubOrder满了，测试一下是哪种情况
                        {
                            var testProduct = SplitProduct(restProduct, this.MaxQuantity, this.MaxWeight, this.MaxPrice);
                            if (testProduct.Item1.Quantity <= 0)    // Rule不满足
                            {
                                break;
                            }
                            else    // 满了，换下一个子包
                            {
                                result.AddSubOrder(subOrder);
                                subOrder = CreateSubOrder();
                            }
                        }
                        else    // 分配了部分或全部
                        {
                            subOrder.ProList.Add(productTuple.Item1);
                            restProduct = productTuple.Item2;
                            if (restProduct.Quantity > 0 || subOrder.CalculateTotalQuantity() >= this.MaxQuantity)    // 当前子包满了(有剩余，或者子包的数量达到上限)
                            {
                                result.AddSubOrder(subOrder);
                                subOrder = CreateSubOrder();
                            }
                        }
                    }
                    if (restProduct.Quantity > 0)    // 如果分配不出去了，但还有剩下，那放回原队列，分配给下一家物流
                    {
                        restProducts.Add(restProduct);
                    }
                }

                if (subOrder.ProList.Count > 0)
                {
                    result.AddSubOrder(subOrder);
                    subOrder = CreateSubOrder();
                }

                if (restProducts.Any())    // 最后还有剩余，把剩余放回队列
                {
                    productList[index].OrderInfo = restProducts;
                }
                else    // 没有剩余，从队列移除
                {
                    productList.RemoveAt(index);
                }
            }

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
                ? subOrder.ProList.Sum(p => p.CalculateTotalPrice() * (decimal)Spliter.TheSubLevelDic[p.PTId].PostTaxRate)
                : 0;
            //price <= this.TaxThreshold ? 0 : price * this.TaxRate / 100;
        }

        private Tuple<Product, Product> SplitProduct(Product product, int quantityLimit, int weightLimit, decimal priceLimit)
        {
            var splitProduct = product.Clone();
            var restProduct = product.Clone();

            var quantity = new int[] { quantityLimit, weightLimit / product.Weight, (int)(priceLimit / product.ProPrice) }.Min();
            splitProduct.Quantity = Math.Min(product.Quantity, quantity);
            restProduct.Quantity = product.Quantity - splitProduct.Quantity;

            return Tuple.Create(splitProduct, restProduct);
        }

        private SubOrder CreateSubOrder()
        {
            var result = new SubOrder("",this.Rule.LogisticsId, this.Rule.LogisticsName, this.Rule.Organization.URL, this.Rule.SubOrganizationName, this.Rule.RuleName);
            result.LogisticsUnitPrice = (decimal)this.Rule.Rule.Price;
            return result;
        }

        private ProductLevel FindSubLevel(Product product)
        {
            return null;
            //if (this.Levels == null)
            //{
            //    return null;
            //}

            //var level = Spliter.SubLevelDic[product.PTId].SubLevelItems.FirstOrDefault(l => product.ProPrice >= (decimal)l.BaselineFloor && product.ProPrice <= (decimal)l.BaselineUpper);
            //return level != null ? this.Levels.FirstOrDefault(pl => pl.Name.Equals(level.Name)) : null;
        }
    }
}
