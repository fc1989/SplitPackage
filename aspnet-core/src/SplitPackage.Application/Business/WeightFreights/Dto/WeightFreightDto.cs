﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.WeightFreights.Dto
{
    [AutoMapFrom(typeof(WeightFreight))]
    public class WeightFreightDto : EntityDto<long>, IPassivable
    {
        [Required]
        public long LogisticLineId { get; set; }

        public string LogisticLineName { get; set; }

        public double StartingWeight { get; set; }

        public double StartingPrice { get; set; }

        public double StepWeight { get; set; }

        public double Price { get; set; }

        public bool IsActive { get; set; }
    }
}
