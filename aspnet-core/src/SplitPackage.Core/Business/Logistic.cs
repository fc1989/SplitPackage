using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("Logistics")]
    public class Logistic : FullAuditedEntity<long>, IPassivable, IMayHaveTenant
    {
        public const int MaxCorporationNameLength = 50;
        public const int MaxCorporationUrlLength = 200;
        public const int MaxLogisticCodeLength = 50;

        /// <summary>
        /// 企业名称
        /// </summary>
        public string CorporationName { get; set; }

        /// <summary>
        /// 企业网址
        /// </summary>
        public string CorporationUrl { get; set; }

        /// <summary>
        /// 物流代码
        /// </summary>
        public string LogisticCode { get; set; }

        public bool IsActive { get; set; }

        public int? TenantId { get; set; }

        public virtual ICollection<LogisticChannel> LogisticChannels { get; set; }

        public Logistic()
        {
            IsActive = true;
        }
    }
}
