using SplitPackage.SplitV1;
using SplitPackage.Split.Dto;
using SplitPackage.Tests.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using Shouldly;

namespace SplitPackage.Tests.SplitV1
{
    [Collection("Assistant collection")]
    public class SplitService_Sku_Test
    {
        private readonly ISplitService _splitService;
        private readonly AssistantCase _context;

        public SplitService_Sku_Test(Xunit.Abstractions.ITestOutputHelper output, AssistantCase context)
        {
            this._context = context;
            this._splitService = context.ResolveService<ISplitService>();
        }

        /// <summary>
        /// 替换商品明细的ptid,拆单成功
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "1")]
        public async Task ReplaceSku_SplitSuccess_Test()
        {
            var scInput = new SplitRequest()
            {
                OrderId = "20180629001",
                Type = 3,
                TotalQuantity = 0,
                ProList = new List<SplitPackage.Split.SplitModels.Product>(){
                    new SplitPackage.Split.SplitModels.Product(){
                        ProNo = "测试商品1",
                        SkuNo = "20180629001",
                        Quantity = 4,
                        ProName = "奶牛的阵地",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010706"
                    },
                }
            };
            var result = await this._splitService.Split(scInput, this._context.GetTenantId());
            result.OrderList.Any(o => o.ProList.Any(oi => !oi.PTId.Equals("1019904"))).ShouldBeFalse();
            var sweInput = new SplitWithExpRequest1() {
                OrderId = "20180629001",
                TotalQuantity = 0,
                ProList = new List<SplitPackage.Split.SplitModels.Product>(){
                    new SplitPackage.Split.SplitModels.Product(){
                        ProNo = "测试商品1",
                        SkuNo = "20180629001",
                        Quantity = 4,
                        ProName = "奶牛的阵地",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010706"
                    },
                },
                logistics = new List<string> { "EWE Express 标准线" }
            };
            result = await this._splitService.SplitWithOrganization1(sweInput, this._context.GetTenantId());
            result.OrderList.Any(o => o.ProList.Any(oi => !oi.PTId.Equals("1019904"))).ShouldBeFalse();
        }

        /// <summary>
        /// 替换商品明细的ptid,拆单失败,
        /// 使用请求ptid拆单
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "1")]
        public async Task ReplaceSku_SplitFailure_Test()
        {
            var scInput = new SplitRequest()
            {
                OrderId = "20180629001",
                Type = 3,
                TotalQuantity = 0,
                ProList = new List<SplitPackage.Split.SplitModels.Product>(){
                    new SplitPackage.Split.SplitModels.Product(){
                        ProNo = "测试商品1",
                        SkuNo = "20180629002",
                        Quantity = 4,
                        ProName = "奶牛的阵地",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010706"
                    },
                }
            };
            var result = await this._splitService.Split(scInput, this._context.GetTenantId());
            result.OrderList.Any(o => o.ProList.Any(oi => !oi.PTId.Equals("1010706"))).ShouldBeFalse();
            var sweInput = new SplitWithExpRequest1()
            {
                OrderId = "20180629001",
                TotalQuantity = 0,
                ProList = new List<SplitPackage.Split.SplitModels.Product>(){
                    new SplitPackage.Split.SplitModels.Product(){
                        ProNo = "测试商品1",
                        SkuNo = "20180629002",
                        Quantity = 4,
                        ProName = "奶牛的阵地",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010706"
                    },
                },
                logistics = new List<string> { "EWE Express 标准线" }
            };
            result = await this._splitService.SplitWithOrganization1(sweInput, this._context.GetTenantId());
            result.OrderList.Any(o => o.ProList.Any(oi => !oi.PTId.Equals("1010706"))).ShouldBeFalse();
        }

        /// <summary>
        /// 根据sku规则拆单成功
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "1")]
        public async Task SkuRule_SplitSuccess_Test()
        {
            var scInput = new SplitRequest()
            {
                OrderId = "20180629001",
                Type = 3,
                TotalQuantity = 0,
                ProList = new List<SplitPackage.Split.SplitModels.Product>(){
                    new SplitPackage.Split.SplitModels.Product(){
                        ProNo = "测试商品1",
                        SkuNo = "20180629003",
                        Quantity = 4,
                        ProName = "奶牛的阵地",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010706"
                    },
                }
            };
            var result = await this._splitService.Split(scInput, this._context.GetTenantId());
            result.OrderList.Any(o => o.ProList.Any(oi => !oi.PTId.Equals("20180629003"))).ShouldBeFalse();
            var sweInput = new SplitWithExpRequest1()
            {
                OrderId = "20180629001",
                TotalQuantity = 0,
                ProList = new List<SplitPackage.Split.SplitModels.Product>(){
                    new SplitPackage.Split.SplitModels.Product(){
                        ProNo = "测试商品1",
                        SkuNo = "20180629003",
                        Quantity = 4,
                        ProName = "奶牛的阵地",
                        ProPrice = 10,
                        Weight = 100,
                        PTId = "1010706"
                    },
                },
                logistics = new List<string> { "EWE Express 标准线" }
            };
            result = await this._splitService.SplitWithOrganization1(sweInput, this._context.GetTenantId());
            result.OrderList.Any(o => o.ProList.Any(oi => !oi.PTId.Equals("20180629003"))).ShouldBeFalse();
        }
    }
}
