using System.Collections.Generic;
using System.Linq;

namespace SplitPackage.Split.SplitModels
{
    public class SubOrder
    {
        public SubOrder()
        {

        }

        public SubOrder(string id,string logisticsCode, string logisticsName, string url, string gradeName, string subBusinessName, List<Product> proList = null)
        {
            this.Id = id;
            this.LogisticsName = logisticsName;
            this.LogisticsCode = logisticsCode;
            this.URL = url;
            this.GradeName = gradeName;
            this.SubBusinessName = subBusinessName;
            this.ProList = proList ?? new List<Product>();

            CalculateTotalWeight();
        }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string LogisticsName { get; set; }

        /// <summary>
        /// 物流商code
        /// </summary>
        public string LogisticsCode { get; set; }

        /// <summary>
        /// 物流公司URL
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 物流级别
        /// </summary>
        public string GradeName { get; set; }
        /// <summary>
        /// 物流公司业务线名称
        /// </summary>
        public string SubBusinessName { get; set; }
        /// <summary>
        /// 物流单价
        /// </summary>
        public decimal LogisticsUnitPrice { get; set; }
        /// <summary>
        /// 订单总重量(g)
        /// </summary>
        public int TotalWeight { get; set; }
        /// <summary>
        /// 此单预估物流总价
        /// </summary>
        public decimal LogisticsCost { get; set; }

        /// <summary>
        /// 此单预估税费
        /// </summary>
        public decimal TaxCost { get; set; }

        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        public List<Product> ProList { get; set; }

        public int CalculateTotalQuantity()
        {
            return this.ProList.Sum(p => p.Quantity);
        }

        public int CalculateTotalWeight()
        {
            return this.TotalWeight = this.ProList.Sum(p => p.Weight * p.Quantity);
        }

        /// <summary>
        /// 计算该子订单的总价值
        /// </summary>
        /// <returns></returns>
        public decimal CalculateTotalPrice()
        {
            return this.TotalPrice = this.ProList.Sum(p => p.CalculateTotalPrice());
        }
        public override string ToString()
        {
            return "SubOrder{" +
                "Id=" + this.Id +
                ", LogisticsName=" + this.LogisticsName +
                ", URL=" + this.URL +
                ", GradeName=" + this.GradeName +
                ", SubBusinessName=" + this.SubBusinessName +
                ", LogisticsUnitPrice=" + this.LogisticsUnitPrice +
                ", TotalWeight=" + this.TotalWeight +
                ", LogisticsCost=" + this.LogisticsCost +
                ", TaxCost=" + this.TaxCost +
                ", TotalPrice=" + this.TotalPrice +
                ", ProList=(Count=" + this.ProList.Count  + ")[" + string.Join(", ", this.ProList) + "]" +
                "}";
        }
    }
}
