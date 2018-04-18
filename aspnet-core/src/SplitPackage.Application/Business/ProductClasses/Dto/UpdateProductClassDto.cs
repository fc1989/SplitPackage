using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.ProductClasses.Dto
{
    [AutoMap(typeof(ProductClass))]
    public class UpdateProductClassDto : EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(ProductClass.MaxClassNameLength)]
        public string ClassName { get; set; }

        [Required]
        [StringLength(ProductClass.MaxPTIdLength)]
        public string PTId { get; set; }

        public double PostTaxRate { get; set; }

        public double BCTaxRate { get; set; }

        public bool IsActive { get; set; }
    }
}
