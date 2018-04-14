using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.Products.Dto
{
    [AutoMap(typeof(Product))]
    public class ProductDto : EntityDto<long>
    {

    }
}
