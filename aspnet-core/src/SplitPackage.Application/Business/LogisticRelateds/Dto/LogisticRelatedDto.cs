using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.LogisticRelateds.Dto
{
    [AutoMap(typeof(LogisticRelated))]
    public class LogisticRelatedDto : EntityDto<long>
    {
        public string RelatedName { get; set; }

        public int? TenantId { get; set; }

        public IEnumerable<LogisticRelateItemdDto> Items { get; set; }
    }

    [AutoMap(typeof(LogisticRelatedItem))]
    public class LogisticRelateItemdDto
    {
        public long LogisticId { get; set; }

        public string LogisticName { get; set; }
    }
}
