using SplitPackage.Split;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SplitPackage.Tests.Split
{
    public class SplitServiceInPhoenix_Test
    {
        private readonly SplitServiceOld _splitService;

        public SplitServiceInPhoenix_Test()
        {
            _splitService = new SplitServiceOld();
            _splitService.Initialize(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SplitRuleXml/Phoenix"));
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
                        ProNo = "9344949001140",
                        SkuNo = "9344949001140",
                        Quantity = 8,
                        ProName = "Bio island 孕妇专用海藻油DHA 60粒",
                        ProPrice = 22.5M,
                        Weight = 120
                    },
                    new Product()
                    {
                        ProNo = "9344949001294",
                        SkuNo = "9344949001294",
                        Quantity = 5,
                        ProName = "Bio island 乳钙骨护理软胶囊 150粒",
                        ProPrice = 19.5M,
                        Weight = 420
                    },
                    new Product()
                    {
                        ProNo = "9344949001256",
                        SkuNo = "9344949001256",
                        Quantity = 5,
                        ProName = "Bio island 婴幼儿童乳钙胶囊 90粒",
                        ProPrice = 18M,
                        Weight = 120
                    },
                    new Product()
                    {
                        ProNo = "9331927003333",
                        SkuNo = "9331927003333",
                        Quantity = 3,
                        ProName = "Life Space 婴儿益生菌粉 6个月-3岁 调节肠胃增强免疫力 60g",
                        ProPrice = 24.95M,
                        Weight = 250
                    },
                    new Product()
                    {
                        ProNo = "9331927003326",
                        SkuNo = "9331927003326",
                        Quantity = 2,
                        ProName = "Life Space 孕妇专用益生菌 调节肠胃便秘增强免疫力 60粒",
                        ProPrice = 24.95M,
                        Weight = 250
                    },
                    new Product()
                    {
                        ProNo = "9314807022839",
                        SkuNo = "9314807022839",
                        Quantity = 2,
                        ProName = "Nature‘s Way佳思敏复合维生素+蔬菜软糖60粒",
                        ProPrice = 9M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9311770592505",
                        SkuNo = "9311770592505",
                        Quantity = 6,
                        ProName = "swisse 钙加维D 150粒",
                        ProPrice = 19M,
                        Weight = 350
                    },
                    new Product()
                    {
                        ProNo = "9314807025267",
                        SkuNo = "9314807025267",
                        Quantity = 2,
                        ProName = "swisse 钙加维D 150粒",
                        ProPrice = 9.5M,
                        Weight = 200
                    },
                    new Product()
                    {
                        ProNo = "9311770590082",
                        SkuNo = "9311770590082",
                        Quantity = 3,
                        ProName = "Swisse 高浓缩维C泡腾片 60粒装",
                        ProPrice = 19.99M,
                        Weight = 460
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(4, result.Item2.OrderList.Count);
        }

        /// <summary>
        /// 3个包裹，但应该是3+3+3，而不是4+3+2，这些都是洗护产品
        /// </summary>
        [Fact]
        public void SplitTest2()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "708177112662",
                        SkuNo = "708177112662",
                        Quantity = 3,
                        ProName = "Jurlique 恬蜜玫瑰沐浴露 300ml",
                        ProPrice = 24.5M,
                        Weight = 400
                    },
                    new Product()
                    {
                        ProNo = "708177054375",
                        SkuNo = "708177054375",
                        Quantity = 2,
                        ProName = "Jurlique 檀香木护发素 300ml",
                        ProPrice = 19.6M,
                        Weight = 500
                    },
                    new Product()
                    {
                        ProNo = "708177054290",
                        SkuNo = "708177054290",
                        Quantity = 3,
                        ProName = "Jurlique 檀香木洗发水 300ml 檀香木洗发水",
                        ProPrice = 19.6M,
                        Weight = 500
                    },
                    new Product()
                    {
                        ProNo = "708177054399",
                        SkuNo = "708177054399",
                        Quantity = 1,
                        ProName = "Jurlique婴儿温和洗发沐浴露200ml",
                        ProPrice = 17.5M,
                        Weight = 272
                    }
                },
                Type = 3
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
        }

        /// <summary>
        /// 2包裹，但其中最后两个洗护商品一个包裹应该最多只能有三个
        /// </summary>
        [Fact]
        public void SplitTest3()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9331927003678",
                        SkuNo = "9331927003678",
                        Quantity = 1,
                        ProName = "Life Space 儿童益生菌粉 适合3-12岁儿童 60克",
                        ProPrice = 23.5M,
                        Weight = 150
                    },
                    new Product()
                    {
                        ProNo = "9311770592505",
                        SkuNo = "9311770592505",
                        Quantity = 1,
                        ProName = "swisse 钙加维D 150粒",
                        ProPrice = 18M,
                        Weight = 350
                    },
                    new Product()
                    {
                        ProNo = "9344949001249",
                        SkuNo = "9344949001249",
                        Quantity = 2,
                        ProName = "Bio island 婴幼儿童鳕鱼肝+鱼油 90粒",
                        ProPrice = 18.5M,
                        Weight = 150
                    },
                    new Product()
                    {
                        ProNo = "9331927003326",
                        SkuNo = "9331927003326",
                        Quantity = 1,
                        ProName = "Life Space 孕妇专用益生菌 调节肠胃便秘增强免疫力 60粒",
                        ProPrice = 23.5M,
                        Weight = 250
                    },
                    new Product()
                    {
                        ProNo = "9331927003333",
                        SkuNo = "9331927003333",
                        Quantity = 1,
                        ProName = "Life Space 婴儿益生菌粉 6个月-3岁 调节肠胃增强免疫力 60g",
                        ProPrice = 23.5M,
                        Weight = 250
                    },
                    new Product()
                    {
                        ProNo = "9331927003357",
                        SkuNo = "9331927003357",
                        Quantity = 1,
                        ProName = "Life Space 60岁以上老年人益生菌 调节肠胃免疫力 60粒",
                        ProPrice = 23.5M,
                        Weight = 220
                    },
                    new Product()
                    {
                        ProNo = "9314839011948",
                        SkuNo = "9314839011948",
                        Quantity = 2,
                        ProName = "Ego QV婴儿角鲨烷滋润保湿霜250g",
                        ProPrice = 11M,
                        Weight = 350
                    },
                    new Product()
                    {
                        ProNo = "9314839011931",
                        SkuNo = "9314839011931",
                        Quantity = 2,
                        ProName = "Ego QV婴儿洗发沐浴2合1 无泪配方250g",
                        ProPrice = 6.99M,
                        Weight = 280
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
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
                        ProNo = "689214788944",
                        SkuNo = "689214788944",
                        Quantity = 1,
                        ProName = "Goodcombo 角鲨烯胶囊 1000毫克 365粒",
                        ProPrice = 45M,
                        Weight = 720
                    },
                    new Product()
                    {
                        ProNo = "9316199000037",
                        SkuNo = "9316199000037",
                        Quantity = 1,
                        ProName = "LUCAS PAPAW神奇番木瓜膏25g",
                        ProPrice = 4.5M,
                        Weight = 75
                    },
                    new Product()
                    {
                        ProNo = "9300807238837",
                        SkuNo = "9300807238837",
                        Quantity = 1,
                        ProName = "Jurlique 檀香木洗发水 300ml 檀香木洗发水",
                        ProPrice = 8M,
                        Weight = 80
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
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
                        ProNo = "708177066699",
                        SkuNo = "708177066699",
                        Quantity = 1,
                        ProName = "Jurlique 草本高抗氧晚霜 50ml",
                        ProPrice = 34.5M,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "708177098218",
                        SkuNo = "708177098218",
                        Quantity = 1,
                        ProName = "Jurlique 臻萃活颜莹润乳液100ml",
                        ProPrice = 35M,
                        Weight = 318
                    },
                    new Product()
                    {
                        ProNo = "93530828",
                        SkuNo = "93530828",
                        Quantity = 2,
                        ProName = "Ego QV 唇膏SPF30+ 15g保湿防嗮",
                        ProPrice = 4M,
                        Weight = 35
                    },
                    new Product()
                    {
                        ProNo = "708177054290",
                        SkuNo = "708177054290",
                        Quantity = 1,
                        ProName = "Jurlique 檀香木洗发水 300ml 檀香木洗发水",
                        ProPrice = 14M,
                        Weight = 500
                    },
                    new Product()
                    {
                        ProNo = "708177054375",
                        SkuNo = "708177054375",
                        Quantity = 1,
                        ProName = "Jurlique 檀香木护发素 300ml",
                        ProPrice = 14M,
                        Weight = 500
                    },
                    new Product()
                    {
                        ProNo = "9314839008870",
                        SkuNo = "9314839008870",
                        Quantity = 2,
                        ProName = "QV Ego(意高) Sunsense 防晒霜滚珠 SPF50+ 50ml",
                        ProPrice = 8M,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "9349689000927",
                        SkuNo = "9349689000927",
                        Quantity = 1,
                        ProName = "GOODCOMBO 红柳桉树蜜500G 30+",
                        ProPrice = 40M,
                        Weight = 600
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
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
                        ProNo = "9421902960055",
                        SkuNo = "9421902960055",
                        Quantity = 6,
                        ProName = "A2白金幼儿配方奶粉 3段（12个月以上）",
                        ProPrice = 33M,
                        Weight = 1250
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
        }

        [Fact]
        public void SplitTest7()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9421902960055",
                        SkuNo = "9421902960055",
                        Quantity = 6,
                        ProName = "A2白金幼儿配方奶粉 3段（12个月以上）",
                        ProPrice = 33M,
                        Weight = 1250
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
        }

        [Fact]
        public void SplitTest8()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9314839006814",
                        SkuNo = "9314839006814",
                        Quantity = 10,
                        ProName = "Ego QV儿童3合1 洗护洁面洗发沐浴液350g",
                        ProPrice = 10.76M,
                        Weight = 430
                    },
                    new Product()
                    {
                        ProNo = "9314839011931",
                        SkuNo = "9314839011931",
                        Quantity = 10,
                        ProName = "Ego QV婴儿洗发沐浴2合1 无泪配方250g",
                        ProPrice = 6.05M,
                        Weight = 280
                    }
                },
                Type = 1
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(7, result.Item2.OrderList.Count);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Single(result.Item2.OrderList[0].ProList);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[1].SubBusinessName);
            Assert.Single(result.Item2.OrderList[1].ProList);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[2].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[2].SubBusinessName);
            Assert.Single(result.Item2.OrderList[2].ProList);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[3].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[3].SubBusinessName);
            Assert.Equal(2, result.Item2.OrderList[3].ProList.Count);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[4].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[4].SubBusinessName);
            Assert.Single(result.Item2.OrderList[4].ProList);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[5].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[5].SubBusinessName);
            Assert.Single(result.Item2.OrderList[5].ProList);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[6].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[6].SubBusinessName);
            Assert.Single(result.Item2.OrderList[6].ProList);
        }

        [Fact]
        public void SplitTest9()
        {
            var request = new SplitRequest()
            {
                OrderId = "5ef94dd0b936477c8a0dd63aa68154e",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9421902960055",
                        SkuNo = "9421902960055",
                        Quantity = 6,
                        ProName = "A2白金幼儿配方奶粉 3段（12个月以上）",
                        ProPrice = 32M,
                        Weight = 1200
                    },
                    new Product()
                    {
                        ProNo = "9421902960031",
                        SkuNo = "9421902960031",
                        Quantity = 3,
                        ProName = "A2白金幼儿配方奶粉 1段（初生婴儿）",
                        ProPrice = 37M,
                        Weight = 1200
                    }
                },
                Type = 3
            };
            var result = this._splitService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("EWE经济线6罐奶粉", result.Item2.OrderList[0].SubBusinessName);
            Assert.Single(result.Item2.OrderList[0].ProList);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("EWE经济线1-3罐奶粉", result.Item2.OrderList[1].SubBusinessName);
            Assert.Single(result.Item2.OrderList[1].ProList);
        }
    }
}
