using AutoMapper;
using SplitPackage.Business.NumFreights.Dto;
using SplitPackage.Business.WeightFreights.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SplitPackage.Business.LogisticChannels.Dto
{
    public class LogisticChannelMapProfile : Profile
    {
        public LogisticChannelMapProfile()
        {
            CreateMap<WeightFreightDto, WeightFreight>();
            CreateMap<NumFreightDto, NumFreight>();

            CreateMap<LogisticChannel, LogisticChannelDto>().ForMember(x => x.LogisticName, opt => opt.MapFrom(src =>
                src.LogisticBy == null ? string.Empty : src.LogisticBy.CorporationName
            ));

            CreateMap<CreateLogisticChannelDto, LogisticChannel>();
        }
    }
}
