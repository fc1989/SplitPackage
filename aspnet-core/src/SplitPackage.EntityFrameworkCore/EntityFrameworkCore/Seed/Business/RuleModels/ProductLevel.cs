using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels
{
    public class ProductLevel
    {
        [XmlAttribute()]
        public string Name { get; set; }
        //[XmlAttribute()]
        //public string Property { get; set; }
        //[XmlAttribute()]
        //public double BaselineFloor { get; set; }
        //[XmlAttribute()]
        //public double BaselineUpper { get; set; }
        [XmlAttribute()]
        public int MaxQuantity { get; set; }
        [XmlAttribute()]
        public int MaxWeight { get; set; }
        [XmlAttribute()]
        public double MaxPrice { get; set; }
    }
}
