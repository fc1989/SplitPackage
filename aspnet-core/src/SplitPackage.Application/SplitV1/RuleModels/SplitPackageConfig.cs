using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.SplitV1.RuleModels
{
    public class SplitPackageConfig
    {
        [XmlAttribute()]
        public string OrganizationId { get; set; }

        [XmlAttribute()]
        public String OrganizationName { get; set; }

        [XmlAttribute()]
        public String URL { get; set; }

        [XmlAttribute()]
        public String LogoURL { get; set; }
    }
}
