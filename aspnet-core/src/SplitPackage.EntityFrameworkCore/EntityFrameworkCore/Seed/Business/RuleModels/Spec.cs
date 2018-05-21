using System;
using System.Xml.Serialization;

namespace SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels
{
    public class Spec
    {

        [XmlAttribute()]
        public int Id { get; set; }

        [XmlAttribute()]
        public String Name { get; set; }

        [XmlAttribute()]
        public String Value { get; set; }

        [XmlAttribute()]
        public String Unit { get; set; }
    }
}
