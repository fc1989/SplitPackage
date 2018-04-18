﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.WeightFreights.Dto
{
    [AutoMap(typeof(WeightFreight))]
    public class CreateWeightFreightDto : EntityDto<long>
    {
        [Required]
        public long LogisticLineId { get; set; }

        public double StartingWeight { get; set; }

        public double StartingPrice { get; set; }

        public double StepWeight { get; set; }

        public double Price { get; set; }
    }
}