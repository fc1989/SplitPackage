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

        public override async Task<PagedResultDto<ProductDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Select(o=>new ProductDto() {
                ProductName = o.ProductName,
                AbbreName = o.AbbreName,
                ProductNo = o.ProductNo,
                Sku = o.Sku,
                TaxNo = o.TaxNo,
                Brand = o.Brand,
                Weight = o.Weight,
                IsActive = o.IsActive,
                ProductClassIds = o.ProductClasses.Select(oi=>oi.ProductClassId)
            }));

            return new PagedResultDto<ProductDto>(
                totalCount,
                entities.ToList()
            );
        }

        public override async Task<ProductDto> Create(CreateProductDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task<bool> Verify(string sku)
        {
            var count = await this.Repository.GetAll().Where(o=>o.Sku == sku).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }
    }
}
