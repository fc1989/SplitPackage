using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.LogisticRelateds.Dto
{
    [AutoMap(typeof(LogisticRelated))]
    public class UpdateLogisticRelatedDto : EntityDto<long>
    {
        public string RelatedName { get; set; }

        public List<long> LogisticIds { get; set; }
    }
}
