using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Split.SplitModels
{
    public class Product
    {
        /// <summary>
        /// 货号
        /// </summary>
        public string ProNo { get; set; }
        /// <summary>
        /// SkuNo
        /// </summary>
        public string SkuNo { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 名称（可不填，测试阶段必填用以查错）
        /// </summary>
        public string ProName { get; set; }
        /// <summary>
        /// 商品单价（成本价）
        /// </summary>
        public decimal ProPrice { get; set; }
        /// <summary>
        /// 单位重量(单位：g)
        /// </summary>
        public int Weight { get; set; }

        public int? PTId { get; set; }

        public int Remove(int num)
        {
            if (this.Quantity >= num)
            {
                this.Quantity -= num;
                return num;
            }
            else
            {
                return 0;
            }
        }

        public Product Clone()
        {
            Product result = new Product();
            result.ProName = ProName;
            result.ProNo = ProNo;
            result.ProPrice = ProPrice;
            result.Quantity = Quantity;
            result.SkuNo = SkuNo;
            result.Weight = Weight;
            result.PTId = PTId;
            return result;
        }

        public decimal CalculateTotalPrice()
        {
            return this.ProPrice * this.Quantity;
        }

        public override string ToString()
        {
            return "Product{" +
                "ProNo=" + this.ProNo +
                ", SkuNo=" + this.SkuNo +
                ", Quantity=" + this.Quantity +
                ", ProName=" + this.ProName +
                ", ProPrice=" + this.ProPrice +
                ", Weight=" + this.Weight +
                ", PTId=" + this.PTId +
                "}";
        }
    }
}
