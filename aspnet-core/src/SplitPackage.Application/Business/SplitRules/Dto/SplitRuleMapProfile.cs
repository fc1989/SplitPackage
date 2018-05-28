using AutoMapper;
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
                .ForMember(x => x.TenantId, opt => opt.MapFrom(src => src.LogisticChannelBy.LogisticBy.TenantId));

            CreateMap<SplitRuleProductClass, RuleItemDto>().ForMember(x => x.RuleName, opt => opt.MapFrom(src => src.SplitRuleBy.RuleName));
        }
    }
}
