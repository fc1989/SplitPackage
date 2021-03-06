﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.Logistics.Dto
{
    [AutoMap(typeof(Logistic))]
    public class UpdateLogisticDto : EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(Logistic.MaxCorporationNameLength)]
        public string CorporationName { get; set; }

        [StringLength(Logistic.MaxCorporationUrlLength)]
        public string CorporationUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
