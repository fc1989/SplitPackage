using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SplitPackage.RuleModels
{
    public class RelatedItem
    {
        [XmlAttribute]
        public string ID { get; set; }

        [XmlArray]
        [XmlArrayItem("Logistic")]
        public List<string> Logistics { get; set; }
    }
}
