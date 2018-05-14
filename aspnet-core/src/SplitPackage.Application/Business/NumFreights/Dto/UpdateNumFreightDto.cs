using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.NumFreights.Dto
{
    [AutoMap(typeof(NumFreight))]
    public class UpdateNumFreightDto : EntityDto<long>
    {
        public string Currency { get; set; }

        public string Unit { get; set; }

        public int SplitNum { get; set; }

        public double FirstPrice { get; set; }

        public double CarryOnPrice { get; set; }
    }
}
