using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels
{
    public class PackageRule
    {
        //Id="1" SubOrganizationName="customs" StartingPrice="0" StartingWeight="0" Price="0" Priority="" 
        //PriceLevel="" SpeedLevel=""

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

        /// <summary>
        /// 规则适用优先级
        /// 0代表优先级最高，建议以10为单位累进，后期更改时可以中间随意插入
        /// </summary>
        [XmlAttribute()]
        public int Priority { get; set; }

        [XmlAttribute()]
        public int PriceLevel { get; set; }

        [XmlAttribute()]
        public int SpeedLevel { get; set; }

        [XmlArray("SinglePackRules"), XmlArrayItem("SingleRule")]
        public List<SingleRule> SinglePackRule { get; set; }

        [XmlArray("MixedPackRules"), XmlArrayItem("MixRule")]
        public List<MixRule> MixRule { get; set; }
    }
}
