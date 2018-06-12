using System.Collections.Generic;

namespace SplitPackage.Split.Dto
{
    public class SplitRequest: BaseRequest
    {
        /// <summary>
        /// 拆单方式
        /// </summary>
        public int Type { get; set; }
    }
}