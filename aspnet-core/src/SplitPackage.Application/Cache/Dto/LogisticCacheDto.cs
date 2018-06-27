using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SplitPackage.Business;

namespace SplitPackage.Cache.Dto
{
    public class SplitPackageSettingCache
    {
        public IList<LogisticCacheDto> OwnLogistics { get; set; }

        public IList<LogisticRelatedCacheDto> Relateds { get; set; }
    }

    public class LogisticRelatedCacheDto
    {
        public long RelatedId { get; set; }

        public IList<LogisticRelatedOptionCacheDto> Logistics { get; set; }
    }

    public class LogisticRelatedOptionCacheDto
    {
        public long LogisticId { get; set; }

        public string LogisticCode { get; set; }
    }

    public class LogisticCacheDto
    {
        public long Id { get; set; }

        public string CorporationName { get; set; }

        public string CorporationUrl { get; set; }

        public string LogoURL { get; set; }

        public string LogisticCode { get; set; }

        public IList<ChannelCacheDto> LogisticChannels { get; set; }
    }

    public class ChannelCacheDto
    {
        public long Id { get; set; }

        public string ChannelName { get; set; }

        public string AliasName { get; set; }

        public ChannelType Type { get; set; }

        public ChargeWay Way { get; set; }

        public virtual IList<SplitRuleCacheDto> SplitRules { get; set; }

        public virtual IList<WeightFreightCacheDto> WeightFreights { get; set; }

        public virtual IList<NumFreightCacheDto> NumFreights { get; set; }
    }

    public class WeightFreightCacheDto
    {
        public long Id { get; set; }

        public string Currency { get; set; }

        public string Unit { get; set; }

        public double StartingWeight { get; set; }

        public double EndWeight { get; set; }

        public double StartingPrice { get; set; }

        public double StepWeight { get; set; }

        public double CostPrice { get; set; }

        public double Price { get; set; }
    }

    public class NumFreightCacheDto
    {
        public long Id { get; set; }

        public string Currency { get; set; }

        public string Unit { get; set; }

        public int SplitNum { get; set; }

        public double FirstPrice { get; set; }

        public double CarryOnPrice { get; set; }
    }

    public class SplitRuleCacheDto
    {
        public long Id { get; set; }

        public string RuleName { get; set; }

        public int MaxPackage { get; set; }

        public double MaxWeight { get; set; }

        public double MaxTax { get; set; }

        public double MaxPrice { get; set; }

        public IList<SplitRuleProductClassCacheDto> ProductClasses { get; set; }
    }

    [AutoMapFrom(typeof(SplitRuleItem))]
    public class SplitRuleProductClassCacheDto
    {
        public long Id { get; set; }

        public string PTId { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }
    }
}
