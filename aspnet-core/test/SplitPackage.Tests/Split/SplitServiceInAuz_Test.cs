using SplitPackage.Split;
using SplitPackage.Split.Common;
using SplitPackage.Split.Dto;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SplitPackage.Tests.Split
{
    /// <summary>
    /// 海购助手
    /// </summary>
    public class SplitServiceInAuz_Test
    {
        private readonly SplitServiceOld _splitService;

        public SplitServiceInAuz_Test()
        {
            _splitService = new SplitServiceOld();
            _splitService.Initialize(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SplitRuleXml/Auz"));
        }

        [Fact]
        public void SplitTest1()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9418783003414",
                        SkuNo = "9418783003414",
                        Quantity = 6,
                        ProName = "Aptamil爱他美白金版婴儿奶粉2段（6-12个月）900g",
                        ProPrice = 28.5M,
                        Weight = 1200
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("AU Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("澳邮婴儿奶粉专线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("AU Express", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("澳邮婴儿奶粉专线", result.Item2.OrderList[1].SubBusinessName);
        }

        [Fact]
        public void SplitTest2()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9344949000174",
                        SkuNo = "9344949000174",
                        Quantity = 1,
                        ProName = "Maxigenes 美可卓脱脂奶粉 蓝妹子 1kg",
                        ProPrice = 14.5M,
                        Weight = 1400
                    },
                    new Product()
                    {
                        ProNo = "9344949000136",
                        SkuNo = "9344949000136",
                        Quantity = 2,
                        ProName = "Maxigenes 美可卓全脂奶粉 蓝胖子 1kg",
                        ProPrice = 13.5M,
                        Weight = 1400
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("AU Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("澳邮成人奶粉专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitTest3()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "3039011141406",
                        SkuNo = "3039011141406",
                        Quantity = 2,
                        ProName = "【预售】OZLANA 2018 3039 春夏鞋",
                        ProPrice = 72.5M,
                        Weight = 1450
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("CNP Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("CNP Express", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[1].SubBusinessName);
        }

        [Fact]
        public void SplitTest4()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9311770588270",
                        SkuNo = "9311770588270",
                        Quantity = 2,
                        ProName = "Swisse 2019年3月到期 野生深海鱼油无腥 1000mg 200粒装",
                        ProPrice = 6,
                        Weight = 350
                    },
                    new Product()
                    {
                        ProNo = "9311770591690",
                        SkuNo = "9311770591690",
                        Quantity = 2,
                        ProName = "Swisse cranberry 蔓越莓 超浓缩胶囊25000mg 30粒",
                        ProPrice = 12.5M,
                        Weight = 150
                    },
                    new Product()
                    {
                        ProNo = "9311770592505",
                        SkuNo = "9311770592505",
                        Quantity = 2,
                        ProName = "swisse 钙加维D 150粒",
                        ProPrice = 14.5M,
                        Weight = 460
                    },
                    new Product()
                    {
                        ProNo = "9311770589994",
                        SkuNo = "9311770589994",
                        Quantity = 2,
                        ProName = "Swisse 护肝灵护肝片 120粒",
                        ProPrice = 18.95M,
                        Weight = 250
                    },
                    new Product()
                    {
                        ProNo = "9311770589307",
                        SkuNo = "9311770589307",
                        Quantity = 5,
                        ProName = "swisse 胶原蛋白液 500ml",
                        ProPrice = 19.5M,
                        Weight = 650
                    },
                    new Product()
                    {
                        ProNo = "9311770595766",
                        SkuNo = "9311770595766",
                        Quantity = 2,
                        ProName = "Swisse 2019年2月到期 月见草油软胶囊 200粒装",
                        ProPrice = 15,
                        Weight = 350
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(3, result.Item2.OrderList.Count);
            Assert.Equal("CHANGJIANG Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("长江国际速递杂货混装线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal(3, result.Item2.OrderList[0].ProList.Count);
            Assert.Equal("CHANGJIANG Express", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("长江国际速递杂货混装线", result.Item2.OrderList[1].SubBusinessName);
            Assert.Equal(2, result.Item2.OrderList[1].ProList.Count);
            Assert.Equal("CHANGJIANG Express", result.Item2.OrderList[2].LogisticsName);
            Assert.Equal("长江国际速递杂货混装线", result.Item2.OrderList[2].SubBusinessName);
            Assert.Equal(2, result.Item2.OrderList[2].ProList.Count);
        }

        [Fact]
        public void SplitTest5()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9332045000334",
                        SkuNo = "9332045000334",
                        Quantity = 1,
                        ProName = "Bellamy’s 贝拉米宝宝辅食 有机蔬菜通心粉12+ 200g",
                        ProPrice = 5.69M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9332045000860",
                        SkuNo = "9332045000860",
                        Quantity = 1,
                        ProName = "Bellamy’s 贝拉米宝宝辅食 有机蔬菜字母意面 8M+ 200g",
                        ProPrice = 5.69M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9332045000327",
                        SkuNo = "9332045000327",
                        Quantity = 1,
                        ProName = "Bellamy’s 贝拉米有机糙米 星星意大利面 7M+ 200g",
                        ProPrice = 5.69M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9332045000280",
                        SkuNo = "9332045000280",
                        Quantity = 2,
                        ProName = "Bellamy’s 贝拉米婴儿有机燕麦米粉(麦片粥)5M+ 125g",
                        ProPrice = 4.2M,
                        Weight = 190
                    },
                    new Product()
                    {
                        ProNo = "9332045000273",
                        SkuNo = "9332045000273",
                        Quantity = 2,
                        ProName = "贝拉米有机米粉原味 4M+ 125g 最新日期",
                        ProPrice = 4.2M,
                        Weight = 190
                    },
                    new Product()
                    {
                        ProNo = "9332045000297",
                        SkuNo = "9332045000297",
                        Quantity = 2,
                        ProName = "Bellamy’s 贝拉米有机婴儿香草米粉米糊125g 6M+",
                        ProPrice = 4.2M,
                        Weight = 190
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("ZH Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中环杂货单一装", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitTest6()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9332045001218",
                        SkuNo = "9332045001218",
                        Quantity = 4,
                        ProName = "Bellamy‘s Organic 贝拉米 燕麦无花果苹果泥",
                        ProPrice = 2.5M,
                        Weight = 165
                    },
                    new Product()
                    {
                        ProNo = "9332045000280",
                        SkuNo = "9332045000280",
                        Quantity = 2,
                        ProName = "Bellamy’s 贝拉米婴儿有机燕麦米粉(麦片粥)5M+ 125g",
                        ProPrice = 4.2M,
                        Weight = 190
                    },
                    new Product()
                    {
                        ProNo = "9319064004215",
                        SkuNo = "9319064004215",
                        Quantity = 1,
                        ProName = "B Box Bowl&Straw Strawberry Shake 吸管碗草莓色  大号 ｛红色｝",
                        ProPrice = 12.5M,
                        Weight = 320
                    },
                    new Product()
                    {
                        ProNo = "9319064003126",
                        SkuNo = "9319064003126",
                        Quantity = 1,
                        ProName = "Bbox 宝宝儿童吸管杯/重力球饮水杯 240ml Raspberry 枚红色",
                        ProPrice = 10.5M,
                        Weight = 195
                    },
                    new Product()
                    {
                        ProNo = "9311770591690",
                        SkuNo = "9311770591690",
                        Quantity = 1,
                        ProName = "Swisse cranberry 蔓越莓 超浓缩胶囊25000mg 30粒",
                        ProPrice = 12.5M,
                        Weight = 150
                    },
                    new Product()
                    {
                        ProNo = "9332045000204",
                        SkuNo = "9332045000204",
                        Quantity = 2,
                        ProName = "Bellamy’s 有机苹果肉桂燕麦粥6+ 125g",
                        ProPrice = 4.2M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9332045000273",
                        SkuNo = "9332045000273",
                        Quantity = 2,
                        ProName = "贝拉米有机米粉原味 4M+ 125g 最新日期",
                        ProPrice = 4.2M,
                        Weight = 190
                    },
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("ZH Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中环杂货混装线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal(5, result.Item2.OrderList[0].ProList.Count);
            Assert.Equal("ZH Express", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("中环杂货混装线", result.Item2.OrderList[1].SubBusinessName);
            Assert.Equal(3, result.Item2.OrderList[1].ProList.Count);
        }

        [Fact]
        public void SplitTest_PackageTooMany()
        {
            var request = new SplitRequest()
            {
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
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("ZH Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中环杂货单一装", result.Item2.OrderList[0].SubBusinessName);
        }
    }
}
