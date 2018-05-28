using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.Split.RuleModels
{
    public class SingleRule
    {
        // 货号/条码
        [XmlAttribute()]
        public string PTId { get; set; }

        [XmlAttribute()]
        public String TypeName { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        [XmlAttribute()]
        public String ClassId { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        [XmlAttribute()]
        public String ClassName { get; set; }

        /// <summary>
        /// 品牌白名单，仅此名单中包含的品牌可运，为空则表示无品牌限制
        /// 多个品牌用|分割
        /// </summary>
        [XmlAttribute()]
        public String WhiteBrands { get; set; }
        /// <summary>
        /// 品牌黑名单，此名单中品牌属于禁运品牌，为空则表示无品牌限制
        /// 多个品牌用|分割
        /// </summary>
        [XmlAttribute()]
        public String BlackBrands { get; set; }

        /// <summary>
        /// 最大数量
        /// </summary>
        [XmlAttribute()]
        public int MaxQuantity { get; set; }

        /// <summary>
        /// 最大重量
        /// </summary>
        [XmlAttribute()]
        public double MaxWeight { get; set; }

        [XmlAttribute()]
        public double MinUnitPrice { get; set; }

        [XmlAttribute()]
        public double TaxThreshold { get; set; }

        //[XmlAttribute()]
        //public double TaxRate { get; set; }

        /// <summary>
        /// 超过退运 DEATH LINE
        /// </summary>
        [XmlAttribute()]
        public double MaxPrice { get; set; }

        [XmlAttribute()]
        public String SingleOnly { get; set; }

        [XmlArray("ProductSpecs"), XmlArrayItem("Spec")]
        public List<Spec> Specs { get; set; }

        [XmlArray("SubLevelItems"), XmlArrayItem("Level")]
        public List<ProductLevel> Levels { get; set; }
    }
}
