using SplitPackage.Split;
using SplitPackage.Split.Dto;
using SplitPackage.Split.SplitModels;
using System.Collections.Generic;
using Xunit;

namespace SplitPackage.Tests.Split
{
    public class SplitAppService_Tesst : SplitPackageTestBase
    {
        private readonly ISplitService _splitAppService;

        public SplitAppService_Tesst()
        {
            _splitAppService = Resolve<ISplitService>();
        }

        [Fact]
        public void SplitTest()
        {
            //按件数，件数多的优先
            //按重量，重量大的优先
            //按混装种类，种类多的优先
            var request = new SplitWithExpRequest() {
                UserName = "admin",
                OrderId = "123",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "XSM001",
                        SkuNo = "9310160820488",
                        Quantity = 8,
                        ProName = "Elevit 爱乐维 备孕孕妇补充叶酸 100片",
                        ProPrice = 10,
                        Weight = 10
                    }
                },
                TotalQuantity = 8,
                LogisticsName = "CNP Express",
                GradeName = "标准型"
            };
            var result = this._splitAppService.SplitWithOrganization(request);
            Assert.Equal( string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            var od = result.Item2.OrderList[0];
            Assert.Equal("12301",od.Id);
            Assert.Equal("CNP Express", od.LogisticsName);
            Assert.Equal("www.auexpress.com.au", od.URL);
            Assert.Equal("中邮混装线", od.SubBusinessName);
            Assert.Equal((decimal)0.45, od.LogisticsUnitPrice,2);
            Assert.Equal(50, od.TotalWeight);
            Assert.Equal((decimal)4.5, od.LogisticsCost, 1);
            Assert.Equal(0, od.TaxCost);
            Assert.Equal(50, od.TotalPrice);
            Assert.Single(od.ProList);
            Assert.Equal("XSM001", od.ProList[0].ProNo);
            Assert.Equal("9310160820488", od.ProList[0].SkuNo);
            Assert.Equal(5, od.ProList[0].Quantity);
            Assert.Equal("Elevit 爱乐维 备孕孕妇补充叶酸 100片", od.ProList[0].ProName);
            Assert.Equal(10, od.ProList[0].ProPrice);
            Assert.Equal(10, od.ProList[0].Weight);
            Assert.Equal(101990401, od.ProList[0].PTId);
            od = result.Item2.OrderList[1];
            Assert.Equal("12302", od.Id);
            Assert.Equal("CNP Express", od.LogisticsName);
            Assert.Equal("www.auexpress.com.au", od.URL);
            Assert.Equal("中邮混装线", od.SubBusinessName);
            Assert.Equal((decimal)0.45, od.LogisticsUnitPrice, 2);
            Assert.Equal(30, od.TotalWeight);
            Assert.Equal((decimal)4.5, od.LogisticsCost, 1);
            Assert.Equal(0, od.TaxCost);
            Assert.Equal(30, od.TotalPrice);
            Assert.Single(od.ProList);
            Assert.Equal("XSM001", od.ProList[0].ProNo);
            Assert.Equal("9310160820488", od.ProList[0].SkuNo);
            Assert.Equal(3, od.ProList[0].Quantity);
            Assert.Equal("Elevit 爱乐维 备孕孕妇补充叶酸 100片", od.ProList[0].ProName);
            Assert.Equal(10, od.ProList[0].ProPrice);
            Assert.Equal(10, od.ProList[0].Weight);
            Assert.Equal(101990401, od.ProList[0].PTId);
        }

        [Fact]
        public void SplitWithExp1Test()
        {
            var request = new SplitWithExpRequest1()
            {
                UserName = "admin",
                OrderId = "123",
                ProList = new List<Product>() {
                    new Product()
                    {
                        ProNo = "XSM001",
                        SkuNo = "9310160820488",
                        Quantity = 8,
                        ProName = "Elevit 爱乐维 备孕孕妇补充叶酸 100片",
                        ProPrice = 10,
                        Weight = 10,
                        PTId = 101990401
                    }
                },
                TotalQuantity = 8,
                logistics = new List<string> { "CNP Express" }
            };
            var result = this._splitAppService.SplitWithOrganization1(request);
            Assert.Equal(string.Empty, result.Item1);
            Assert.Equal(2, result.Item2.OrderList.Count);
            var od = result.Item2.OrderList[0];
            Assert.Equal("12301", od.Id);
            Assert.Equal("CNP Express", od.LogisticsName);
            Assert.Equal("www.auexpress.com.au", od.URL);
            Assert.Equal("中邮混装线", od.SubBusinessName);
            Assert.Equal((decimal)0.45, od.LogisticsUnitPrice, 2);
            Assert.Equal(50, od.TotalWeight);
            Assert.Equal((decimal)4.5, od.LogisticsCost, 1);
            Assert.Equal(0, od.TaxCost);
            Assert.Equal(50, od.TotalPrice);
            Assert.Single(od.ProList);
            Assert.Equal("XSM001", od.ProList[0].ProNo);
            Assert.Equal("9310160820488", od.ProList[0].SkuNo);
            Assert.Equal(5, od.ProList[0].Quantity);
            Assert.Equal("Elevit 爱乐维 备孕孕妇补充叶酸 100片", od.ProList[0].ProName);
            Assert.Equal(10, od.ProList[0].ProPrice);
            Assert.Equal(10, od.ProList[0].Weight);
            Assert.Equal(101990401, od.ProList[0].PTId);
            od = result.Item2.OrderList[1];
            Assert.Equal("12302", od.Id);
            Assert.Equal("CNP Express", od.LogisticsName);
            Assert.Equal("www.auexpress.com.au", od.URL);
            Assert.Equal("中邮混装线", od.SubBusinessName);
            Assert.Equal((decimal)0.45, od.LogisticsUnitPrice, 2);
            Assert.Equal(30, od.TotalWeight);
            Assert.Equal((decimal)4.5, od.LogisticsCost, 1);
            Assert.Equal(0, od.TaxCost);
            Assert.Equal(30, od.TotalPrice);
            Assert.Single(od.ProList);
            Assert.Equal("XSM001", od.ProList[0].ProNo);
            Assert.Equal("9310160820488", od.ProList[0].SkuNo);
            Assert.Equal(3, od.ProList[0].Quantity);
            Assert.Equal("Elevit 爱乐维 备孕孕妇补充叶酸 100片", od.ProList[0].ProName);
            Assert.Equal(10, od.ProList[0].ProPrice);
            Assert.Equal(10, od.ProList[0].Weight);
            Assert.Equal(101990401, od.ProList[0].PTId);
        }
    }
}
