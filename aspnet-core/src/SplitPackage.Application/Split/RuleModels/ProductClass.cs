using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.Split.RuleModels
{
    public class ProductClass
    {
        [XmlElement()]
        public BcConfig BcConfig { get; set; }

        [XmlArray("SubLevels"), XmlArrayItem("SubLevel")]
        public List<SubLevel> SubLevels { get; set; }
    }
}
