using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.LogisticRelateds.Dto
{
    [AutoMap(typeof(LogisticRelated))]
    public class CreateLogisticRelatedDto
    {
        [Required]
        [MaxLengthAttribute(LogisticRelated.MaxRelatedNameLength)]
        public string RelatedName { get; set; }

        public List<long> LogisticIds { get; set; }
    }
}
