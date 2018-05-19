using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.ProductSorts.Dto
{
    [AutoMap(typeof(ProductSort))]
    public class CreateProductSortDto : EntityDto<long>
    {
        public string SortName { get; set; }
    }
}