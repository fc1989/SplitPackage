using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.SplitRules.Dto
{
    [AutoMapFrom(typeof(SplitRuleProductClass))]
    public class RuleItemDto: EntityDto<long>
    {
        public long ProductClassId { get; set; }

        public string ProductClassName { get; set; }

        public int MaxNum { get; set; }

        public int MinNum { get; set; }
    }
}
