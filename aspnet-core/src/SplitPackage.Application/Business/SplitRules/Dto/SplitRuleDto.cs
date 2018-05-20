using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.SplitRules.Dto
{
    [AutoMapFrom(typeof(SplitRule))]
    public class SplitRuleDto : EntityDto<long>, IPassivable
    {
        public string LogisticName { get; set; }

        [Required]
        public long LogisticChannelId { get; set; }

        public string LogisticChannelName { get; set; }

        public string RuleName { get; set; }

        public int MaxPackage { get; set; }

        public double MaxWeight { get; set; }

        public double MaxTax { get; set; }

        public double MaxPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
