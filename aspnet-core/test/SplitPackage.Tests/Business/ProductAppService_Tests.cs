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
using System.Linq;
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
            var output = await ((ProductAppService)_productAppService).GetAll(new ProductSearchFilter { MaxResultCount = 20, SkipCount = 0 });

            // Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateUser_Test()
        {
            // Act
            string sku = DateTime.Now.ToString("yyyyMMDDHHmmssff");
            await ((ProductAppService)_productAppService).Create(
                new CreateProductDto
                {
                    ProductName = "测试",
                    Sku = sku,
                    Brand = "测试",
                    Weight = 10,
                    DeclarePrice = 10,
                    DeclareTaxrate = 0.5,
                    PTId = "123456789"
                });

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Products.FirstOrDefaultAsync(u => u.Sku.Equals(sku));
                johnNashUser.ShouldNotBeNull();
            });
        }
    }
}
