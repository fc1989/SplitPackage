using System.Collections.Generic;

namespace SplitPackage.Split.SplitModels
{
    /// <summary>
    /// 物流信息
    /// </summary>
    public class LogisticsModel
    {
        /// <summary>
        /// 物流编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 物流公司名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 物流公司规则
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// 物流公司主页/物流查询地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 是否分等级（标准类、经济类）
        /// </summary>
        public List<string> GradeList { get; set; }

        /// <summary>
        /// 物流公司LOGO的URL
        /// </summary>
        public string LogoURL { get; set; }

        public bool HasMultiGrade
        {
            get
            {
                return ((GradeList != null) && (GradeList.Count > 1));
            }
        }

        public LogisticsModel()
        {
            this.GradeList = new List<string>();
        }
    }
}