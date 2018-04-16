using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using SplitPackage.Business;
using SplitPackage.Business.Products;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.Business
{
    public class ProductAppService_Tests : SplitPackageTestBase
    {
        private readonly IProductAppService _productAppService;

        public ProductAppService_Tests()
        {
            _productAppService = Resolve<IProductAppService>();
        }

        [Fact]
        public async Task GetProducts_Test()
        {
            // Act
            var output = await ((ProductAppService)_productAppService).GetAll(new PagedResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

            // Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }

        //[Fact]
        public async Task CreateUser_Test()
        {
            // Act
            await ((ProductAppService)_productAppService).Create(
                new CreateProductDto
                {

                });

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Products.FirstOrDefaultAsync(u => u.ProductName == "测试商品");
                johnNashUser.ShouldNotBeNull();
            });
        }
    }
}
