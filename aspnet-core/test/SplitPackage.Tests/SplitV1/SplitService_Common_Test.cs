using Abp.UI;
using SplitPackage.Split;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using SplitPackage.Tests.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.SplitV1
{
    [Collection("ReadStateless collection")]
    public class SplitService_Common_Test
    {
        private readonly ISplitService _splitService;
        private readonly ReadStatelessCase _context;

        public SplitService_Common_Test(Xunit.Abstractions.ITestOutputHelper output, ReadStatelessCase context)
        {
            this._context = context;
            this._splitService = this._context.ResolveService<ISplitService>();
        }

        [Fact]
        public async Task SplitInputValidTest()
        {
            UserFriendlyException result = null;
            var request = new SplitRequest()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26098951",
                        Quantity = 1,
                        ProName = "MAMIA婴幼儿奶粉二段900G二段",
                        ProPrice = 100,
                        Weight = 100,
                        PTId = "1010703"
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100,
                        PTId = "1010704"
                    }
                },
                TotalQuantity = 1,
                Type = 3
            };
            //商品重量不能为0
            request.ProList.ForEach(o => o.Weight = 0);
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.Split(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("product's weight must more then zero", result.Message);
            //商品价格不能为0
            request.ProList.ForEach(o => { o.ProPrice = 0; o.Weight = 100; });
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.Split(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("product's price must more then zero", result.Message);
            //商品数量不能为0
            request.ProList.ForEach(o => { o.Quantity = 0; o.ProPrice = 100; });
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.Split(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("product's quantity must more then zero", result.Message);
        }

        [Fact]
        public async Task SplitWithExp1InputValidTest()
        {
            UserFriendlyException result = null;
            var request = new SplitWithExpRequest1()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26098951",
                        Quantity = 1,
                        ProName = "MAMIA婴幼儿奶粉二段900G二段",
                        ProPrice = 100,
                        Weight = 100,
                        PTId = "1010703"
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100,
                        PTId = "1010704"
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "AOLAU EXPRESS" }
            };
            //商品重量不能为0
            request.ProList.ForEach(o => o.Weight = 0);
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("product's weight must more then zero", result.Message);
            //商品价格不能为0
            request.ProList.ForEach(o => { o.ProPrice = 0; o.Weight = 100; });
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("product's price must more then zero", result.Message);
            //商品数量不能为0
            request.ProList.ForEach(o => { o.Quantity = 0; o.ProPrice = 100; });
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("product's quantity must more then zero", result.Message);
            //指定物流商不存在
            request.ProList.ForEach(o => o.Quantity = 1);
            request.logistics = new List<string> { };
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("logistic must more then zero", result.Message);
            request.logistics = new List<string> { "123", "1" };
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("the following logistics providers do not exist:123,1", result.Message);
            request.ProList.ForEach(o => { o.PTId = "0"; });
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("the following logistics providers do not exist:123,1", result.Message);
            //无效的PTId
            request.logistics = new List<string> { "AOLAU EXPRESS" };
            result = await Assert.ThrowsAsync<UserFriendlyException>(() => this._splitService.SplitWithOrganization1(request, this._context.GetTenantId()));
            Assert.Equal((int)ResultCode.BadRequest_ParamConstraint, result.Code);
            Assert.Equal("the following ptid no corresponding rules:0", result.Message);
        }
    }
}
