using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.LogisticLines.Dto
{
    [AutoMap(typeof(LogisticLine))]
    public class UpdateLogisticLineDto : EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(LogisticLine.MaxLineNameLength)]
        public string LineName { get; set; }

        [Required]
        [StringLength(LogisticLine.MaxLineCodeLength)]
        public string LineCode { get; set; }

        [Required]
        public long LogisticId { get; set; }

        public bool IsActive { get; set; }
    }
}
