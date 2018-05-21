using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels
{
    public class Organization
    {
        [XmlAttribute()]
        public string GradeName { get; set; }

        [XmlArray("Rules"), XmlArrayItem("Rule")]
        public List<PackageRule> Rules { get; set; }
    }
}
