using AutoMapper;
using SplitPackage.Business;
using SplitPackage.Business.NumFreights.Dto;
using SplitPackage.Business.WeightFreights.Dto;
using SplitPackage.Domain.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.Cache.Dto
{
    public class CacheProfile : Profile
    {
        public CacheProfile()
        {
            CreateMap<Logistic, LogisticCacheDto>().ForMember(dest => dest.LogisticChannels, opt => opt.MapFrom(o => o.LogisticChannels));

            CreateMap<SplitRuleProductClass, SplitRuleProductClassCacheDto>();

            CreateMap<CreateLogisticEvent, LogisticCacheDto>();

            CreateMap<CreateChannelEvent, ChannelCacheDto>();

            CreateMap<CreateSplitRuleEvent, SplitRuleCacheDto>();

            CreateMap<LogisticRelated, LogisticRelatedCacheDto>().ConvertUsing(o => new LogisticRelatedCacheDto() {
                RelatedId = o.Id,
                Logistics = o.Items.Select(oi=>new LogisticRelatedOptionCacheDto()
                {
                    LogisticId = oi.LogisticId,
                    LogisticCode = oi.LogisticBy.LogisticCode
                }).ToList()
            });

            CreateMap<WeightFreight, WeightFreightCacheDto>();

            CreateMap<NumFreight, NumFreightCacheDto>();

            CreateMap<NumFreightDto, NumFreightCacheDto>();

            CreateMap<WeightFreightDto, WeightFreightCacheDto>();

            CreateMap<ModifyChannelEvent, ChannelCacheDto>();

            CreateMap<LogisticChannel, ModifyChannelEvent>();
        }
    }
}
