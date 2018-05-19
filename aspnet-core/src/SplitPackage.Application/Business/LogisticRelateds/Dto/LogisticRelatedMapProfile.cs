using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SplitPackage.Business.LogisticRelateds.Dto
{
    public class LogisticRelatedMapProfile : Profile
    {
        public LogisticRelatedMapProfile()
        {
            CreateMap<LogisticRelated, LogisticRelatedDto>().ForMember(x => x.Items, opt => opt.MapFrom(src =>
                src.Items.Select(o=>
                    new LogisticRelateItemdDto()
                    {
                        LogisticId = o.LogisticId,
                        LogisticName = o.LogisticBy == null ? string.Empty : o.LogisticBy.CorporationName
                    }).ToList()
            ));
        }
    }
}
