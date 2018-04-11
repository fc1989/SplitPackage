using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split
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
        public int PTId { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        public int ClassId { get; set; }

        public string Brand { get; set; }

        public double Weight { get; set; }

        public List<Product> OrderInfo { get; set; }

        //public List<ProductSingleRuleEntity> ProductRuleList { get; set; }
        //private Dictionary<string, ProductSingleRuleEntity> productRuleDic;
        //private Dictionary<string, RuleEntity> ruleDic;

        public ProductEntity(ProductConfigItem productItem)
        {
            this.No = productItem.No;
            this.SKUNo = productItem.SKUNo;
            this.Name = productItem.Name;
            this.PTId = productItem.PTId;
            this.ClassId = productItem.ClassId;
            this.Brand = productItem.Brand;
            this.Weight = productItem.Weight;
            this.OrderInfo = new List<Product>();
            //this.ProductRuleList = new List<ProductSingleRuleEntity>();

            //this.productRuleDic = new Dictionary<string, ProductSingleRuleEntity>();
            //this.ruleDic = new Dictionary<string, RuleEntity>();
        }

        private ProductEntity() { }

        //public void AddRule(ProductSingleRuleEntity prodRule)
        //{
        //    if (prodRule == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    if (prodRule.PTId != this.PTId)
        //    {
        //        throw new ArgumentException(string.Format("传入参数有误，传入的ProductRuleEntity对象的PTId值[{0}]与本ProductEntity实例的PTId[{1}]不一致",
        //            prodRule.PTId.ToString(), this.PTId.ToString()));
        //    }

        //    if (!this.ProductRuleList.Contains(prodRule))
        //    {
        //        this.ProductRuleList.Add(prodRule);
        //    }
        //    if (this.productRuleDic.ContainsKey(prodRule.Key))
        //    {
        //        this.productRuleDic.Add(prodRule.Key, prodRule);
        //    }

        //    if ((prodRule.Rule != null) && (this.ruleDic.ContainsKey(prodRule.Rule.Key)))
        //    {
        //        this.ruleDic.Add(prodRule.Rule.Key, prodRule.Rule);
        //    }
        //}

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
            // Rule List不拷贝
            //result.ProductRuleList = ProductRuleList;
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
