using System.Xml.Serialization;

namespace SplitPackage.Split.RuleModels
{
    public class ProductConfigItem
    {
        /// <summary>
        /// 货号
        /// </summary>
        [XmlAttribute()]
        public string No { get; set; }

        /// <summary>
        /// SKU编码
        /// </summary>
        [XmlAttribute()]
        public string SKUNo { get; set; }

        /// <summary>
        /// 商品子类别ID
        /// </summary>
        [XmlAttribute()]
        public int PTId { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        [XmlAttribute()]
        public int ClassId { get; set; }

        [XmlAttribute()]
        public double Weight { get; set; }

        [XmlAttribute()]
        public string Brand { get; set; }

        /// <summary>
        /// 货物名称
        /// </summary>
        [XmlAttribute()]
        public string Name { get; set; }
    }
}
