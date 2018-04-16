using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("Logistics")]
    public class Logistic : FullAuditedEntity<long>, IPassivable, IMustHaveTenant
    {
        public const int MaxCorporationNameLength = 50;
        public const int MaxCorporationUrlLength = 200;
        public const int MaxLogisticFlagLength = 50;

        /// <summary>
        /// 企业名称
        /// </summary>
        public string CorporationName { get; set; }

        /// <summary>
        /// 企业网址
        /// </summary>
        public string CorporationUrl { get; set; }

        /// <summary>
        /// 物流标识
        /// </summary>
        public string LogisticFlag { get; set; }

        public bool IsActive { get; set; }

        public int TenantId { get; set; }
    }
}
