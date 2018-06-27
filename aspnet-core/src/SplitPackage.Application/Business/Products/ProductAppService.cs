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
using AutoMapper;
using Abp.Authorization;
using SplitPackage.Authorization;
using System.Linq.Expressions;
using SplitPackage.Dto;

namespace SplitPackage.Business.Products
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_Products)]
    public class ProductAppService : AsyncCrudAppService<Product, ProductDto, long, ProductSearchFilter, CreateProductDto, UpdateProductDto>
    {
        public ProductAppService(IRepository<Product, long> repository) : base(repository)
        {

        }

        protected override IQueryable<Product> CreateFilteredQuery(ProductSearchFilter input)
        {
            var filter = input.GenerateFilter();
            return filter == null ? Repository.GetAll() : Repository.GetAll().Where(input.GenerateFilter());
        }

        public async Task<bool> Verify(string sku)
        {
            var count = await this.Repository.GetAll().Where(o=>o.Sku == sku).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<List<OptionDto<string>>> GetOwnOption(string sku)
        {
            var result = await this.Repository.GetAll().Where(o => o.Sku.Contains(sku) && o.IsActive).Select(o=> new OptionDto<string>() {
                Value = o.Sku,
                Label = o.ProductName
            }).ToListAsync();
            return result;
        }
    }
}
