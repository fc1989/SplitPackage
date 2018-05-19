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
    public class SplitAppService_Test : SplitPackageTestBase
    {
        private readonly ISplitService _splitAppService;

        public SplitAppService_Test()
        {
            _splitAppService = Resolve<ISplitService>();
        }

        [Fact]
        public void AllLogisticTest()
        {
            var files = Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SplitPackageRules", "Rules"));
            Tuple<string, SplitedOrder> result = null;
            foreach (var item in files)
            {
                SplitPackageConfig rule = XmlHelper.LoadXmlFile<SplitPackageConfig>(item);
                var isSpecial = item.Contains("Rule_EWEExpress Standard new.xml");
                var request = new SplitWithExpRequest()
                {
                    UserName = "admin",
                    OrderId = "18040300110001",
                    ProList = new List<Product>() {
                        new Product()
                        {
                            ProNo = "",
                            SkuNo = isSpecial ? "10000264" : "10000027",
                            Quantity = 1,
                            ProName = isSpecial ? "Refresh Eye Drops  0.04眼药水" : "S26 Stage 3新版 1.1",
                            ProPrice = 10,
                            Weight = 10
                        }
                    },
                    TotalQuantity = 1,
                    LogisticsName = rule.OrganizationName,
                    GradeName = "标准型"
                };
                result = this._splitAppService.SplitWithOrganization(request);
                Assert.True(result.Item2.OrderList.Count > 0);
                var request1 = new SplitWithExpRequest1()
                {
                    UserName = "admin",
                    OrderId = "18040300110001",
                    ProList = new List<Product>() {
                        new Product()
                        {
                            ProNo = "",
                            Quantity = 1,
                            ProName = isSpecial ? "Refresh Eye Drops  0.04眼药水" : "S26 Stage 3新版 1.1",
                            ProPrice = 10,
                            Weight = 10,
                            PTId = isSpecial ? 1010703 : 1019904
                        }
                    },
                    TotalQuantity = 1,
                    logistics = new List<string> { rule.OrganizationName }
                };
                result = this._splitAppService.SplitWithOrganization1(request1);
                Assert.True(result.Item2.OrderList.Count > 0);
            }
        }

        [Fact]
        public void SplitInputValidTest()
        {
            Tuple<string, SplitedOrder> result = null;
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26098951",
                        Quantity = 1,
                        ProName = "MAMIA婴幼儿奶粉二段900G二段",
                        ProPrice = 100,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                Type = 3
            };
            //商品重量不能为0
            request.ProList.ForEach(o => o.Weight = 0);
            result = this._splitAppService.Split(request);
            Assert.Equal("商品重量必须大于0", result.Item1);
            //商品价格不能为0
            request.ProList.ForEach(o => { o.ProPrice = 0; o.Weight = 100; });
            result = this._splitAppService.Split(request);
            Assert.Equal("商品价格必须大于0", result.Item1);
            //商品数量不能为0
            request.ProList.ForEach(o => { o.Quantity = 0; o.ProPrice = 100; });
            result = this._splitAppService.Split(request);
            Assert.Equal("商品数量必须大于0", result.Item1);
            //该商品sku商品库不存在
            request.ProList.ForEach(o => { o.SkuNo = "0000000000"; o.Quantity = 1; });
            result = this._splitAppService.Split(request);
            Assert.Equal("不存在SkuNo:0000000000", result.Item1);
        }

        [Fact]
        public void SplitWithExpInputValidTest()
        {
            Tuple<string, SplitedOrder> result = null;
            var request = new SplitWithExpRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26098951",
                        Quantity = 1,
                        ProName = "MAMIA婴幼儿奶粉二段900G二段",
                        ProPrice = 100,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                LogisticsName = "AOLAU EXPRESS",
                GradeName = "标准型"
            };
            //商品重量不能为0
            request.ProList.ForEach(o => o.Weight = 0);
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("商品重量必须大于0", result.Item1);
            //商品价格不能为0
            request.ProList.ForEach(o => { o.ProPrice = 0; o.Weight = 100; });
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("商品价格必须大于0", result.Item1);
            //商品数量不能为0
            request.ProList.ForEach(o => { o.Quantity = 0; o.ProPrice = 100; });
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("商品数量必须大于0", result.Item1);
            //指定物流商不存在
            request.ProList.ForEach(o => { o.Quantity = 1; });
            request.LogisticsName = "123";
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("不存在123（标准型）的规则", result.Item1);
            request.LogisticsName = "AOLAU EXPRESS";
            request.GradeName = "标准型123";
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("不存在AOLAU EXPRESS（标准型123）的规则", result.Item1);
            request.LogisticsName = "";
            request.GradeName = "";
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("请提供指定物流商", result.Item1);
            //该商品sku商品库不存在
            request.LogisticsName = "AOLAU EXPRESS";
            request.GradeName = "标准型";
            request.ProList.ForEach(o => { o.SkuNo = "0000000000"; });
            result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal("不存在SkuNo:0000000000", result.Item1);
        }

        [Fact]
        public void SplitWithExp1InputValidTest()
        {
            Tuple<string, SplitedOrder> result = null;
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
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
                        PTId = 1010703
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100,
                        PTId = 1010704
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "AOLAU EXPRESS" }
            };
            //商品重量不能为0
            request.ProList.ForEach(o => o.Weight = 0);
            result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal("商品重量必须大于0", result.Item1);
            //商品价格不能为0
            request.ProList.ForEach(o => { o.ProPrice = 0; o.Weight = 100; });
            result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal("商品价格必须大于0", result.Item1);
            //商品数量不能为0
            request.ProList.ForEach(o => { o.Quantity = 0; o.ProPrice = 100; });
            result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal("商品数量必须大于0", result.Item1);
            //指定物流商不存在
            request.ProList.ForEach(o=>o.Quantity = 1);
            request.logistics = new List<string> { };
            result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal("请提供指定物流商", result.Item1);
            request.logistics = new List<string> { "123","1"};
            result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal("指定物流商:123,1不存在", result.Item1);
            //无效的PTId
            request.ProList.ForEach(o => { o.PTId = 0; });
            result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal("不存在PTId:0", result.Item1);
        }

        [Fact]
        public void SplitTest()
        {
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26098951",
                        Quantity = 1,
                        ProName = "MAMIA婴幼儿奶粉二段900G二段",
                        ProPrice = 100,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                Type = 3
            };
            var result = this._splitAppService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("转运四方现代物流3罐婴儿奶粉专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExpTest()
        {
            var request = new SplitWithExpRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26098951",
                        Quantity = 1,
                        ProName = "MAMIA婴幼儿奶粉二段900G二段",
                        ProPrice = 100,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                LogisticsName = "AOLAU EXPRESS",
                GradeName = "标准型"
            };
            var result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("澳通速递奶粉线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
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
                        PTId = 1010703
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "26113807",
                        Quantity = 2,
                        ProName = "MAMIA婴幼儿奶粉三段900G三段",
                        ProPrice = 100,
                        Weight = 100,
                        PTId = 1010704
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "AOLAU EXPRESS" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("澳通速递奶粉线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitTest_1()
        {
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 1,
                        ProName = "NOZOHAEM止鼻血凝胶4粒",
                        ProPrice = 57,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "735282370007",
                        Quantity = 1,
                        ProName = "Munchkin草饲牧牛婴儿配方奶粉1段",
                        ProPrice = 57,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "768990017414",
                        Quantity = 1,
                        ProName = "孕妇DHA",
                        ProPrice = 57,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                Type = 3
            };
            var result = this._splitAppService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中环杂货混装线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExpTest_1()
        {
            var request = new SplitWithExpRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 1,
                        ProName = "NOZOHAEM止鼻血凝胶4粒",
                        ProPrice = 1,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "735282370007",
                        Quantity = 1,
                        ProName = "Munchkin草饲牧牛婴儿配方奶粉1段",
                        ProPrice = 1,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "768990017414",
                        Quantity = 1,
                        ProName = "孕妇DHA",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                LogisticsName = "CNP Express",
                GradeName = "标准型"
            };
            var result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中邮混装线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test_1()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "391709501015",
                        Quantity = 1,
                        ProName = "NOZOHAEM止鼻血凝胶4粒",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 9029900
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "735282370007",
                        Quantity = 1,
                        ProName = "Munchkin草饲牧牛婴儿配方奶粉1段",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 1010703
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "768990017414",
                        Quantity = 1,
                        ProName = "孕妇DHA",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 1019904
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "CNP Express" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中邮混装线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitTest_2()
        {
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "735282490033",
                        Quantity = 10,
                        ProName = "MUNCHKIN碗",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                Type = 3
            };
            var result = this._splitAppService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExpTest_2()
        {
            var request = new SplitWithExpRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "735282490033",
                        Quantity = 10,
                        ProName = "MUNCHKIN碗",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                LogisticsName = "CNP Express",
                GradeName = "标准型"
            };
            var result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test_2()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "735282490033",
                        Quantity = 10,
                        ProName = "MUNCHKIN碗",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 9029900
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "CNP Express" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitTest_3()
        {
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "852696000204",
                        Quantity = 15,
                        ProName = "COBRAMESTATE橄榄油750ML",
                        ProPrice = 1,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "857052004445",
                        Quantity = 10,
                        ProName = "SKIPHOP围兜",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 25,
                Type = 3
            };
            var result = this._splitAppService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[1].SubBusinessName);
        }

        [Fact]
        public void SplitWithExpTest_3()
        {
            var request = new SplitWithExpRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "852696000204",
                        Quantity = 15,
                        ProName = "COBRAMESTATE橄榄油750ML",
                        ProPrice = 1,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "857052004445",
                        Quantity = 10,
                        ProName = "SKIPHOP围兜",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                LogisticsName = "CNP Express",
                GradeName = "标准型"
            };
            var result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[1].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test_3()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "852696000204",
                        Quantity = 15,
                        ProName = "COBRAMESTATE橄榄油750ML",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 1019903
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "857052004445",
                        Quantity = 10,
                        ProName = "SKIPHOP围兜",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 9029900
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "CNP Express" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[1].SubBusinessName);
        }

        [Fact]
        public void SplitTest_4()
        {
            var request = new SplitRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "70000137",
                        Quantity = 10,
                        ProName = "蜡笔细长0.15",
                        ProPrice = 1,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "93262170",
                        Quantity = 5,
                        ProName = "BONJELA口腔凝胶15G",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 15,
                Type = 3
            };
            var result = this._splitAppService.Split(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("方舟AlphaEX", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExpTest_4()
        {
            var request = new SplitWithExpRequest()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "70000137",
                        Quantity = 10,
                        ProName = "蜡笔细长0.15",
                        ProPrice = 1,
                        Weight = 100
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "93262170",
                        Quantity = 5,
                        ProName = "BONJELA口腔凝胶15G",
                        ProPrice = 1,
                        Weight = 100
                    }
                },
                TotalQuantity = 1,
                LogisticsName = "AOLAU EXPRESS",
                GradeName = "标准型"
            };
            var result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("澳通速递杂货混装线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("澳通速递杂货混装线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test_4()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "70000137",
                        Quantity = 10,
                        ProName = "蜡笔细长0.15",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 1019903
                    },
                    new Product()
                    {
                        ProNo = "",
                        SkuNo = "93262170",
                        Quantity = 5,
                        ProName = "BONJELA口腔凝胶15G",
                        ProPrice = 1,
                        Weight = 100,
                        PTId = 1019904
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "AOLAU EXPRESS" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("澳通速递杂货混装线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("澳通速递杂货混装线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test_5()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
                OrderId = "18040300110001",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "9310160814098",
                        SkuNo = "9310160814098",
                        Quantity = 1,
                        ProName = "Menevit 爱乐维男性备孕营养素 90粒装",
                        ProPrice = 5.69M,
                        Weight = 200,
                        PTId = 101990401
                    },
                    new Product()
                    {
                        ProNo = "9320971310566",
                        SkuNo = "9320971310566",
                        Quantity = 2,
                        ProName = "全脂奶粉",
                        ProPrice = 5.69M,
                        Weight = 200,
                        PTId = 1010701
                    }
                },
                TotalQuantity = 1,
                logistics = new List<string> { "EWE Express 标准线" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("EWE Express 标准线", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("EWE杂货标准线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("EWE Express 经济线", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("EWE杂货经济线", result.Item2.OrderList[1].SubBusinessName);
        }
    }
}
