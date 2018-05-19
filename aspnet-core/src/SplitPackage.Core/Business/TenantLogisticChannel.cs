using Abp.Domain.Entities;
using SplitPackage.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("Tenant_LogisticChannel")]
    public class TenantLogisticChannel : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long LogisticChannelId { get; set; }

        public string AliasName { get; set; }

        public ChargeWay? Way { get; set; }

        public string LogisticChannelChange { get; set; }

        public virtual Tenant TenantBy { get; set; }

        public virtual LogisticChannel LogisticChannelBy { get; set; }
    }

    public class ChangeInformation
    {
        public IEnumerable<WeightFreight> WeightChargeRules { get; set; }

        public IEnumerable<NumFreight> NumChargeRules { get; set; }
    }
}
