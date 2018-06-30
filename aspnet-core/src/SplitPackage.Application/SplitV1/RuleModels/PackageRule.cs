using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace SplitPackage.SplitV1.RuleModels
{
    /// <summary>
    /// 物流线路
    /// </summary>
    public class PackageRule
    {
        [XmlAttribute()]
        public int Id { get; set; }

        [XmlAttribute()]
        public String SubBusinessName { get; set; }

        /// <summary>
        /// 物流起步价格，海关规则时固定为-1
        /// </summary>
        [XmlAttribute()]
        public double StartingPrice { get; set; }

        /// <summary>
        /// 物流起步重量，海关规则时固定为-1
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

        [XmlArray("MixedPackRules"), XmlArrayItem("MixRule")]
        public List<MixRule> MixRule { get; set; }
    }
}
