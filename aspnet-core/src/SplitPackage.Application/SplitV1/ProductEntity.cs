using SplitPackage.Split.SplitModels;
using SplitPackage.SplitV1.RuleModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.SplitV1
{
    public class ProductEntity
    {
        /// <summary>
        /// 货号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// SKU编码
        /// </summary>
        public string SKUNo { get; set; }

        /// <summary>
        /// 货物名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品子类别ID
        /// </summary>
        public string PTId { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        public int ClassId { get; set; }

        public string Brand { get; set; }

        public double Weight { get; set; }

        public List<Product> OrderInfo { get; set; }

        public void AddProdcut(Product product)
        {
            this.OrderInfo.Add(product);
        }

        internal ProductEntity Clone()
        {
            ProductEntity result = new ProductEntity();
            result.Name = Name;
            result.No = No;
            result.SKUNo = SKUNo;
            result.Brand = Brand;
            result.ClassId = ClassId;
            result.Weight = Weight;
            result.PTId = PTId;
            result.OrderInfo = CloneProductList(OrderInfo);

            return result;
        }

        private List<Product> CloneProductList(List<Product> sources)
        {
            if (sources == null)
            {
                return null;
            }

            List<Product> result = new List<Product>();
            foreach (Product p in sources)
            {
                Product np = p.Clone();
                result.Add(np);
            }

            return result;
        }

        public override string ToString()
        {
            return "ProductEntity{" +
                "No=" + this.No +
                ", SKUNo=" + this.SKUNo +
                ", Name=" + this.Name +
                ", PTId=" + this.PTId +
                ", ClassId=" + this.ClassId +
                ", Brand=" + this.Brand +
                ", Weight=" + this.Weight +
                ", OrderInfo=(Count=" + this.OrderInfo.Count + ")[" + string.Join(", ", this.OrderInfo) + "]" +
                //", ProductRuleList.Count=" + this.ProductRuleList.Count +
                //", productRuleDic={Count=" + this.productRuleDic.Count + ", Keys=[" + string.Join(", ", this.productRuleDic.Keys) + "])" +
                //", ruleDic={Count=" + this.ruleDic.Count + ", Keys=[" + string.Join(", ", this.ruleDic.Keys) + "]}" +
                "}";
        }
    }
}
