using System.Xml.Serialization;

namespace SplitPackage.SplitV1.RuleModels
{
    public class BcConfig
    {
        /// <summary>
        /// BC模式的物流公司名
        /// </summary>
        [XmlAttribute()]
        public string LogisticsName { get; set; }

        /// <summary>
        /// BC模式的物流公司Grade名
        /// </summary>
        [XmlAttribute()]
        public string GradeName { get; set; }

        [XmlAttribute()]
        public string SubBusinessName { get; set; }

        [XmlAttribute()]
        public string URL { get; set; }

        /// <summary>
        /// 单一订单价格限制
        /// </summary>
        [XmlAttribute()]
        public double TotalPriceLimit { get; set; }

        /// <summary>
        /// 物流起步价格
        /// </summary>
        [XmlAttribute()]
        public double StartingPrice { get; set; }

        /// <summary>
        /// 物流起步重量
        /// </summary>
        [XmlAttribute()]
        public double StartingWeight { get; set; }

        /// <summary>
        /// 物流单价
        /// </summary>
        [XmlAttribute()]
        public double Price { get; set; }

        /// <summary>
        /// 计价重量（超过起价重量后的计价重量单位）
        /// </summary>
        [XmlAttribute()]
        public int StepWeight { get; set; }
    }
}
