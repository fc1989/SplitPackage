using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.Split.RuleModels
{
    [XmlRoot("ProductConfig")]
    public class ProductConfig
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public ProductClass ProductClass { get; set; }


        [XmlArray("Products"), XmlArrayItem("Product")]
        public List<ProductConfigItem> Products { get; set; }
    }
}
