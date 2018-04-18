using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.ProductClasses.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.ProductClasses
{
    public class ProductClassAppService : AsyncCrudAppService<ProductClass, ProductClassDto, long, PagedResultRequestDto, CreateProductClassDto, UpdateProductClassDto>, IProductClassAppService
    {
        public ProductClassAppService(IRepository<ProductClass, long> repository) : base(repository)
        {

        }
    }
}
