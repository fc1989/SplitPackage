using SplitPackage.Split;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;

namespace SplitPackage.Tests.Split
{
    public class SplitAppService1_Test
    {
        private readonly ISplitService _splitAppService;

        public SplitAppService1_Test()
        {
            _splitAppService = new SplitService();
            this._splitAppService.Initialize(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SplitPackageRules1"));
        }

        [Fact]
        public async Task SplitTest()
        {
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9332045000860",
                        SkuNo = "9332045000860",
                        Quantity = 4,
                        ProName = "Bellamy’s 贝拉米宝宝辅食 有机蔬菜字母意面 8M+ 200g",
                        ProPrice = 5.69M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9332045000327",
                        SkuNo = "9332045000327",
                        Quantity = 4,
                        ProName = "Bellamy’s 贝拉米有机糙米 星星意大利面 7M+ 200g",
                        ProPrice = 5.69M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9332045000297",
                        SkuNo = "9332045000297",
                        Quantity = 2,
                        ProName = "Bellamy’s 贝拉米有机婴儿香草米粉米糊125g 6M+",
                        ProPrice = 4.2M,
                        Weight = 190
                    },
                    new Product()
                    {
                        ProNo = "9332045000280",
                        SkuNo = "9332045000280",
                        Quantity = 6,
                        ProName = "Bellamy’s 贝拉米婴儿有机燕麦米粉(麦片粥)5M+ 125g",
                        ProPrice = 4.2M,
                        Weight = 190
                    },
                    new Product()
                    {
                        ProNo = "9332045000204",
                        SkuNo = "9332045000204",
                        Quantity = 4,
                        ProName = "Bellamy’s 有机苹果肉桂燕麦粥6+ 125g",
                        ProPrice = 4.2M,
                        Weight = 200
                    }
                },
                TotalQuantity = 20,
                Type = 1
            };
            var result = this._splitAppService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中环杂货单一装", result.Item2.OrderList[0].SubBusinessName);
            await Task.CompletedTask;
        }
    }
}
