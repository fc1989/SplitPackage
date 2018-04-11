using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.Split.RuleModels
{
    public class SplitPackageConfig
    {
        [XmlAttribute()]
        public string OrganizationId { get; set; }

        [XmlAttribute()]
        public int OrganizationType { get; set; }

        [XmlAttribute()]
        public String OrganizationName { get; set; }

        [XmlAttribute()]
        public String URL { get; set; }

        [XmlElement()]
        public String RuleDiscription { get; set; }

        [XmlArray("SubOrganizations"), XmlArrayItem("Organization")]
        public List<Organization> SubOrganizations { get; set; }

        [XmlAttribute()]
        public String LogoURL { get; set; }
    }
}
