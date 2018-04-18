using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.Products
{
    public class ProductAppService : AsyncCrudAppService<Product, ProductDto, long, PagedResultRequestDto, CreateProductDto, UpdateProductDto>, IProductAppService
    {
        public ProductAppService(IRepository<Product, long> repository) : base(repository)
        {

        }
    }
}
