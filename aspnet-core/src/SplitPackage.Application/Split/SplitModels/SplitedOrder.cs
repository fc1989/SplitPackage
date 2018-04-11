using System.Collections.Generic;
using System.Linq;

namespace SplitPackage.Split.SplitModels
{
    /// <summary>
    /// 拆包返回
    /// </summary>
    public class SplitedOrder
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 子订单列表
        /// </summary>
        public List<SubOrder> OrderList { get; set; }

        public SplitedOrder()
        {
            this.OrderList = new List<SubOrder>();
        }

        public void AddSubOrderRange(SplitedOrder subOrders)
        {
            if (this.OrderList == null)
            {
                this.OrderList = new List<SubOrder>();
            }
            if ((subOrders != null) && (subOrders.OrderList != null))
                this.OrderList.AddRange(subOrders.OrderList);
        }

        public void AddSubOrder(SubOrder subOrder)
        {
            if (this.OrderList == null)
            {
                this.OrderList = new List<SubOrder>();
            }

            if (subOrder != null)
                this.OrderList.Add(subOrder);
        }

        public decimal CalculateLogisticsAndTaxCost()
        {
            return CalculateLogisticsCost() + CalculateTaxCost();
        }

        public decimal CalculateLogisticsCost()
        {
            return this.OrderList.Sum(o => o.LogisticsCost);
        }

        public decimal CalculateTaxCost()
        {
            return this.OrderList.Sum(o => o.TaxCost);
        }

        public SplitedOrder GenerateSubOrderId()
        {
            int index = 0;
            this.OrderList.Where(o => string.IsNullOrEmpty(o.Id)).ToList().ForEach(o => o.Id = string.Format("{0}{1:00}", this.OrderId, ++index));
            return this;
        }

        public override string ToString()
        {
            return "SplitedOrder{" +
                "OrderId=" + this.OrderId +
                ", LogisticsCost=" + this.CalculateLogisticsCost() +
                ", TaxCost=" + this.CalculateTaxCost() +
                ", OrderList=(Count=" + this.OrderList.Count  + ")[" + string.Join(", ", this.OrderList) + "]" +
                "}";
        }
    }
}