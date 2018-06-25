using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.Split
{
    public class BcRuleEntity
    {
        public BcConfig BcConfig { get; private set; }

        public Dictionary<string, SubLevel> SubLevelDic { get; private set; }

        public BcRuleEntity()
        {
        }

        public void Initialize(BcConfig bcConfig, Dictionary<string, SubLevel> subLevelDic)
        {
            this.BcConfig = bcConfig;
            this.SubLevelDic = subLevelDic;
        }

        /// <summary>
        /// 先按BC方式拆单，BC拆完仍有剩余时，全部放到一个子订单中(里面的费用全设为MaxValue)
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public SplitedOrder Split(List<ProductEntity> productList/*, int totalQuantity*/)
        {
            var result = new SplitedOrder();

            var peTuple = SplitProductEntity(productList, (decimal)this.BcConfig.TotalPriceLimit);

            // 额度(2万)以后按BC拆单
            var subOrder = CreateSubOrder();
            peTuple.Item1.ForEach(pe => subOrder.ProList.AddRange(pe.OrderInfo));
            subOrder.TaxCost = CalculateTax(peTuple.Item1);
            result.AddSubOrder(subOrder);

            if (peTuple.Item2.Count > 0)
            {
                // 超过额度的不拆了，卖不了
                var invalidSubOrder = new SubOrder("-1", null, null, null, null, null, peTuple.Item2.SelectMany(pe => pe.OrderInfo).ToList())
                {
                    LogisticsUnitPrice = int.MaxValue,
                    LogisticsCost = int.MaxValue,
                    TaxCost = int.MaxValue,
                };
                result.AddSubOrder(invalidSubOrder);

            }

            result.OrderList.ForEach(so => so.LogisticsCost = this.CalculateFreight(so.CalculateTotalWeight()));
            result.OrderList.ForEach(so => so.CalculateTotalPrice());
            return result;
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public decimal CalculateFreight(int weight)
        {
            decimal result = (decimal)this.BcConfig.StartingPrice;
            if (this.BcConfig.StartingWeight < weight)
            {
                if ((this.BcConfig == null) || (this.BcConfig.StepWeight == 0))
                {
                    throw new ArgumentException("Config is Invalid. @[Product.xml] : BcConfig.StepWeight is 0.");
                }

                long rem = 0;
                long div = Math.DivRem((long)(weight - this.BcConfig.StartingWeight), (long)this.BcConfig.StepWeight, out rem);
                result += (((rem > 0) ? div + 1 : div) * (decimal)this.BcConfig.Price);
            }

            return result;
        }

        private Tuple<List<ProductEntity>, List<ProductEntity>> SplitProductEntity(List<ProductEntity> productList, decimal totalPriceLimit)
        {
            var splitProductEntity = new List<ProductEntity>();
            var restProductEntity = new List<ProductEntity>();

            foreach (var pe in productList)
            {
                if (restProductEntity.Count > 0)
                {
                    restProductEntity.Add(pe);
                    continue;
                }

                var totalPrice = CalculatePrice(splitProductEntity);
                if (totalPrice + CalculatePrice(pe) <= totalPriceLimit)
                {
                    splitProductEntity.Add(pe);
                }
                else
                {
                    var peTuple = SplitProductEntity(pe, totalPriceLimit - totalPrice);
                    if (peTuple.Item1.OrderInfo.Count > 0)
                    {
                        splitProductEntity.Add(peTuple.Item1);
                    }
                    restProductEntity.Add(peTuple.Item2);
                }
            }

            return Tuple.Create(splitProductEntity, restProductEntity);
        }
        private Tuple<ProductEntity, ProductEntity> SplitProductEntity(ProductEntity productEntity, decimal priceLimit)
        {
            var splitProductEntity = productEntity.Clone();
            splitProductEntity.OrderInfo.Clear();
            var restProductEntity = productEntity.Clone();
            restProductEntity.OrderInfo.Clear();

            foreach (var p in productEntity.OrderInfo)
            {
                if (restProductEntity.OrderInfo.Count > 0)
                {
                    restProductEntity.OrderInfo.Add(p);
                    continue;
                }

                var totalPrice = CalculatePrice(splitProductEntity);
                if (totalPrice + (p.ProPrice * p.Quantity) <= priceLimit)
                {
                    splitProductEntity.OrderInfo.Add(p);
                }
                else
                {
                    var pTuple = SplitProduct(p, priceLimit - totalPrice);
                    if (pTuple.Item1.Quantity > 0)
                    {
                        splitProductEntity.OrderInfo.Add(pTuple.Item1);
                    }
                    restProductEntity.OrderInfo.Add(pTuple.Item2);
                }
            }

            return Tuple.Create(splitProductEntity, restProductEntity);
        }

        private Tuple<Product, Product> SplitProduct(Product product, decimal priceLimit)
        {
            var splitProduct = product.Clone();
            var restProduct = product.Clone();

            var quantity = (int)(priceLimit / product.ProPrice);
            splitProduct.Quantity = Math.Min(product.Quantity, quantity);
            restProduct.Quantity = product.Quantity - splitProduct.Quantity;

            return Tuple.Create(splitProduct, restProduct);
        }

        private SubOrder CreateSubOrder()
        {
            var result = new SubOrder("", this.BcConfig.LogisticsName, this.BcConfig.LogisticsName, this.BcConfig.URL, this.BcConfig.GradeName, this.BcConfig.SubBusinessName);
            result.LogisticsUnitPrice = (decimal)this.BcConfig.Price;
            return result;
        }

        private decimal CalculateTax(List<ProductEntity> productList)
        {
            return productList.Sum(pe => pe.OrderInfo.Sum(p => p.ProPrice * p.Quantity) * (int)SubLevelDic[pe.PTId].BcTaxRate / 100);
        }

        private decimal CalculatePrice(List<ProductEntity> productList)
        {
            return productList.Sum(pe => CalculatePrice(pe));
        }

        private decimal CalculatePrice(ProductEntity productEntity)
        {
            return productEntity.OrderInfo.Sum(p => p.ProPrice * p.Quantity);
        }
    }
}
