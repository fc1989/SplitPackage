using AutoMapper;
using SplitPackage.Domain.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.SplitRules.Dto
{
    public class SplitRuleMapProfile : Profile
    {
        public SplitRuleMapProfile()
        {
            CreateMap<SplitRule, SplitRuleDto>().ForMember(x => x.LogisticChannelName, opt => opt.MapFrom(src => src.LogisticChannelBy.ChannelName))
                .ForMember(x => x.LogisticName, opt => opt.MapFrom(src => src.LogisticChannelBy.LogisticBy.CorporationName))
                .ForMember(x => x.TenantId, opt => opt.MapFrom(src => src.TenantId));

            CreateMap<SplitRuleItem, RuleItemDto>().ForMember(x => x.RuleName, opt => opt.MapFrom(src => src.SplitRuleBy.RuleName));

            CreateMap<SplitRule, CreateSplitRuleEvent>();

            CreateMap<SplitRuleItem, CreateSplitRuleItemEvent>();

            CreateMap<SplitRuleItem, ModifySplitRuleItemEvent>();
        }
    }
}
