using Abp.Domain.Entities;
using Newtonsoft.Json;
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

        public ChangeFreightRule GetInformation()
        {
            ChangeFreightRule result = null;
            if (!string.IsNullOrEmpty(this.LogisticChannelChange))
            {
                result = JsonConvert.DeserializeObject<ChangeFreightRule>(this.LogisticChannelChange);
                if (result.WeightChargeRules == null)
                {
                    result.WeightChargeRules = new List<WeightFreight>();
                }
                if (result.NumChargeRules == null)
                {
                    result.NumChargeRules = new List<NumFreight>();
                }
            }
            return result;
        }
    }

    public class ChangeFreightRule
    {
        public ChangeFreightRule()
        {
            WeightChargeRules = new List<WeightFreight>();
            NumChargeRules = new List<NumFreight>();
        }

        public IEnumerable<WeightFreight> WeightChargeRules { get; set; }

        public IEnumerable<NumFreight> NumChargeRules { get; set; }
    }
}
