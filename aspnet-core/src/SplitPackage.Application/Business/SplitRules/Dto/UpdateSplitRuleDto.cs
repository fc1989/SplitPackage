﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.SplitRules.Dto
{
    [AutoMap(typeof(SplitRule))]
    public class UpdateSplitRuleDto : EntityDto<long>
    {
        [Required]
        public long LogisticChannelId { get; set; }

        public string RuleName { get; set; }

        public int MaxPackage { get; set; }

        public double MaxWeight { get; set; }

        public double MaxTax { get; set; }

        public double MaxPrice { get; set; }
    }
}
