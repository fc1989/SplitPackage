using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.ProductSorts.Dto
{
    [AutoMapFrom(typeof(ProductSort))]
    public class ProductSortDto : EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(ProductSort.MaxSortNameLength)]
        public string SortName { get; set; }

        public bool IsActive { get; set; }
    }
}
