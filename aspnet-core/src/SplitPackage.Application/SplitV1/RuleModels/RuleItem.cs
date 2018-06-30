using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SplitPackage.SplitV1.RuleModels
{
    /// <summary>
    /// 混装规则中的产品细分项
    /// </summary>
    public class RuleItem
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlAttribute]
        public string PTId { get; set; }
        /// <summary>
        /// 该商品在所在混装规则中可装入的最小数量
        /// </summary>
        [XmlAttribute]
        public int MinQuantity { get; set; }
        /// <summary>
        /// 该商品在所在混装规则中可装入的最大数量
        /// </summary>
        [XmlAttribute]
        public int MaxQuantity { get; set; }
        /// <summary>
        /// 级别名称；如：价格级别（高价、中价、低价）   重量级别（>=950g、950>x>700g、700g>）
        /// 与商品类别ID组合成唯一键，标记一类特定的商品
        /// 
        /// 目前价格是通过外部传入（因价格可变，故需外部传入），重量类等商品固有属性可配置在Product.xml中
        /// </summary>
        [XmlAttribute]
        public string LevelName { get; set; }
    }
}
