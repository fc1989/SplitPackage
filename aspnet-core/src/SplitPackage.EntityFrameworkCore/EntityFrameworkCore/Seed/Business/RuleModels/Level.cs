using System.Xml.Serialization;

namespace SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels
{
    public class Level
    {
        [XmlAttribute()]
        public string Name { get; set; }

        [XmlAttribute()]
        public string Property { get; set; }

        /// <summary>
        /// 下限
        /// </summary>
        [XmlAttribute()]
        public double BaselineFloor { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        [XmlAttribute()]
        public double BaselineUpper { get; set; }
    }
}
