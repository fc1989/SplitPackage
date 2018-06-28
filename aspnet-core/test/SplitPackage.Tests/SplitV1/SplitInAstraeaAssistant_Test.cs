using Abp.Authorization.Users;
using SplitPackage.Split;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using SplitPackage.Tests.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.Split
{
    [Collection("Assistant collection")]
    public class SplitInAstraeaAssistant_Test
    {
        private readonly ISplitService _splitService;
        private readonly AssistantCase _context;

        public SplitInAstraeaAssistant_Test(Xunit.Abstractions.ITestOutputHelper output, AssistantCase context)
        {
            this._context = context;
            this._splitService = this._context.ResolveService<ISplitService>();
        }

        [Fact]
        public async Task SplitWithExp1Test1()
        {
            var request = new SplitWithExpRequest1()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 2,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 5,
                        Weight = 100,
                        PTId = "1019904"
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 1,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010703"
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 2,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "9029900"
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "CNP Express" }
            };
            var result = await this._splitService.SplitWithOrganization1(request, this._context.GetTenantId());
            Assert.Single(result.OrderList);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsName);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsCode);
            Assert.Equal("中邮混装线", result.OrderList[0].SubBusinessName);
        }

        [Fact]
        public async Task SplitWithExp1Test2()
        {
            var request = new SplitWithExpRequest1()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 10,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "9029900"
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "CNP Express" }
            };
            var result = await this._splitService.SplitWithOrganization1(request, this._context.GetTenantId());
            Assert.Single(result.OrderList);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsName);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsCode);
            Assert.Equal("中邮杂货专线", result.OrderList[0].SubBusinessName);
        }

        [Fact]
        public async Task SplitWithExp1Test3()
        {
            var request = new SplitWithExpRequest1()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 10,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "9029900"
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 15,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1019903"
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "CNP Express" }
            };
            var result = await this._splitService.SplitWithOrganization1(request, this._context.GetTenantId());
            Assert.Equal(2, result.OrderList.Count);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsName);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsCode);
            Assert.Equal("中邮杂货专线", result.OrderList[0].SubBusinessName);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsName);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsCode);
            Assert.Equal("中邮杂货专线", result.OrderList[0].SubBusinessName);
        }

        [Fact]
        public async Task SplitWithExp1Test4()
        {
            var request = new SplitWithExpRequest1()
            {
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 5,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1019904"
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 10,
                        ProName = "XSL 纯天然高端减肥粉",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1019903"
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "AOLAU EXPRESS" }
            };
            var result = await this._splitService.SplitWithOrganization1(request, this._context.GetTenantId());
            Assert.Equal(2, result.OrderList.Count);
            Assert.Equal("AOLAU EXPRESS", result.OrderList[0].LogisticsName);
            Assert.Equal("AOLAU EXPRESS", result.OrderList[0].LogisticsCode);
            Assert.Equal("澳通速递杂货混装线", result.OrderList[0].SubBusinessName);
            Assert.Equal("AOLAU EXPRESS", result.OrderList[1].LogisticsName);
            Assert.Equal("AOLAU EXPRESS", result.OrderList[1].LogisticsCode);
            Assert.Equal("澳通速递杂货混装线", result.OrderList[1].SubBusinessName);
        }
    }
}
