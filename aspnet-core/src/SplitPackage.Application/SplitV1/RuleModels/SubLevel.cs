﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace SplitPackage.SplitV1.RuleModels
{
    public class SubLevel
    {
        [XmlAttribute()]
        public string PTId { get; set; }

        [XmlAttribute()]
        public string TypeName { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        [XmlAttribute()]
        public int ClassId { get; set; }

        [XmlAttribute()]
        public string Description { get; set; }

        /// <summary>
        /// 行邮税率
        /// </summary>
        [XmlAttribute()]
        public double PostTaxRate { get; set; }

        /// <summary>
        /// BC税率
        /// </summary>
        [XmlAttribute()]
        public double BcTaxRate { get; set; }
    }
}
