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
    public class CreateLogisticEvent : EventData
    {
        public long Id { get; set; }

        public string CorporationName { get; set; }

        public string CorporationUrl { get; set; }

        public string LogoURL { get; set; }

        public string LogisticCode { get; set; }

        public int? TenantId { get; set; }
    }

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
    public class CreateChannelEvent : EventData
    {
        public long Id { get; set; }

        public long LogisticId { get; set; }

        public int? TenantId { get; set; }

        public string ChannelName { get; set; }

        public string AliasName { get; set; }

        public ChannelType Type { get; set; }

        public ChargeWay Way { get; set; }

        public IEnumerable<Business.NumFreight> NumFreights { get; set; }

        public IEnumerable<Business.WeightFreight> WeightFreights { get; set; }
    }

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

        public long LogisticChannelId { get; set; }
    }

    public class StartUseChannelEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }
    }

    public class TenantStartUseImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }
    }

    public class BanishChannelEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }
    }

    public class TenantBanishImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }
    }

    public class TenantImportChannelEvent : EventData
    {
        public int TenantId { get; set; }

        public List<long> AddChannelIds { get; set; }

        public List<long> RemoveChannelIds { get; set; }
    }
    #endregion

    #region splitrule
    public class CreateSplitRuleEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long Id { get; set; }

        public string RuleName { get; set; }

        public int MaxPackage { get; set; }

        public double MaxWeight { get; set; }

        public double MaxTax { get; set; }

        public double MaxPrice { get; set; }
    }

    public class TenantCreateImportSplitRuleEvent :EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class ModifyImportSplitRuleEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public string RuleName { get; set; }

        public int MaxPackage { get; set; }

        public double MaxWeight { get; set; }

        public double MaxTax { get; set; }

        public double MaxPrice { get; set; }
    }

    public class TenanModifyImportSplitRuleEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class StartUseSplitRuleEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class TenantStartUseImportSplitRuleEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class BanishSplitRuleEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }

    public class TenantBanishImportSplitRuleEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }
    }
    #endregion

    #region splitruleitem
    public class CreateSplitRuleItemEvent : EventData
    {
        public long Id { get; set; }

        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public string PTId { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }
    }

    public class TenantCreateImportSplitRuleItemEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }

    public class ModifySplitRuleItemEvent : EventData
    {
        public long Id { get; set; }

        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public string PTId { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }
    }

    public class TenanModifyImportSplitRuleItemEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }

    public class BanishSplitRuleItemEvent : EventData
    {
        public int? TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }

    public class BanishTenantSplitRuleItemEvent : EventData
    {
        public int TenantId { get; set; }

        public long LogisticId { get; set; }

        public long LogisticChannelId { get; set; }

        public long SplitRuleId { get; set; }

        public long SplitRuleItemId { get; set; }
    }
    #endregion

    #region logisticrelation
    public class CreateLogisticRelation : EventData
    {
        public int? TenantId { get; set; }

        public long RelationId { get; set; }

        public List<long> LogisticIds { get; set; }
    }

    public class ModifyLogisticRelation : EventData
    {
        public int? TenantId { get; set; }

        public long RelationId { get; set; }

        public List<long> AddLogisticIds { get; set; }

        public List<long> RemoveLogisticIds { get; set; }
    }

    public class BanishLogisticRelation : EventData
    {
        public int? TenantId { get; set; }

        public long RelationId { get; set; }
    }
    #endregion
}
