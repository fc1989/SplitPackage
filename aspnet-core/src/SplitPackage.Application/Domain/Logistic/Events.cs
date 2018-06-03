using Abp.AutoMapper;
using Abp.Events.Bus;
using SplitPackage.Business;
using SplitPackage.Business.NumFreights.Dto;
using SplitPackage.Business.WeightFreights.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Domain.Logistic
{
    #region logistic
    [AutoMapFrom(typeof(Business.Logistic))]
    public class ModifyLogisticEvent : EventData
    {
        public long Id { get; set; }

        public string CorporationName { get; set; }

        public string CorporationUrl { get; set; }

        public string LogoURL { get; set; }

        public string LogisticCode { get; set; }

        public int? TenantId { get; set; }

        public bool IsActive { get; set; }
    }

    public class TenantModifyImportLogisticEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }
    }

    public class StartUseLogisticEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }
    }

    public class TenantStartUseImportLogisticEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }
    }

    public class BanishLogisticEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }
    }

    public class TenantBanishImportLogisticEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }
    }
    #endregion

    #region channel
    [AutoMapFrom(typeof(LogisticChannel))]
    public class ModifyChannelEvent : EventData
    {
        public long Id { get; set; }

        public long LogisticId { get; set; }

        public int? TenantId { get; set; }

        public string ChannelName { get; set; }

        public string AliasName { get; set; }

        public ChannelType Type { get; set; }

        public ChargeWay Way { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<NumFreightDto> NumFreights { get; set; }

        public IEnumerable<WeightFreightDto> WeightFreights { get; set; }
    }

    public class TenantModifyImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }
    }

    public class StartUseChannelEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }
    }

    public class TenantStartUseImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }
    }

    public class BanishChannelEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }
    }

    public class TenantBanishImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }
    }

    public class TenantImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public List<long> AddChannelIds { get; set; }

        public List<long> RemoveChannelIds { get; set; }
    }
    #endregion

    #region splitrule
    public class TenantCreateImportSplitRuleEvent :EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class TenanModifyImportSplitRuleEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class StartUseSplitRuleEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class TenantStartUseImportSplitRuleEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class BanishSplitRuleEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class TenantBanishImportSplitRuleEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }
    #endregion

    #region logistcirelation
    public class TenantCreateImportSplitRuleItemEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }

    public class TenanModifyImportSplitRuleItemEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }

    public class BanishSplitRuleItemEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }

    public class BanishTenantSplitRuleItemEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long ChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }
    #endregion
}
