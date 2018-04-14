using System.Collections.Generic;

namespace SplitPackage.Split.Dto
{
    public class SplitWithExpRequest : BaseRequest
    {
        /// <summary>
        /// 物流公司名称
        /// </summary>
        public string LogisticsName { get; set; }
        /// <summary>
        /// 选定的物流级别（经济型/标准型）
        /// </summary>
        public string GradeName { get; set; }

        public override string ToString()
        {
            return "SplitRequest{" +
                "UserName=" + this.UserName +
                ", OrderId=" + this.OrderId +
                ", ProList=(Count=" + this.ProList.Count + ")[" + string.Join(", ", this.ProList) + "]" +
                ", TotalQuantity=" + this.TotalQuantity +
                ", LogisticsName=" + this.LogisticsName +
                ", GradeName=" + this.GradeName +
                "}";
        }
    }
}