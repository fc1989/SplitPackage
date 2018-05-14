using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.NumFreights.Dto
{
    [AutoMapFrom(typeof(NumFreight))]
    public class NumFreightDto : EntityDto<long>, IPassivable
    {
        [Required]
        public long LogisticChannelId { get; set; }

        public string LogisticChannelName { get; set; }

        public bool IsActive { get; set; }

        public string Currency { get; set; }

        public string Unit { get; set; }

        public int SplitNum { get; set; }

        public double FirstPrice { get; set; }

        public double CarryOnPrice { get; set; }
    }
}
