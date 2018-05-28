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
    public class SplitServiceInAstraeaAssistant_Test
    {
        private readonly SplitServiceOld _splitService;

        public SplitServiceInAstraeaAssistant_Test()
        {
            _splitService = new SplitServiceOld();
            _splitService.Initialize(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SplitRuleXml/AstraeaAssistant"));
        }

        [Fact]
        public void SplitWithExp1Test1()
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
            var result = this._splitService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("CNP Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中邮混装线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test2()
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
            var result = this._splitService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Single(result.Item2.OrderList);
            Assert.Equal("CNP Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test3()
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
            var result = this._splitService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("CNP Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("CNP Express", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("中邮杂货专线", result.Item2.OrderList[0].SubBusinessName);
        }

        [Fact]
        public void SplitWithExp1Test4()
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
            var result = this._splitService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            Assert.Equal("AOLAU EXPRESS", result.Item2.OrderList[0].LogisticsName);
            Assert.Equal("澳通速递杂货混装线", result.Item2.OrderList[0].SubBusinessName);
            Assert.Equal("AOLAU EXPRESS", result.Item2.OrderList[1].LogisticsName);
            Assert.Equal("澳通速递杂货混装线", result.Item2.OrderList[1].SubBusinessName);
            Console.Out.WriteLine(result.Item2);
        }
    }
}
