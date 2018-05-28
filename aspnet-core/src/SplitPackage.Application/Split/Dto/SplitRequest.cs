using System.Collections.Generic;

namespace SplitPackage.Split.Dto
{
    public class SplitRequest: BaseRequest
    {
        /// <summary>
        /// 拆单方式
        /// </summary>
        public int Type { get; set; }

        //public override string ToString()
        //{
        //    return "SplitRequest{" +
        //        "OrderId=" + this.OrderId +
        //        ", ProList=(Count=" + this.ProList.Count + ")[" + string.Join(", ", this.ProList) + "]" +
        //        ", TotalQuantity=" + this.TotalQuantity +
        //        ", Type=" + this.Type +
        //        "}";
        //}
    }
}