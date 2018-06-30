using Abp.UI;
using SplitPackage.SplitV1;
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
        public async Task SplitInputValid_Test()
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
        public async Task SplitWithExp1InputValid_Test()
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

        [Fact]
        public async Task SplitWithRemain_Test()
        {
            var request = new SplitRequest()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "20180629001",
                        SkuNo = "20180629001",
                        Quantity = 10,
                        ProName = "夯大力型号1",
                        ProPrice = 50,
                        Weight = 60,
                        PTId = "1234567890"
                    },
                    new Product()
                    {
                        ProNo = "20180629002",
                        SkuNo = "20180629002",
                        Quantity = 4,
                        ProName = "夯大力型号2",
                        ProPrice = 30,
                        Weight = 40,
                        PTId = "1234567890"
                    }
                },
                TotalQuantity = 1,
                Type = 3
            };
            var result = await this._splitService.Split(request, this._context.GetTenantId());
            Assert.Single(result.OrderList);
            Assert.Equal("-1", result.OrderList[0].Id);
            Assert.Null(result.OrderList[0].LogisticsName);
            Assert.Null(result.OrderList[0].LogisticsCode);
            Assert.Null(result.OrderList[0].URL);
            Assert.Null(result.OrderList[0].GradeName);
            Assert.Null(result.OrderList[0].SubBusinessName);
            Assert.Equal(0, result.OrderList[0].LogisticsUnitPrice);
            Assert.Equal(0, result.OrderList[0].LogisticsCost);
            Assert.Equal(0, result.OrderList[0].TaxCost);
            Assert.Equal(760, result.OrderList[0].TotalWeight);
            Assert.Equal(620, result.OrderList[0].TotalPrice);
            Assert.Equal(2, result.OrderList[0].ProList.Count);
            Assert.Equal("20180629001", result.OrderList[0].ProList[0].ProNo);
            Assert.Equal("20180629001", result.OrderList[0].ProList[0].SkuNo);
            Assert.Equal(10, result.OrderList[0].ProList[0].Quantity);
            Assert.Equal("夯大力型号1", result.OrderList[0].ProList[0].ProName);
            Assert.Equal(50, result.OrderList[0].ProList[0].ProPrice);
            Assert.Equal(60, result.OrderList[0].ProList[0].Weight);
            Assert.Equal("1234567890", result.OrderList[0].ProList[0].PTId);
            Assert.Equal("20180629002", result.OrderList[0].ProList[1].ProNo);
            Assert.Equal("20180629002", result.OrderList[0].ProList[1].SkuNo);
            Assert.Equal(4, result.OrderList[0].ProList[1].Quantity);
            Assert.Equal("夯大力型号2", result.OrderList[0].ProList[1].ProName);
            Assert.Equal(30, result.OrderList[0].ProList[1].ProPrice);
            Assert.Equal(40, result.OrderList[0].ProList[1].Weight);
            Assert.Equal("1234567890", result.OrderList[0].ProList[1].PTId);
        }
    }
}
