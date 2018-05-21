using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels
{
    public class MixRule
    {
        [XmlAttribute]
        public int MRId { get; set; }
        //[XmlAttribute]
        //public int ClassAId { get; set; }
        //[XmlAttribute]
        //public int ClassBId { get; set; }
        [XmlAttribute]
        public int LimitedQuantity { get; set; }
        //[XmlAttribute]
        //public int LimitedQuantityA { get; set; }
        [XmlAttribute]
        public double LimitedWeight { get; set; }
        //[XmlAttribute]
        //public double LimitedUnitPrice { get; set; }
        [XmlAttribute]
        public double TaxThreshold { get; set; }
        //[XmlAttribute]
        //public double TaxRate { get; set; }
        [XmlAttribute]
        public double LimitedMaxPrice { get; set; }

        [XmlArray("MixRuleItems"), XmlArrayItem("RuleItem")]
        public List<RuleItem> RuleItems { get; set; }
    }
}
