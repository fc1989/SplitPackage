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
        public long LogisticLineId { get; set; }

        public string LogisticLineName { get; set; }

        public int ProductNum { get; set; }

        public double PackagePrice { get; set; }

        public bool IsActive { get; set; }
    }
}
