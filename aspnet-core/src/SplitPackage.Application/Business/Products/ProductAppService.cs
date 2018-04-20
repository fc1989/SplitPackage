using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SplitPackage.Business.Products
{
    public class ProductAppService : AsyncCrudAppService<Product, ProductDto, long, PagedResultRequestDto, CreateProductDto, UpdateProductDto>, IProductAppService
    {
        public ProductAppService(IRepository<Product, long> repository) : base(repository)
        {

        }

        public async Task<bool> Verify(string sku)
        {
            CheckGetAllPermission();

            var count = await this.Repository.GetAll().Where(o=>o.Sku == sku).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }
    }
}
